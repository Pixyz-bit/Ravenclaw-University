using System.Buffers.Binary;

namespace _241611JalopFinalQuiz2.Services;

// Stores uploaded image files separately from source assets and database records.
public class ImageUploadService(IWebHostEnvironment environment, IConfiguration configuration)
{
    public const long MaxBytes = 5 * 1024 * 1024;
    public string RootPath { get; } = Path.GetFullPath(configuration["ImageUploads:RootPath"]
        ?? Path.Combine(environment.ContentRootPath, "App_Data", "Uploads"));

    public async Task<string?> ValidateAsync(IFormFile? file)
    {
        if (file is null) return null;
        if (file.Length == 0 || file.Length > MaxBytes) return "Choose an image between 1 byte and 5 MB.";
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension is not (".png" or ".jpg" or ".jpeg" or ".webp")) return "Choose a PNG, JPEG, or WebP image.";
        await using var stream = file.OpenReadStream();
        var header = new byte[12];
        var count = await stream.ReadAtLeastAsync(header, 12, throwOnEndOfStream: false);
        var matches = count == 12 && (extension switch
        {
            ".png" => header.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
            ".jpg" or ".jpeg" => header[0] == 255 && header[1] == 216 && header[2] == 255,
            ".webp" => header.AsSpan(0, 4).SequenceEqual("RIFF"u8) && header.AsSpan(8, 4).SequenceEqual("WEBP"u8)
                && BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4, 4)) == file.Length - 8,
            _ => false
        });
        return matches ? null : "The file contents do not match a supported image type.";
    }

    public async Task<string> SaveAsync(IFormFile file)
    {
        var error = await ValidateAsync(file);
        if (error is not null) throw new InvalidDataException(error);
        Directory.CreateDirectory(RootPath);
        var name = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName).ToLowerInvariant();
        var path = Path.Combine(RootPath, name);
        try
        {
            await using var input = file.OpenReadStream();
            await using var output = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            var buffer = new byte[81920];
            long total = 0;
            int count;
            while ((count = await input.ReadAsync(buffer)) > 0)
            {
                total += count;
                if (total > MaxBytes) throw new InvalidDataException("Choose an image no larger than 5 MB.");
                await output.WriteAsync(buffer.AsMemory(0, count));
            }
            if (total != file.Length) throw new InvalidDataException("The image upload was incomplete. Select the file again.");
        }
        catch { File.Delete(path); throw; }
        return "/images/uploads/" + name;
    }

    // Only used for a newly uploaded file whose database save failed.
    public void Discard(string? imagePath)
    {
        if (imagePath is null) return;
        var name = Path.GetFileName(imagePath);
        if (imagePath == "/images/uploads/" + name && Guid.TryParseExact(Path.GetFileNameWithoutExtension(name), "N", out _))
            File.Delete(Path.Combine(RootPath, name));
    }
}
