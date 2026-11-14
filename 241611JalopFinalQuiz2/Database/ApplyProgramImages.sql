-- User-approved CONTENT only; this does not alter the database schema.
-- Replaces only the four documented sample-image paths. Unexpected custom paths,
-- missing subjects, or duplicate subject names stop the entire update.
USE [Ravenclaw University Db];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET LOCK_TIMEOUT 5000;
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;

DECLARE @Images TABLE (SubjectName NVARCHAR(150), OldPath NVARCHAR(500), NewPath NVARCHAR(500));
INSERT @Images VALUES
    (N'Astronomy', N'/images/hero/ravenclaw-common-room.png', N'/images/programs/astronomy.png'),
    (N'Charms', N'/images/hero/ravenclaw-eagle-wall.png', N'/images/programs/charms.png'),
    (N'Potions', N'/images/hero/great-hall.png', N'/images/programs/potions.png'),
    (N'Defence Against the Dark Arts', N'/images/hero/ravenclaw-eagle-wall.png', N'/images/programs/defence-against-the-dark-arts.png');

IF EXISTS (
    SELECT expected.SubjectName
    FROM @Images AS expected
    LEFT JOIN dbo.ProgramsTable AS program WITH (UPDLOCK, HOLDLOCK)
        ON LTRIM(RTRIM(program.SubjectName)) COLLATE Latin1_General_100_CI_AS = expected.SubjectName
    GROUP BY expected.SubjectName
    HAVING COUNT(program.SubjectId) <> 1
)
    THROW 50001, 'Expected exactly one existing program for each approved image. No images were changed.', 1;

IF EXISTS (
    SELECT 1 FROM dbo.ProgramsTable AS program
    JOIN @Images AS expected
        ON LTRIM(RTRIM(program.SubjectName)) COLLATE Latin1_General_100_CI_AS = expected.SubjectName
    WHERE program.ImagePath IS NULL OR program.ImagePath NOT IN (expected.OldPath, expected.NewPath)
)
    THROW 50002, 'A current image differs from the documented placeholder. Review it before replacing. No images were changed.', 1;

UPDATE program SET ImagePath = expected.NewPath
FROM dbo.ProgramsTable AS program
JOIN @Images AS expected
    ON LTRIM(RTRIM(program.SubjectName)) COLLATE Latin1_General_100_CI_AS = expected.SubjectName
WHERE program.ImagePath <> expected.NewPath;

COMMIT TRANSACTION;

SELECT program.SubjectId, program.SubjectName, program.ImagePath
FROM dbo.ProgramsTable AS program
JOIN @Images AS expected
    ON LTRIM(RTRIM(program.SubjectName)) COLLATE Latin1_General_100_CI_AS = expected.SubjectName
ORDER BY program.SubjectId;
