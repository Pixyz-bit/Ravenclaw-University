namespace _241611JalopFinalQuiz2.ViewModels;

public record AdminPreviewField(string Name, string Label, string? Value, string EmptyText = "Not entered yet");

public record AdminPreviewViewModel(
    string Title,
    IReadOnlyList<AdminPreviewField> Fields,
    string? ImagePath = null,
    bool ShowImage = false,
    bool Portrait = false);
