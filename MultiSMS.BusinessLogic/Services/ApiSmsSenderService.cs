using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Helpers;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class ApiSmsSenderService : GenericService<ApiSmsSender>, IApiSmsSenderService
    {
        public ApiSmsSenderService(IGenericRepository<ApiSmsSender> apiSmsSenderRepository) : base(apiSmsSenderRepository) { }

        public async Task AssignUserToSender(int userId, int senderId)
        {
            ValidationHelper.ValidateId(userId, nameof(userId));
            ValidationHelper.ValidateId(senderId, nameof(senderId));

            ApiSmsSender sender = await GetByIdAsync(senderId);

            if (!sender.AssingedUserIds.Contains(userId))
            {
                sender.AssingedUserIds.Add(userId);
                await UpdateEntityAsync(sender);
            }
        }

        public async Task UnassignUserFromSender(int userId, int senderId)
        {
            ValidationHelper.ValidateId(userId, nameof(userId));
            ValidationHelper.ValidateId(senderId, nameof(senderId));

            ApiSmsSender sender = await GetByIdAsync(senderId);

            if (sender.AssingedUserIds.Contains(userId))
            {
                sender.AssingedUserIds.Remove(userId);
                await UpdateEntityAsync(sender);
            }
        }

        public async Task<ApiSmsSender> GetSenderByUserId(int userId)
        {
            ValidationHelper.ValidateId(userId, nameof(userId));

            ApiSmsSender sender = await GetAllEntriesQueryable()
                .Include(s => s.ApiToken)
                .FirstOrDefaultAsync(s => s.AssingedUserIds.Contains(userId))
                ?? throw new InvalidOperationException($"No sender assigned to User Id {userId}");

            return sender;
        }

        public async Task<List<ApiSmsSender>> GetSendersBySearchPhraseAsync(string searchPhrase)
        {
            ValidationHelper.ValidateString(searchPhrase, nameof(searchPhrase));

            return await GetAllEntriesQueryable().Where(s => s.Name.ToLower().Contains(searchPhrase)
            || (string.IsNullOrEmpty(s.Description) ? "Brak opisu" : s.Description).ToLower().Contains(searchPhrase)).ToListAsync();
        }

        public async Task RemoveUserFromSendersAsync(int userId)
        {
            ValidationHelper.ValidateId(userId, nameof(userId));

            var senders = await GetAllEntriesQueryable()
                .Where(s => s.AssingedUserIds.Contains(userId))
                .ToListAsync();

            if (senders.Any())
            {
                foreach (var sender in senders)
                {
                    sender.AssingedUserIds.Remove(userId);
                }

                await UpdateRangeAsync(senders);
            }
        }
    }
}
