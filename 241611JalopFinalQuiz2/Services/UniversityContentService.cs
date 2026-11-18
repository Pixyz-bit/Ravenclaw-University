using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Services;
//Packages data from the database, returns to HomeController.cs.
public class UniversityContentService(UniversityDbContext database)
{
    public async Task<UniversityPageViewModel> ReadAsync(CancellationToken cancellationToken)
    {
        return new UniversityPageViewModel
        {
            University = await database.AcademyInfoTables.AsNoTracking().OrderBy(x => x.AcademyInfoId).FirstOrDefaultAsync(cancellationToken),
            Contact = await database.ContactsTables.AsNoTracking().OrderBy(x => x.ContactId).FirstOrDefaultAsync(cancellationToken),
            Values = await database.CoreValuesTables.AsNoTracking().OrderBy(x => x.CoreValueId).ToListAsync(cancellationToken),
            Programs = await database.ProgramsTables.AsNoTracking().OrderBy(x => x.SubjectId).ToListAsync(cancellationToken), //Get the programs, ordered by their IDs, and give me a C# list.
            Founder = await database.FounderTables.AsNoTracking().SingleOrDefaultAsync(cancellationToken)
        };
    }
}
