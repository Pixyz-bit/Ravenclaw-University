-- Add profile details without changing existing account records.
-- New registration must require first/last name through form validation.
USE [Ravenclaw University Db];
GO

SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF COL_LENGTH(N'dbo.UserTable', N'LastName') IS NULL
BEGIN
    ALTER TABLE dbo.UserTable ADD LastName NVARCHAR(100) NULL;
END;

IF COL_LENGTH(N'dbo.UserTable', N'FirstName') IS NULL
BEGIN
    ALTER TABLE dbo.UserTable ADD FirstName NVARCHAR(100) NULL;
END;

IF COL_LENGTH(N'dbo.UserTable', N'MiddleName') IS NULL
BEGIN
    ALTER TABLE dbo.UserTable ADD MiddleName NVARCHAR(100) NULL;
END;

IF COL_LENGTH(N'dbo.UserTable', N'Birthday') IS NULL
BEGIN
    ALTER TABLE dbo.UserTable ADD Birthday DATE NULL;
END;

COMMIT TRANSACTION;
GO
