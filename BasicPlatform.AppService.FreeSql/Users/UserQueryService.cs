using BasicPlatform.AppService.ExternalPages.Models;
using BasicPlatform.AppService.Roles;
using BasicPlatform.AppService.Roles.Models;
using BasicPlatform.AppService.Users;
using BasicPlatform.AppService.Users.Models;
using BasicPlatform.AppService.Users.Requests;
using BasicPlatform.AppService.Users.Responses;
using BasicPlatform.Domain.Models.Roles;
using BasicPlatform.Domain.Models.Tenants;
using BasicPlatform.Domain.Models.Users;

namespace BasicPlatform.AppService.FreeSql.Users;

/// <summary>
/// 用户服务接口实现类
/// </summary>
[Component(LifeStyle.Transient)]
public class UserQueryService : AppQueryServiceBase<User>, IUserQueryService
{
    private readonly IRoleQueryService _roleQueryService;

    public UserQueryService(
        FreeSqlMultiTenancy freeSql,
        ISecurityContextAccessor accessor,
        IRoleQueryService roleQueryService) : base(freeSql, accessor)
    {
        _roleQueryService = roleQueryService;
    }

    /// <summary>
    /// 读取分页列表
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Paging<GetUserPagingResponse>> GetPagingAsync(GetUserPagingRequest request)
    {
        ISelect<Organization>? organizationQuery = null;
        if (request.OrganizationId != null)
        {
            organizationQuery = QueryNoTracking<Organization>()
                .As("o")
                // 当前组织架构及下级组织架构
                .Where(p => p.ParentPath.Contains(request.OrganizationId!) || p.Id == request.OrganizationId);
        }

        ISelect<RoleUser>? userRoleQuery = null;
        if (request.RoleId != null)
        {
            userRoleQuery = QueryNoTracking<RoleUser>()
                .As("d")
                .Where(p => p.RoleId == request.RoleId);
        }

        var result = await QueryableNoTracking
            .HasWhere(request.Keyword, p =>
                p.UserName.Contains(request.Keyword!) ||
                p.RealName.Contains(request.Keyword!) ||
                p.PhoneNumber!.Contains(request.Keyword!) ||
                p.Email!.Contains(request.Keyword!)
            )
            .HasWhere(request.Status, p => request.Status!.Contains(p.Status))
            .HasWhere(request.Gender, p => request.Gender!.Contains(p.Gender))
            .HasWhere(organizationQuery, p => organizationQuery!.Any(o => o.Id == p.OrganizationId))
            .HasWhere(userRoleQuery, p => userRoleQuery!.Any(d => d.UserId == p.Id))
            .ToPagingAsync(request, p => new GetUserPagingResponse
            {
                CreatedUserName = p.CreatedUser!.RealName,
                UpdatedUserName = p.LastUpdatedUser!.RealName,
                OrganizationName = p.Organization!.Name,
                PositionName = p.Position!.Name
            });

        return result;
    }

    /// <summary>
    /// 读取访问记录分页列表
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Paging<GetUserAccessRecordPagingResponse>> GetAccessRecordPagingAsync(
        GetCommonPagingRequest request)
    {
        var result = await QueryNoTracking<UserAccessRecord>()
            .HasWhere(request.Keyword, p =>
                p.AccessUrl.Contains(request.Keyword!) ||
                p.User.RealName.Contains(request.Keyword!)
            )
            .OrderByPropertyNameIf(request.Sorter == null, "a.AccessTime", false)
            .ToPagingAsync(request, p => new GetUserAccessRecordPagingResponse
            {
                UserRealName = p.User.RealName
            });
        return result;
    }


    /// <summary>
    /// 读取信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<GetUserByIdResponse> GetAsync(string id, CancellationToken cancellationToken = default)

