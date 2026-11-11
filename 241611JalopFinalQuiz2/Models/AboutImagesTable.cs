using System;
using System.Collections.Generic;

namespace _241611JalopFinalQuiz2.Models;

public partial class AboutImagesTable
{
    public int AboutImageId { get; set; }

    public string ImagePath { get; set; } = null!;

    public string? AltText { get; set; }
}
