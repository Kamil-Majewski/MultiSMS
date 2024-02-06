using AutoMapper;
using MultiSMS.BusinessLogic.DTO;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Entities.ServerSms;
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
                       : JsonConvert.DeserializeObject<List<Employee>>(src.AddedEmployeesSerialized)))
                .ForMember(dest => dest.RepeatedEmployees, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.RepeatedEmployeesSerialized)
                       ? null
                       : JsonConvert.DeserializeObject<List<Employee>>(src.RepeatedEmployeesSerialized)))
                .ForMember(dest => dest.InvalidEmployees, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.InvalidEmployeesSerialized)
                       ? null
                       : JsonConvert.DeserializeObject<List<Employee>>(src.InvalidEmployeesSerialized)))
                .ForMember(dest => dest.NonExistantGroupIds, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.NonExistantGroupIdsSerialized)
                       ? null
                       : JsonConvert.DeserializeObject<List<List<string>>>(src.NonExistantGroupIdsSerialized)));

            CreateMap<SMSMessage, SmsMessageDTO>()
                .ForMember(dest => dest.DataDictionary, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<Dictionary<string, string>>(src.DataDictionarySerialized)))
                .ForMember(dest => dest.ServerResponse, opt => opt.MapFrom(src => DeserializeServerResponse(src)));

        }

        private object DeserializeServerResponse(SMSMessage src)
        {
            try
            {
                return JsonConvert.DeserializeObject<ServerSmsSuccessResponse>(src.ServerResponseSerialized)!;
            }
            catch (JsonSerializationException)
            {
                return JsonConvert.DeserializeObject<ServerSmsErrorResponse>(src.ServerResponseSerialized)!;
            }
        }
    }
}
