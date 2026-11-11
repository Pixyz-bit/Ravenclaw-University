using System;
using System.Collections.Generic;

namespace _241611JalopFinalQuiz2.Models;

public partial class ProgramsTable
{
    public int SubjectId { get; set; }

    public string SubjectName { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImagePath { get; set; }
}
