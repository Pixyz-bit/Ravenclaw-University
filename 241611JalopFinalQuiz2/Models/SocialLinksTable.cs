using System;
using System.Collections.Generic;

namespace _241611JalopFinalQuiz2.Models;

public partial class SocialLinksTable
{
    public int SocialLinkId { get; set; }

    public string PlatformName { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string? IconClass { get; set; }
}
