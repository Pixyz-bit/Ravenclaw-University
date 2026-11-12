-- Optional sample CONTENT only. Schema.sql remains the sole schema definition.
-- Run only when sample programs are wanted. Existing matching subjects are preserved.
USE [Ravenclaw University Db];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;

INSERT INTO dbo.ProgramsTable (SubjectName, Description, ImagePath)
SELECT sample.SubjectName, sample.Description, sample.ImagePath
FROM (VALUES
    (N'Astronomy', N'Sample program: Explore the night sky through star charts, planetary observation, and celestial navigation. Practice careful observation and learn to interpret the patterns above the university.', N'/images/hero/ravenclaw-common-room.png'),
    (N'Charms', N'Sample program: Study the principles of spellwork, precise wand movements, and clear incantations. Develop concentration through practical exercises and thoughtful problem solving.', N'/images/hero/ravenclaw-eagle-wall.png'),
    (N'Transfiguration', N'Sample program: Examine the theory and practice of changing the form of objects. Build accuracy, discipline, and an understanding of the limits of magical transformation.', N'/images/hero/portrait-stairwell.png'),
    (N'Potions', N'Sample program: Discover how magical ingredients interact through measured preparation, careful brewing, and observation. Learn to document methods and evaluate results.', N'/images/hero/great-hall.png'),
    (N'Herbology', N'Sample program: Learn to identify, cultivate, and care for magical plants. Explore their properties and uses while developing responsible handling and research skills.', N'/images/hero/ravenclaw-common-room.png'),
    (N'Defence Against the Dark Arts', N'Sample program: Explore protective magic, the recognition of magical threats, and practical defensive techniques. Develop sound judgement and confidence through guided exercises.', N'/images/hero/ravenclaw-eagle-wall.png')
) AS sample(SubjectName, Description, ImagePath)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.ProgramsTable AS existing WITH (UPDLOCK, HOLDLOCK)
    WHERE LTRIM(RTRIM(existing.SubjectName)) COLLATE Latin1_General_100_CI_AS = sample.SubjectName
);

SELECT @@ROWCOUNT AS ProgramsAdded;
COMMIT TRANSACTION;
SELECT SubjectId, SubjectName, ImagePath FROM dbo.ProgramsTable ORDER BY SubjectId;
GO
