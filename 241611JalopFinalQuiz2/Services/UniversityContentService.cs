using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Services;

public class UniversityContentService(UniversityDbContext database)
{
    public async Task<UniversityPageViewModel> ReadAsync(CancellationToken cancellationToken)
    {
        // A DbContext is not thread-safe: complete each query before starting the next.
        return new UniversityPageViewModel
        {
            University = await database.AcademyInfoTables.AsNoTracking().OrderBy(x => x.AcademyInfoId).FirstOrDefaultAsync(cancellationToken),
            Contact = await database.ContactsTables.AsNoTracking().OrderBy(x => x.ContactId).FirstOrDefaultAsync(cancellationToken),
            Values = await database.CoreValuesTables.AsNoTracking().OrderBy(x => x.CoreValueId).ToListAsync(cancellationToken),
            Photos = await database.AboutImagesTables.AsNoTracking().OrderBy(x => x.AboutImageId).ToListAsync(cancellationToken),
            Programs = await database.ProgramsTables.AsNoTracking().OrderBy(x => x.SubjectId).ToListAsync(cancellationToken),
            Founders = await database.FounderTables.AsNoTracking().OrderBy(x => x.FounderId).ToListAsync(cancellationToken),
            SocialLinks = await database.SocialLinksTables.AsNoTracking().OrderBy(x => x.SocialLinkId).ToListAsync(cancellationToken)
        };
    }
}
