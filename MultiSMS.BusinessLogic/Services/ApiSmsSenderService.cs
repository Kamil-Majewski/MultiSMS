using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Helpers;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface;
using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services
{
    public class ApiSmsSenderService : IApiSmsSenderService
    {
        private readonly LocalDbContext _localContext;

        public ApiSmsSenderService(LocalDbContext localContext)
        {
            _localContext = localContext;
        }

        public async Task<IEnumerable<ApiSmsSender>> GetAllSenders()
        {
            return await _localContext.ApiSmsSenders.ToListAsync();
        }

        public async Task<ApiSmsSender> GetSenderById(int senderId)
        {
            return await _localContext.ApiSmsSenders.FindAsync(senderId) ?? throw new InvalidOperationException($"Could not find entity ApiSmsSender with given Id");
        }

        public async Task<ApiSmsSender> CreateNewSenderAsync(ApiSmsSender sender)
        {
            ValidationHelper.ValidateObject(sender, nameof(sender));

            await _localContext.ApiSmsSenders.AddAsync(sender);
            await _localContext.SaveChangesAsync();

            return sender;
        }

        public async Task<ApiSmsSender> UpdateSenderAsync(ApiSmsSender sender)
        {
            ValidationHelper.ValidateObject(sender, nameof(sender));

            _localContext.Entry(sender).State = EntityState.Modified;
            await _localContext.SaveChangesAsync();
            return sender;
        }

        public async Task DeleteSenderAsync(int senderId)
        {
            ValidationHelper.ValidateId(senderId, nameof(senderId));

            ApiSmsSender sender = await _localContext.ApiSmsSenders.FindAsync(senderId) ?? throw new InvalidOperationException($"Could not find entity ApiSmsSender with given Id");
            _localContext.ApiSmsSenders.Remove(sender);
            await _localContext.SaveChangesAsync();
        }

        public async Task AssignUserToSender(int userId, int senderId)
        {
            ValidationHelper.ValidateId(userId, nameof(userId));
            ValidationHelper.ValidateId(senderId, nameof(senderId));

            ApiSmsSender sender = await GetSenderById(senderId);

            if (!sender.AssingedUserIds.Contains(userId))
            {
                sender.AssingedUserIds.Add(userId);
                await UpdateSenderAsync(sender);
            }
        }

        public async Task UnassignUserFromSender(int userId, int senderId)
        {
            ValidationHelper.ValidateId(userId, nameof(userId));
            ValidationHelper.ValidateId(senderId, nameof(senderId));

            ApiSmsSender sender = await GetSenderById(senderId);

            if (sender.AssingedUserIds.Contains(userId))
            {
                sender.AssingedUserIds.Remove(userId);
                await UpdateSenderAsync(sender);
            }
        }

        public async Task<ApiSmsSender> GetSenderByUserId(int userId)
        {
            ValidationHelper.ValidateId(userId, nameof(userId));

            return await _localContext.ApiSmsSenders.SingleOrDefaultAsync(s => s.AssingedUserIds.Contains(userId))
                                                                        ?? throw new InvalidOperationException($"No sender assigned to User Id {userId}");
        }
    }
}
