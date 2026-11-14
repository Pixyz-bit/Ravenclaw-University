-- Optional approved CONTENT only; never run automatically. No schema changes.
-- Inserts Rowena when absent; updates only her biography when already present.
-- Other founders, birthdays, and stored messages remain unchanged.
USE [Ravenclaw University Db];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;

DECLARE @Biography NVARCHAR(MAX) = N'Rowena Ravenclaw was a gifted witch celebrated for her intelligence, creativity, and love of learning. One of the four founders of Hogwarts and the founder of Ravenclaw House, she valued wit, wisdom, and an inquisitive mind.'
    + NCHAR(10) + NCHAR(10)
    + N'Her legacy inspires Ravenclaw University’s commitment to independent thought, thoughtful discovery, and a lifelong pursuit of knowledge.';
DECLARE @Matches INT = (
    SELECT COUNT(*) FROM dbo.FounderTable WITH (UPDLOCK, HOLDLOCK)
    WHERE LTRIM(RTRIM(FounderName)) COLLATE Latin1_General_100_CI_AS = N'Rowena Ravenclaw'
);
IF @Matches > 1
    THROW 50001, 'Multiple Rowena Ravenclaw entries exist. Review them before publishing.', 1;
IF @Matches = 0
    INSERT INTO dbo.FounderTable (FounderName, Birthday, Description, FounderMessage)
    VALUES (N'Rowena Ravenclaw', NULL, @Biography, NULL);
ELSE
    UPDATE dbo.FounderTable SET Description = @Biography
    WHERE LTRIM(RTRIM(FounderName)) COLLATE Latin1_General_100_CI_AS = N'Rowena Ravenclaw';

COMMIT TRANSACTION;
SELECT FounderId, FounderName, CASE WHEN Description = @Biography THEN 1 ELSE 0 END AS ApprovedBiographyMatches
FROM dbo.FounderTable
WHERE LTRIM(RTRIM(FounderName)) COLLATE Latin1_General_100_CI_AS = N'Rowena Ravenclaw';
GO
