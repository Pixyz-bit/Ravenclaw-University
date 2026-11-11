using System;
using System.Collections.Generic;

namespace _241611JalopFinalQuiz2.Models;

public partial class FounderTable
{
    public int FounderId { get; set; }

    public string FounderName { get; set; } = null!;

    public DateOnly? Birthday { get; set; }

    public string? Description { get; set; }

    public string? FounderMessage { get; set; }
}