    {
        var result = await Queryable
            .Where(p => p.Id == id)
            .ToOneAsync(p => new GetUserByIdResponse
            {
                Password = string.Empty,
                OrganizationPath = p.Organization!.ParentPath
            }, cancellationToken);

        if (result == null)
        {
            throw FriendlyException.Of("找不到数据");
        }

        result.OrganizationPath = !string.IsNullOrEmpty(result.OrganizationPath)
            ? $"{result.OrganizationPath},{result.OrganizationId}"
            : result.OrganizationId;

        // 读取角色
        var roleIds = await Query<RoleUser>()
            .Where(p => p.UserId == id)
            .ToListAsync(p => p.RoleId, cancellationToken);

        result.RoleIds.AddRange(roleIds);

        return result;
    }

    /// <summary>
    /// 读取信息
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public async Task<GetUserByUserNameResponse> GetByUserNameAsync(string userName)
    {
        // 如果是租户环境
        if (IsTenantEnvironment)
        {
            // 判断租户是否生效或过期
            var tenant = await DefaultQueryNoTracking<Tenant>()
                .Where(p => p.Code == TenantId)
                .ToOneAsync();

            if (tenant == null)
            {
                throw FriendlyException.Of("租户不存在");
            }

            if (tenant.Status == Status.Disabled)
            {
                throw FriendlyException.Of("租户不可用，请联系管理员");
            }

            if (tenant.EffectiveTime > DateTime.Now)
            {
                throw FriendlyException.Of($"租户订阅未生效，生效日期({tenant.EffectiveTime:yyyy-MM-dd})");
            }

            var expiredTime = tenant.ExpiredTime?.AddDays(1).AddSeconds(-1);
            if (expiredTime != null && expiredTime < DateTime.Now)
            {
                throw FriendlyException.Of($"租户订阅于{expiredTime.Value:yyyy-MM-dd HH:mm:ss}过期");
            }
        }

        var result = await Query()
            .Where(p => p.UserName == userName)
            .ToOneAsync(p => new GetUserByUserNameResponse
            {
                Id = p.Id,
                UserName = p.UserName,
                RealName = p.RealName,
                Password = p.Password,
                PhoneNumber = p.PhoneNumber,
                Email = p.Email,
                Status = p.Status
            });

        if (result == null)
        {
            throw FriendlyException.Of("登录名或密码错误");
        }

        // 读取角色
        var roleIds = Query<RoleUser>()
            .As("e")
            .Where(p => p.UserId == result.Id);

        // 读取组织架构用户
        var organizationIds = await Query<UserAppointment>()
            .Where(p => p.UserId == result.Id)
            .ToListAsync(p => p.OrganizationId);
        // 组织架构Id
        var organizationIdList = new List<string>();
        // 读取下级组织架构Id列表
        foreach (var organizationId in organizationIds)
        {
            var itemIds = await Query<Organization>()
                .Where(p => p.ParentPath.Contains(organizationId))
                .ToListAsync(p => p.Id);
            if (itemIds.Count > 0)
            {
                organizationIdList.AddRange(itemIds);
            }
        }

        // 读取组织架构角色
        var roleIds1 = Query<OrganizationRole>()
            .As("b")
            .Where(p => organizationIdList.Contains(p.OrganizationId));

        // 读取角色你信息
        var roles = await QueryNoTracking<Role>()
            .Where(p =>
                roleIds.Any(e => e.RoleId == p.Id) ||
                roleIds1.Any(b => b.RoleId == p.Id)
            )
            .ToListAsync<RoleModel>();
        result.Roles.AddRange(roles);
        return result;
    }

