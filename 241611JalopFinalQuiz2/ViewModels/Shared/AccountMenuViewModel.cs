using System.Globalization;

namespace _241611JalopFinalQuiz2.ViewModels;

public record AccountMenuViewModel(string DisplayName, string Username, string? ProfileImage)
{
    public string Initials => string.Concat(DisplayName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Take(2).Select(part => StringInfo.GetNextTextElement(part))).ToUpperInvariant();
}
