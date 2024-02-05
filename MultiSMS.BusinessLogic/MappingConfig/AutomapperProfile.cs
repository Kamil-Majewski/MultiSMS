using AutoMapper;
using MultiSMS.BusinessLogic.DTO;
using MultiSMS.Interface.Entities;
using Newtonsoft.Json;

namespace MultiSMS.BusinessLogic.MappingConfig
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Administrator, AdministratorDTO>();

            CreateMap<ImportResult, ImportResultDTO>()
                .ForMember(dest => dest.AddedEmployees, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.AddedEmployeesSerialized)
                       ? null
                       : JsonConvert.DeserializeObject<List<Employee>>(src.AddedEmployeesSerialized)));
        }
    }
}
