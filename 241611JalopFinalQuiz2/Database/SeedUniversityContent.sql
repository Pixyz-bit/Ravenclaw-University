-- User-approved CONTENT only; Schema.sql remains the sole schema definition.
-- Replaces About, Mission, Vision, and the complete core-value list.
-- Preserves the existing university record ID and hero image. Run only deliberately.
USE [Ravenclaw University Db];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;

IF (SELECT COUNT(*) FROM dbo.AcademyInfoTable WITH (UPDLOCK, HOLDLOCK)) > 1
    THROW 50001, 'Expected at most one university-information record; no content was changed.', 1;

DECLARE @About NVARCHAR(MAX) = N'Ravenclaw University is a place for those who ask questions, seek understanding, and imagine new possibilities. Inspired by Ravenclaw House, we celebrate learning as a lifelong pursuit—one shaped by thoughtful discussion, independent discovery, and the courage to think differently.'
    + NCHAR(10) + NCHAR(10)
    + N'Here, every perspective has something to contribute, and every question can open a new path.';
DECLARE @Mission NVARCHAR(MAX) = N'To nurture curious, independent thinkers through meaningful learning, creative exploration, and open dialogue. We encourage students to question assumptions, share their discoveries, and use knowledge with integrity and purpose.';
DECLARE @Vision NVARCHAR(MAX) = N'To build a community where curiosity becomes discovery and knowledge grows into wisdom. We envision graduates who approach unfamiliar challenges with imagination, respect different perspectives, and continue learning throughout their lives.';

IF NOT EXISTS (SELECT 1 FROM dbo.AcademyInfoTable)
    INSERT dbo.AcademyInfoTable (AcademyDescription, Mission, Vision)
    VALUES (@About, @Mission, @Vision);
ELSE
    UPDATE dbo.AcademyInfoTable
    SET AcademyDescription = @About, Mission = @Mission, Vision = @Vision;

-- Skip replacement when the exact four titles already exist in the desired order.
DECLARE @Expected TABLE (Position INT NOT NULL, Title NVARCHAR(100) NOT NULL);
INSERT @Expected (Position, Title) VALUES
    (1, N'Intelligence'),
    (2, N'Wisdom'),
    (3, N'Creativity'),
    (4, N'A Lifelong Love of Learning');

IF (SELECT COUNT(*) FROM dbo.CoreValuesTable WITH (UPDLOCK, HOLDLOCK)) <> 4
    OR EXISTS (
        SELECT Position, Title FROM @Expected
        EXCEPT
        SELECT CAST(ROW_NUMBER() OVER (ORDER BY CoreValueId) AS INT), Title
        FROM dbo.CoreValuesTable
    )
BEGIN
    DELETE FROM dbo.CoreValuesTable;
    INSERT dbo.CoreValuesTable (Title)
    SELECT Title FROM @Expected ORDER BY Position;
END;

COMMIT TRANSACTION;

SELECT AcademyInfoId,
    CASE WHEN AcademyDescription = @About AND Mission = @Mission AND Vision = @Vision
        THEN N'Approved content saved' ELSE N'Content differs' END AS ContentStatus
FROM dbo.AcademyInfoTable;
SELECT CoreValueId, Title FROM dbo.CoreValuesTable ORDER BY CoreValueId;
