using AutoMapper;
using MultiSMS.BusinessLogic.DTO;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class AdministratorService : IAdministratorService
    {
        private readonly IAdministratorRepository _adminRepository;
        private readonly IMapper _mapper;

        public AdministratorService(IAdministratorRepository adminRepository, IMapper mapper)
        {
            _adminRepository = adminRepository;
            _mapper = mapper;
        }

        public async Task<AdministratorDTO> GetAdministratorDtoByEmailAsync(string email)
        {
            var admin = await _adminRepository.GetAdministratorByEmailAsync(email);
            return _mapper.Map<AdministratorDTO>(admin);
        }

        public async Task<AdministratorDTO> GetAdministratorDtoByIdAsync(int id)
        {
            var admin = await _adminRepository.GetAdinistratorByIdAsync(id);
            return _mapper.Map<AdministratorDTO>(admin);
        }


    }
}