    /// <summary>
    /// 根据角色Id读取用户列表
    /// </summary>
    /// <param name="roleId">角色Id</param>
    /// <returns></returns>
    public async Task<List<string?>> GetIdsByRoleIdAsync(string roleId)
    {
        var list = new List<string>();
        var userIds = await Query<RoleUser>()
            .Where(p => p.RoleId == roleId)
            .ToListAsync(p => p.UserId);
        list.AddRange(userIds);

        var organizationIds = Query<OrganizationRole>()
            .Where(p => p.RoleId == roleId);

        var userIds1 = await Query<UserAppointment>()
            .Where(p => organizationIds.Any(c => c.OrganizationId == p.OrganizationId))
            .ToListAsync(p => p.UserId);
        list.AddRange(userIds1);

        var result = list.GroupBy(userId => userId)
            .Select(userId => userId.FirstOrDefault())
            .ToList();

        if (result.Count == 0)
        {
            throw FriendlyException.Of("该角色下无用户信息");
        }

        return result;
    }

    /// <summary>
    /// 读取当前用户信息
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FriendlyException"></exception>
    public async Task<GetCurrentUserResponse> GetCurrentUserAsync(string? userId = null)
    {
        userId ??= UserId;
        if (string.IsNullOrEmpty(userId))
        {
            throw FriendlyException.Of("用户未登录");
        }

        var result = await QueryableNoTracking
            .Where(p => p.Id == userId)
            .ToOneAsync(p => new GetCurrentUserResponse
            {
                OrganizationName = p.Organization!.Name,
                PositionName = p.Position!.Name,
                OrganizationPath = p.Organization!.ParentPath
            });

        if (result == null)
        {
            throw FriendlyException.Of("找不到数据");
        }

        result.OrganizationPath = !string.IsNullOrEmpty(result.OrganizationPath)
            ? $"{result.OrganizationPath},{result.OrganizationId}"
            : result.OrganizationId;

        // 读取角色
        var roleIds = await Query<RoleUser>()
            .Where(p => p.UserId == userId)
            .ToListAsync(p => p.RoleId);

        result.RoleIds.AddRange(roleIds);

        // 用户拥有的资源代码
        result.ResourceCodes = await GetResourceCodesAsync(userId, null);

        return result;
    }

    /// <summary>
    /// 读取下拉选择框数据列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<SelectViewModel>> GetSelectListAsync(string? organizationId)
    {
        ISelect<Organization>? organizationQuery = null;
        if (organizationId != null)
        {
            organizationQuery = QueryNoTracking<Organization>()
                .As("o")
                // 当前组织架构及下级组织架构
                .Where(p => p.ParentPath.Contains(organizationId) || p.Id == organizationId);
        }

        var result = await Queryable
            .Where(p => p.Status == Status.Enabled)
            .HasWhere(organizationQuery, p => organizationQuery!.Any(o => o.Id == p.OrganizationId))
            .ToListAsync(t1 => new SelectViewModel
            {
                Label = t1.RealName,
                Value = t1.Id,
                Disabled = t1.Status == Status.Disabled,
                Extend = t1.UserName
            });

        return result;
    }

    /// <summary>
    /// 读取下拉选择框数据列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<SelectViewModel>> GetAllSelectListAsync()
    {
        var result = await Queryable
            .ToListAsync(t1 => new SelectViewModel
            {
                Label = t1.RealName,
                Value = t1.Id,
                Disabled = t1.Status == Status.Disabled,
                Extend = t1.UserName
            });

        return result;
    }

    /// <summary>
    /// 读取ID
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public Task<string> GetIdByUserNameAsync(string userName)
    {
        return QueryNoTracking().Where(p => p.UserName == userName).FirstAsync(p => p.Id);
    }

    /// <summary>
    /// 读取用户资源
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="appId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<List<ResourceModel>> GetUserResourceAsync(string? userId, string? appId)
    {
        userId ??= UserId;
        if (userId == null)
        {
            throw FriendlyException.Of("请传入用户Id");
        }

        // 如果是租户管理员，直接返回授权的所有资源
        if (IsTenantAdmin)
        {
            return await DefaultQueryNoTracking<TenantResource>()
                .Where(p => p.Tenant.Code == TenantId)
                .ToListAsync(p => new ResourceModel
                {
                    ApplicationId = p.ApplicationId,
                    Key = p.ResourceKey,
                    Code = p.ResourceCode
                });
        }

        var result = await GetResourceCodeInfoAsync(userId, appId);
        // 去重
        return result
            .RoleResources
            .Union(result.UserResources)
            .ToList();
    }

