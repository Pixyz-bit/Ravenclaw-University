using System;
using System.Collections.Generic;

namespace _241611JalopFinalQuiz2.Models;

public partial class UserTable
{
    public int UserId { get; set; }

    public string Role { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? ProfileImg { get; set; }

    public bool IsActive { get; set; }
}
