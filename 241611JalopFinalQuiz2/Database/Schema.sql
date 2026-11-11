-- Ravenclaw University Db: schema for a fresh database.
-- Single consolidated schema, including all applied changes as of 2026-11-11.
-- Verified against live column metadata, primary keys, identities, and role CHECK.
-- Run once in SSMS or the MSSQL extension. This is not an update script.
-- GO separates batches and is handled by those tools.

CREATE DATABASE [Ravenclaw University Db];
GO

USE [Ravenclaw University Db];
GO

CREATE TABLE dbo.AcademyInfoTable
(
    AcademyInfoId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    HeroImg NVARCHAR(500) NULL,
    AcademyDescription NVARCHAR(MAX) NULL,
    Mission NVARCHAR(MAX) NULL,
    Vision NVARCHAR(MAX) NULL
);
GO

CREATE TABLE dbo.ContactsTable
(
    ContactId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    AcademyAddress NVARCHAR(500) NULL,
    Email NVARCHAR(254) NULL,
    Telephone NVARCHAR(50) NULL,
    OfficeHours NVARCHAR(300) NULL
);
GO

CREATE TABLE dbo.AboutImagesTable
(
    AboutImageId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ImagePath NVARCHAR(500) NOT NULL,
    AltText NVARCHAR(250) NULL
);
GO

CREATE TABLE dbo.UserTable
(
    UserId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Role NVARCHAR(20) NOT NULL
        CHECK (Role IN (N'Admin', N'Student')),
    Username NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    ProfileImg NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT (1),
    LastName NVARCHAR(100) NULL,
    FirstName NVARCHAR(100) NULL,
    MiddleName NVARCHAR(100) NULL,
    Birthday DATE NULL
);
GO

CREATE TABLE dbo.FounderTable
(
    FounderId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    FounderName NVARCHAR(150) NOT NULL,
    Birthday DATE NULL,
    Description NVARCHAR(MAX) NULL,
    FounderMessage NVARCHAR(MAX) NULL
);
GO

CREATE TABLE dbo.CoreValuesTable
(
    CoreValueId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Title NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE dbo.ProgramsTable
(
    SubjectId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    SubjectName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    ImagePath NVARCHAR(500) NULL
);
GO

CREATE TABLE dbo.SocialLinksTable
(
    SocialLinkId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PlatformName NVARCHAR(50) NOT NULL,
    Url NVARCHAR(500) NOT NULL,
    IconClass NVARCHAR(100) NULL
);
GO
