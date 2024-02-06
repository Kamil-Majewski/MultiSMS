using AutoMapper;
using MultiSMS.BusinessLogic.DTO;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class ImportResultService : IImportResultService
    {
        private readonly IImportResultRepository _importRepository;
        private readonly IMapper _mapper;

        public ImportResultService(IImportResultRepository importRepository, IMapper mapper)
        {
            _importRepository = importRepository;
            _mapper = mapper;
        }

        public async Task<ImportResultDTO> GetImportResultDtoByIdAsync(int id)
        {
            var importResult = await _importRepository.GetByIdAsync(id);
            return _mapper.Map<ImportResultDTO>(importResult);
        }

        public async Task<ImportResult> AddEntityToDatabaseAsync(ImportResult entity)
        {
            return await _importRepository.AddEntityToDatabaseAsync(entity);
        }
    }
}
