using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Helpers;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class ApiSmsSenderService : GenericService<ApiSmsSender>, IApiSmsSenderService
    {
        private readonly IGenericRepository<ApiSmsSenderUser> _senderUserRepository;

        public ApiSmsSenderService(IGenericRepository<ApiSmsSender> apiSmsSenderRepository,
                                   IGenericRepository<ApiSmsSenderUser> senderUserRepository) : base(apiSmsSenderRepository)
        {
            _senderUserRepository = senderUserRepository;
        }

        public async Task<bool> AssignUserToSender(int userId, int senderId)
        {
            ValidationHelper.ValidateId(userId, nameof(userId));
            ValidationHelper.ValidateId(senderId, nameof(senderId));

            var existingAssignment = await _senderUserRepository.GetAllEntries()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (existingAssignment != null)
            {
                throw new InvalidOperationException("User is already assigned to another sender.");
            }

            var newAssignment = new ApiSmsSenderUser
            {
                ApiSmsSenderId = senderId,
                UserId = userId
            };

            await _senderUserRepository.AddEntityToDatabaseAsync(newAssignment);
            return true;
        }

        public async Task<bool> UnassignUserFromSender(int userId)
        {
            ValidationHelper.ValidateId(userId, nameof(userId));

            var assignment = await _senderUserRepository.GetAllEntries()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (assignment == null)
            {
                return false;
            }

            await _senderUserRepository.DeleteEntityAsync(assignment);
            return true;
        }

        public async Task<ApiSmsSender> GetSenderByUserId(int userId)
        {
            ValidationHelper.ValidateId(userId, nameof(userId));

            var senderUser = await _senderUserRepository.GetAllEntries()
                .FirstOrDefaultAsync(su => su.UserId == userId) ?? throw new ArgumentNullException("User is not assigned to any senders");

            return await GetAllEntriesQueryable().Include(s => s.ApiToken)
                                                 .FirstOrDefaultAsync(s => s.Id == senderUser.ApiSmsSenderId)
                                                 ?? throw new ArgumentNullException($"Could not find sender by provided id {senderUser.ApiSmsSenderId}");
        }

        public async Task<List<ApiSmsSender>> GetSendersBySearchPhraseAsync(string searchPhrase)
        {
            ValidationHelper.ValidateString(searchPhrase, nameof(searchPhrase));

            return await GetAllEntriesQueryable().Where(s => s.Name.ToLower().Contains(searchPhrase)
            || (string.IsNullOrEmpty(s.Description) ? "Brak opisu" : s.Description).ToLower().Contains(searchPhrase)).ToListAsync();
        }
    }
}
