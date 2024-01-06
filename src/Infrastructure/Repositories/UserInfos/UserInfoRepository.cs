﻿using Defender.Common.Configuration.Options;
using Defender.Common.DB.Model;
using Defender.Common.DB.Pagination;
using Defender.Common.DB.Repositories;
using Defender.Common.Errors;
using Defender.Common.Exceptions;
using Defender.UserManagementService.Application.Common.Interfaces.Repositories;
using Defender.UserManagementService.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Defender.UserManagementService.Infrastructure.Repositories.UserInfos;

public class UserInfoRepository : BaseMongoRepository<UserInfo>, IUserInfoRepository
{
    public UserInfoRepository(IOptions<MongoDbOptions> mongoOption) : base(mongoOption.Value)
    {
    }

    public async Task<UserInfo> CreateUserInfoAsync(UserInfo user)
    {
        return await AddItemAsync(user);
    }

    public async Task<UserInfo> UpdateUserInfoAsync(UpdateModelRequest<UserInfo> request)
    {
        return await UpdateItemAsync(request);
    }

    public async Task<IList<UserInfo>> GetUserInfosByAllFieldsAsync(UserInfo account)
    {
        var paginationSettings = PaginationSettings<UserInfo>.DefaultRequest();

        var findRequest = FindModelRequest<UserInfo>.Init(a => a.Email, account.Email)
                                                    .Or(a => a.Nickname, account.Nickname);

        if (string.IsNullOrWhiteSpace(account.PhoneNumber))
        {
            findRequest.Or(a => a.PhoneNumber, account.PhoneNumber);
        }

        paginationSettings.AddFilter(findRequest);

        var pagedResult = await GetItemsAsync(paginationSettings);

        return pagedResult.Items;
    }

    public async Task<UserInfo> GetUserInfoByIdAsync(Guid userId)
    {
        return await GetItemAsync(userId);
    }

    public async Task<UserInfo> GetUserInfoByLoginAsync(string login)
    {
        var findRequest = FindModelRequest<UserInfo>.Init(a => a.Email, login)
                                                    .Or(a => a.PhoneNumber, login);

        var userInfo = await GetItemAsync(findRequest);

        if (userInfo == null)
            throw new NotFoundException(ErrorCode.BR_USM_UserWithSuchLoginIsNotExist);

        return userInfo;
    }

    public async Task<bool> CheckIfEmailTakenAsync(string email)
    {
        var findRequest = FindModelRequest<UserInfo>.Init(a => a.Email, email);

        var userInfo = await GetItemAsync(findRequest);

        return userInfo != null;
    }
}
