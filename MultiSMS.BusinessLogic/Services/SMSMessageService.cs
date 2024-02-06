using AutoMapper;
using MultiSMS.BusinessLogic.DTO;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class SMSMessageService : ISMSMessageService
    {
        private readonly ISMSMessageRepository _smsMessageRepository;
        private readonly IMapper _mapper;

        public SMSMessageService(ISMSMessageRepository sMSMessageRepository, IMapper mapper)
        {
            _smsMessageRepository = sMSMessageRepository;
            _mapper = mapper;
        }

        public async Task<SmsMessageDTO> GetSmsMessageDtoById(int id)
        {
            var smsMessage = await _smsMessageRepository.GetByIdAsync(id);
            return _mapper.Map<SmsMessageDTO>(smsMessage);
        }
    }
}
