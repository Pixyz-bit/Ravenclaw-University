using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Models;

namespace _241611JalopFinalQuiz2.Data;

public partial class UniversityDbContext : DbContext
{
    public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AboutImagesTable> AboutImagesTables { get; set; }

    public virtual DbSet<AcademyInfoTable> AcademyInfoTables { get; set; }

    public virtual DbSet<ContactsTable> ContactsTables { get; set; }

    public virtual DbSet<CoreValuesTable> CoreValuesTables { get; set; }

    public virtual DbSet<FounderTable> FounderTables { get; set; }

    public virtual DbSet<ProgramsTable> ProgramsTables { get; set; }

    public virtual DbSet<SocialLinksTable> SocialLinksTables { get; set; }

    public virtual DbSet<UserTable> UserTables { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AboutImagesTable>(entity =>
        {
            entity.HasKey(e => e.AboutImageId).HasName("PK__AboutIma__4B9E4150EC311A20");

            entity.ToTable("AboutImagesTable");

            entity.Property(e => e.AltText).HasMaxLength(250);
            entity.Property(e => e.ImagePath).HasMaxLength(500);
        });

        modelBuilder.Entity<AcademyInfoTable>(entity =>
        {
            entity.HasKey(e => e.AcademyInfoId).HasName("PK__AcademyI__488BB1F170D17403");

            entity.ToTable("AcademyInfoTable");

            entity.Property(e => e.HeroImg).HasMaxLength(500);
        });

        modelBuilder.Entity<ContactsTable>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__Contacts__5C66259B04CA2126");

            entity.ToTable("ContactsTable");

            entity.Property(e => e.AcademyAddress).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(254);
            entity.Property(e => e.OfficeHours).HasMaxLength(300);
            entity.Property(e => e.Telephone).HasMaxLength(50);
        });

        modelBuilder.Entity<CoreValuesTable>(entity =>
        {
            entity.HasKey(e => e.CoreValueId).HasName("PK__CoreValu__5FE7767ED07FB46F");

            entity.ToTable("CoreValuesTable");

            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<FounderTable>(entity =>
        {
            entity.HasKey(e => e.FounderId).HasName("PK__FounderT__CC808F81BBA72E36");

            entity.ToTable("FounderTable");

            entity.Property(e => e.FounderName).HasMaxLength(150);
        });

        modelBuilder.Entity<ProgramsTable>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Programs__AC1BA3A80F252D4D");

            entity.ToTable("ProgramsTable");

            entity.Property(e => e.ImagePath).HasMaxLength(500);
            entity.Property(e => e.SubjectName).HasMaxLength(150);
        });

        modelBuilder.Entity<SocialLinksTable>(entity =>
        {
            entity.HasKey(e => e.SocialLinkId).HasName("PK__SocialLi__97EE69659A22CB80");

            entity.ToTable("SocialLinksTable");

            entity.Property(e => e.IconClass).HasMaxLength(100);
            entity.Property(e => e.PlatformName).HasMaxLength(50);
            entity.Property(e => e.Url).HasMaxLength(500);
        });

        modelBuilder.Entity<UserTable>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserTabl__1788CC4C2530E88C");

            entity.ToTable("UserTable");

            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MiddleName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.ProfileImg).HasMaxLength(500);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
