using System;
using System.Collections.Generic;

namespace _241611JalopFinalQuiz2.Models;

public partial class FounderTable
{
    public int FounderId { get; set; }

    public string FounderName { get; set; } = null!;

    public string? Description { get; set; }
}