    /// <summary>
    /// 读取当前登录用户外部页面列表
    /// </summary>
    /// <returns></returns>
    public async Task<IList<ExternalPageModel>> GetCurrentUserExternalPagesAsync()
    {
        // 读取公共的和自己创建的
        var result = await QueryNoTracking<ExternalPage>()
            .Where(p => p.OwnerId == null || p.OwnerId == UserId)
            .ToListAsync<ExternalPageModel>();
        return result;
    }

    /// <summary>
    /// 读取用户外部页面列表
    /// </summary>
    /// <returns></returns>
    public async Task<IList<ExternalPageModel>> GetUserExternalPagesAsync(string userId)
    {
        // 读取公共的和自己创建的
        var result = await QueryNoTracking<ExternalPage>()
            .Where(p => p.OwnerId == null || p.OwnerId == userId)
            .ToListAsync<ExternalPageModel>();
        return result;
    }

    /// <summary>
    /// 读取用户资源代码
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="appId"></param>
    /// <returns></returns>
    public async Task<GetUserResourceCodeInfoResponse> GetResourceCodeInfoAsync(string userId, string? appId)
    {
        var roleResources = new List<ResourceModel>();
        // 用户拥有的角色
        var roleIds = await _roleQueryService.GetRoleIdsByUserIdAsync(userId);
        if (roleIds.Count > 0)
        {
            // 角色资源
            roleResources = await QueryNoTracking<RoleResource>()
                .HasWhere(appId, p => p.ApplicationId == appId)
                .Where(p => roleIds.Contains(p.RoleId))
                .ToListAsync(p => new ResourceModel
                {
                    ApplicationId = p.ApplicationId,
                    Key = p.ResourceKey,
                    Code = p.ResourceCode
                });
        }

        // 用户资源
        var userResources = await QueryNoTracking<UserResource>()
            .HasWhere(appId, p => p.ApplicationId == appId)
            .Where(p => p.UserId == userId)
            // 读取未过期的
            .Where(p => p.ExpireAt == null || p.ExpireAt > DateTime.Now)
            .ToListAsync(p => new ResourceModel
            {
                ApplicationId = p.ApplicationId,
                Key = p.ResourceKey,
                Code = p.ResourceCode
            });

        return new GetUserResourceCodeInfoResponse
        {
            RoleResources = roleResources,
            UserResources = userResources
        };
    }

    /// <summary>
    /// 读取用户资源代码列表
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="appId"></param>
    /// <returns></returns>
    public async Task<List<string>> GetResourceCodesAsync(string userId, string? appId)
    {
        var userResources = await GetUserResourceAsync(userId, appId);
        var list = new List<string>();
        foreach (var model in userResources)
        {
            list.AddRange(model.Codes);
        }

        return list;
    }

    /// <summary>
    /// 读取当前用户自定表格列列表
    /// </summary>
    /// <param name="moduleName">模块名</param>
    /// <returns></returns>
    public Task<List<UserCustomColumnModel>> GetCurrentUserCustomColumnsAsync(string moduleName)
    {
        return QueryNoTracking<UserCustomColumn>()
            .Where(p => p.ModuleName == moduleName)
            .Where(p => p.UserId == UserId)
            .ToListAsync<UserCustomColumnModel>();
    }

    /// <summary>
    /// 读取用户自定表格列列表
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="moduleName">模块名</param>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    public Task<List<UserCustomColumnModel>> GetUserCustomColumnsAsync(string? appId, string moduleName,
        string? userId)
    {
        userId ??= UserId;
        return QueryNoTracking<UserCustomColumn>()
            .Where(p => p.AppId == appId)
            .Where(p => p.ModuleName == moduleName)
            .Where(p => p.UserId == userId)
            .ToListAsync<UserCustomColumnModel>();
    }

