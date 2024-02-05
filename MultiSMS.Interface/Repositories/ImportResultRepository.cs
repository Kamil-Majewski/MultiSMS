using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class ImportResultRepository : GenericRepository<ImportResult>, IImportResultRepository
    {
        public ImportResultRepository(MultiSMSDbContext context) : base(context)
        {
        }
    }
}