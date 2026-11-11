using System.Data;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.Models;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Services;

public enum ContentSaveResult { Saved, Missing, SingletonExists }

public class ContentEditorService(UniversityDbContext database)
{
    public async Task<List<ContentEditorViewModel>> ListAsync(ContentSection section) => section switch
    {
        ContentSection.University => await database.AcademyInfoTables.AsNoTracking().OrderBy(x => x.AcademyInfoId)
            .Select(x => new ContentEditorViewModel { Section = section, Id = x.AcademyInfoId, Title = "University information", ImagePath = x.HeroImg, Description = x.AcademyDescription, Mission = x.Mission, Vision = x.Vision }).ToListAsync(),
        ContentSection.Contacts => await database.ContactsTables.AsNoTracking().OrderBy(x => x.ContactId)
            .Select(x => new ContentEditorViewModel { Section = section, Id = x.ContactId, Title = "Contact information", Address = x.AcademyAddress, Email = x.Email, Telephone = x.Telephone, OfficeHours = x.OfficeHours }).ToListAsync(),
        ContentSection.Values => await database.CoreValuesTables.AsNoTracking().OrderBy(x => x.CoreValueId)
            .Select(x => new ContentEditorViewModel { Section = section, Id = x.CoreValueId, Title = x.Title }).ToListAsync(),
        ContentSection.Photos => await database.AboutImagesTables.AsNoTracking().OrderBy(x => x.AboutImageId)
            .Select(x => new ContentEditorViewModel { Section = section, Id = x.AboutImageId, Title = x.AltText ?? "Gallery image", ImagePath = x.ImagePath, AltText = x.AltText }).ToListAsync(),
        ContentSection.Programs => await database.ProgramsTables.AsNoTracking().OrderBy(x => x.SubjectId)
            .Select(x => new ContentEditorViewModel { Section = section, Id = x.SubjectId, Title = x.SubjectName, Description = x.Description, ImagePath = x.ImagePath }).ToListAsync(),
        ContentSection.Founders => await database.FounderTables.AsNoTracking().OrderBy(x => x.FounderId)
            .Select(x => new ContentEditorViewModel { Section = section, Id = x.FounderId, Title = x.FounderName, Birthday = x.Birthday, Description = x.Description, Message = x.FounderMessage }).ToListAsync(),
        ContentSection.SocialLinks => await database.SocialLinksTables.AsNoTracking().OrderBy(x => x.SocialLinkId)
            .Select(x => new ContentEditorViewModel { Section = section, Id = x.SocialLinkId, Title = x.PlatformName, Url = x.Url, IconClass = x.IconClass }).ToListAsync(),
        _ => throw new ArgumentOutOfRangeException(nameof(section))
    };

    public async Task<ContentSaveResult> SaveAsync(ContentEditorViewModel model)
    {
        await using var transaction = await database.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        if (model.Id == 0 && ContentSections.IsSingleton(model.Section))
        {
            var exists = model.Section == ContentSection.University
                ? await database.AcademyInfoTables.AnyAsync() : await database.ContactsTables.AnyAsync();
            if (exists) return ContentSaveResult.SingletonExists;
        }

        switch (model.Section)
        {
            case ContentSection.University:
                var university = model.Id == 0 ? new AcademyInfoTable() : await database.AcademyInfoTables.FindAsync(model.Id);
                if (university is null) return ContentSaveResult.Missing;
                university.HeroImg = Clean(model.ImagePath); university.AcademyDescription = Clean(model.Description);
                university.Mission = Clean(model.Mission); university.Vision = Clean(model.Vision);
                if (model.Id == 0) database.AcademyInfoTables.Add(university);
                break;
            case ContentSection.Contacts:
                var contact = model.Id == 0 ? new ContactsTable() : await database.ContactsTables.FindAsync(model.Id);
                if (contact is null) return ContentSaveResult.Missing;
                contact.AcademyAddress = Clean(model.Address); contact.Email = Clean(model.Email);
                contact.Telephone = Clean(model.Telephone); contact.OfficeHours = Clean(model.OfficeHours);
                if (model.Id == 0) database.ContactsTables.Add(contact);
                break;
            case ContentSection.Values:
                var value = model.Id == 0 ? new CoreValuesTable() : await database.CoreValuesTables.FindAsync(model.Id);
                if (value is null) return ContentSaveResult.Missing;
                value.Title = model.Title!.Trim();
                if (model.Id == 0) database.CoreValuesTables.Add(value);
                break;
            case ContentSection.Photos:
                var photo = model.Id == 0 ? new AboutImagesTable() : await database.AboutImagesTables.FindAsync(model.Id);
                if (photo is null) return ContentSaveResult.Missing;
                photo.ImagePath = model.ImagePath!.Trim(); photo.AltText = Clean(model.AltText);
                if (model.Id == 0) database.AboutImagesTables.Add(photo);
                break;
            case ContentSection.Programs:
                var program = model.Id == 0 ? new ProgramsTable() : await database.ProgramsTables.FindAsync(model.Id);
                if (program is null) return ContentSaveResult.Missing;
                program.SubjectName = model.Title!.Trim(); program.Description = Clean(model.Description); program.ImagePath = Clean(model.ImagePath);
                if (model.Id == 0) database.ProgramsTables.Add(program);
                break;
            case ContentSection.Founders:
                var founder = model.Id == 0 ? new FounderTable() : await database.FounderTables.FindAsync(model.Id);
                if (founder is null) return ContentSaveResult.Missing;
                founder.FounderName = model.Title!.Trim(); founder.Birthday = model.Birthday;
                founder.Description = Clean(model.Description); founder.FounderMessage = Clean(model.Message);
                if (model.Id == 0) database.FounderTables.Add(founder);
                break;
            case ContentSection.SocialLinks:
                var social = model.Id == 0 ? new SocialLinksTable() : await database.SocialLinksTables.FindAsync(model.Id);
                if (social is null) return ContentSaveResult.Missing;
                social.PlatformName = model.Title!.Trim(); social.Url = model.Url!.Trim(); social.IconClass = Clean(model.IconClass);
                if (model.Id == 0) database.SocialLinksTables.Add(social);
                break;
            default: return ContentSaveResult.Missing;
        }
        await database.SaveChangesAsync();
        await transaction.CommitAsync();
        return ContentSaveResult.Saved;
    }

    public async Task<bool> DeleteAsync(ContentSection section, int id)
    {
        object? entity = section switch
        {
            ContentSection.Values => await database.CoreValuesTables.FindAsync(id),
            ContentSection.Photos => await database.AboutImagesTables.FindAsync(id),
            ContentSection.Programs => await database.ProgramsTables.FindAsync(id),
            ContentSection.Founders => await database.FounderTables.FindAsync(id),
            ContentSection.SocialLinks => await database.SocialLinksTables.FindAsync(id),
            _ => null // Single university/contact records are edited, never deleted here.
        };
        if (entity is null) return false;
        database.Remove(entity);
        await database.SaveChangesAsync();
        return true;
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
