using System;
using System.Collections.Generic;

namespace _241611JalopFinalQuiz2.Models;

public partial class ContactsTable
{
    public int ContactId { get; set; }

    public string? AcademyAddress { get; set; }

    public string? Email { get; set; }

    public string? Telephone { get; set; }

    public string? OfficeHours { get; set; }

    public TimeOnly? TimeIn { get; set; }

    public TimeOnly? TimeOut { get; set; }
}
