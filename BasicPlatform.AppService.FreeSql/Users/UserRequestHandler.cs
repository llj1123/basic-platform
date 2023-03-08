using BasicPlatform.AppService.Users.Requests;

namespace BasicPlatform.AppService.FreeSql.Users;

/// <summary>
/// 用户请求处理程序
/// </summary>
public class UserRequestHandler : AppServiceBase<User>,
    IRequestHandler<CreateUserRequest, string>,
    IRequestHandler<UpdateUserRequest, string>,
    IRequestHandler<UserStatusChangeRequest, string>,
    IRequestHandler<AssignUserResourcesRequest, string>,
    IRequestHandler<UpdateUserLoginInfoRequest, bool>,
    IRequestHandler<AddUserAccessRecordRequest, long>
{
    private readonly ISecurityContextAccessor _contextAccessor;

    public UserRequestHandler(UnitOfWorkManager unitOfWorkManager, ISecurityContextAccessor contextAccessor)
        : base(unitOfWorkManager, contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var exists = await QueryableNoTracking
            .Where(p => p.UserName == request.UserName)
            .AnyAsync(cancellationToken);
        if (exists)
        {
            throw FriendlyException.Of("用户名已存在");
        }

        var entity = new User(
            request.UserName,
            request.Password,
            request.RealName,
            request.PhoneNumber,
            request.Email,
            UserId
        );
        await RegisterNewAsync(entity, cancellationToken);
        // 新增关联数据
        if (request.OrganizationIds.Count > 0)
        {
            var organizationUsers = request
                .OrganizationIds
                .Select(orgId => new OrganizationUser(orgId, entity.Id))
                .ToList();
            await RegisterNewRangeValueObjectAsync(organizationUsers, cancellationToken);
        }

        // 新增关联数据
        if (request.RoleIds.Count > 0)
        {
            var userRoles = request
                .RoleIds
                .Select(roleId => new RoleUser(roleId, entity.Id))
                .ToList();
            await RegisterNewRangeValueObjectAsync(userRoles, cancellationToken);
        }

        return entity.Id;
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        if (request.Id == UserId)
        {
            throw FriendlyException.Of("不能修改自己的信息");
        }

        // 查询用户名是否重复
        var exists = await Queryable
            .Where(p => p.Id != request.Id)
            .AnyAsync(p => p.UserName == request.UserName, cancellationToken);
        // 已存在
        if (exists)
        {
            throw FriendlyException.Of("用户名已存在");
        }

        // 封装实体对象
        var entity = await GetForEditAsync(request.Id);
        // 更新
        entity.Update(
            request.UserName,
            request.Password,
            request.RealName,
            request.PhoneNumber,
            request.Email,
            UserId,
            IsRoot
        );

        #region 组织架构用户

        // 删除旧数据
        await RegisterDeleteValueObjectAsync<OrganizationUser>(
            p => p.UserId == entity.Id, cancellationToken
        );

        // 新增关联数据
        if (request.OrganizationIds.Count > 0)
        {
            var organizationUsers = request
                .OrganizationIds
                .Select(orgId => new OrganizationUser(orgId, entity.Id))
                .ToList();
            await RegisterNewRangeValueObjectAsync(organizationUsers, cancellationToken);
        }

        #endregion

        #region 用户角色

        // 删除旧数据
        await RegisterDeleteValueObjectAsync<RoleUser>(
            p => p.UserId == entity.Id, cancellationToken
        );
        // 新增关联数据
        if (request.RoleIds.Count > 0)
        {
            var userRoles = request
                .RoleIds
                .Select(roleId => new RoleUser(roleId, entity.Id))
                .ToList();
            await RegisterNewRangeValueObjectAsync(userRoles, cancellationToken);
        }

        #endregion

        // 更新
        await RegisterDirtyAsync(entity, cancellationToken);

        return entity.Id;
    }

    /// <summary>
    /// 状态变更
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> Handle(UserStatusChangeRequest request, CancellationToken cancellationToken)
    {
        if (request.Id == UserId)
        {
            throw FriendlyException.Of("不能修改自己的状态");
        }

        var entity = await GetForUpdateAsync(request.Id, cancellationToken);
        entity.StatusChange(UserId);
        await RegisterDirtyAsync(entity, cancellationToken);
        return entity.Id;
    }

    /// <summary>
    /// 分配资源
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<string> Handle(AssignUserResourcesRequest request, CancellationToken cancellationToken)
    {
        if (request.Id == UserId)
        {
            throw FriendlyException.Of("不能给自己分配资源");
        }

        // 删除旧数据
        await RegisterDeleteValueObjectAsync<UserResource>(
            p => p.UserId == request.Id, cancellationToken
        );
        if (request.Resources.Count <= 0)
        {
            return request.Id;
        }

        // 新增新数据
        var userResources = request
            .Resources
            .Select(p => new UserResource(request.Id, p.Key, p.Code, request.ExpireAt))
            .ToList();
        await RegisterNewRangeValueObjectAsync(userResources, cancellationToken);

        return request.Id;
    }

    /// <summary>
    /// 更新用户登录信息
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> Handle(UpdateUserLoginInfoRequest request, CancellationToken cancellationToken)
    {
        var entity = await GetForEditAsync(request.Id);
        entity.UpdateLoginInfo(_contextAccessor.IpAddress);
        await RegisterDirtyAsync(entity, cancellationToken);
        return true;
    }

    /// <summary>
    /// 添加用户访问记录
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<long> Handle(AddUserAccessRecordRequest request, CancellationToken cancellationToken)
    {
        // 未登录
        if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(request.AccessUrl))
        {
            return -1;
        }

        var entity = new UserAccessRecord(UserId, _contextAccessor.IpAddress, request.AccessUrl);
        await RegisterNewValueObjectAsync(entity, cancellationToken);
        return entity.Id;
    }
}