    /// <summary>
    /// 读取用户数据权限
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<List<GetUserDataPermissionsResponse>> GetDataPermissionsAsync(string id)
    {
        // 读取用户的角色数据权限
        var list = await QueryNoTracking<RoleDataPermission>()
            // 读取用户的角色
            .Where(p => QueryNoTracking<RoleUser>()
                .As("c")
                .Where(c => c.UserId == id)
                .Any(c => c.RoleId == p.RoleId)
            )
            .ToListAsync(p => new GetUserDataPermissionsResponse
            {
                ApplicationId = p.ApplicationId,
                ResourceKey = p.ResourceKey,
                DataScope = p.DataScope,
                Enabled = p.Enabled,
                DataScopeCustom = p.DataScopeCustom,
                PolicyResourceKey = p.PolicyResourceKey,
                Policy = p.Policy
            });
        foreach (var item in list)
        {
            item.IsRolePermission = true;
        }

        // 读取用户的数据权限
        var userPermissionList = await QueryNoTracking<UserDataPermission>()
            .Where(p => p.UserId == id)
            // 读取未过期的
            .Where(p => p.ExpireAt == null || p.ExpireAt > DateTime.Now)
            .ToListAsync(p => new GetUserDataPermissionsResponse
            {
                ApplicationId = p.ApplicationId,
                ResourceKey = p.ResourceKey,
                DataScope = p.DataScope,
                Enabled = p.Enabled,
                DataScopeCustom = p.DataScopeCustom,
                PolicyResourceKey = p.PolicyResourceKey,
                Policy = p.Policy
            });

        // 以用户的为准，因为可对用户进行个性化设置
        foreach (var item in userPermissionList)
        {
            // 查询
            var single = list
                .FirstOrDefault(p => p.ResourceKey == item.ResourceKey);
            if (single == null)
            {
                list.Add(item);
                continue;
            }

            single.DataScope = item.DataScope;
            single.DataScopeCustom = item.DataScopeCustom;
            single.Policy = item.Policy;
        }

        // 去重
        list = list
            .GroupBy(p => p.ResourceKey)
            .Select(p => p.First())
            .ToList();

        return list;
    }

    /// <summary>
    /// 递归读取
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="list"></param>
    /// <param name="results"></param>
    /// <param name="parentId"></param>
    /// <param name="onlyUser"></param>
    private static void GetTreeChildren(IList<Organization> entities,
        IList<dynamic> list,
        ICollection<CascaderViewModel> results,
        string parentId = "",
        bool onlyUser = false)
    {
        var result = string.IsNullOrEmpty(parentId)
            ? entities.Where(p => string.IsNullOrEmpty(p.ParentId)).ToList()
            : entities.Where(p => p.ParentId == parentId).ToList();

        foreach (var item in result)
        {
            var res = new CascaderViewModel
            {
                Label = item.Name,
                Value = $"@{item.Id}",
            };
            if (entities.Any(p => p.ParentId == item.Id))
            {
                res.Children = new List<CascaderViewModel>();
                GetTreeChildren(entities, list, res.Children, item.Id, onlyUser);
            }

            if (onlyUser)
            {
                if (list.All(p => p.OrganizationId != item.Id))
                {
                    continue;
                }

                results.Add(res);
            }
            else
            {
                results.Add(res);
                if (list.All(p => p.OrganizationId != item.Id))
                {
                    continue;
                }
            }

            res.Children ??= new List<CascaderViewModel>();
            res.Children.AddRange(list
                .Where(p => p.OrganizationId == item.Id)
                .Select(p => new CascaderViewModel
                {
                    Label = p.RealName,
                    Value = p.UserId,
                }).ToList());
        }
    }
}