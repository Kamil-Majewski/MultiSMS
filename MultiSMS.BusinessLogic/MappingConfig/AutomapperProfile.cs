using AutoMapper;
using MultiSMS.BusinessLogic.DTO;
using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.MappingConfig
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Administrator, AdministratorDTO>();
        }
    }
}
