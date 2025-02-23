using BasicPlatform.Domain.Models.Roles;
using BasicPlatform.Domain.Models.Roles.Events;
using BasicPlatform.Domain.Models.Users.Events;

namespace BasicPlatform.AppService.FreeSql;

/// <summary>
/// 缓存通知处理器
/// </summary>
public class CacheNotificationHandler :
    IDomainEventHandler<UserUpdatedEvent>,
    IDomainEventHandler<RoleDataPermissionAssignedEvent>,
    IDomainEventHandler<UserDataPermissionAssignedEvent>
{
    private readonly ICacheManager _cacheManager;
    private readonly IFreeSql _freeSql;

    public CacheNotificationHandler(ICacheManager cacheManager, IFreeSql freeSql)
    {
        _cacheManager = cacheManager;
        _freeSql = freeSql;
    }

    /// <summary>
    /// 用户更新成功事件
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="NotImplementedException"></exception>
    public async Task Handle(UserUpdatedEvent notification, CancellationToken cancellationToken)
    {
        // 匹配用户缓存
        var patternKey = string.Format(CacheConstant.UserCacheKeys, notification.GetId());
        // 移除用户所有缓存
        await _cacheManager.RemovePatternAsync(patternKey, cancellationToken);
    }

    /// <summary>
    /// 角色数据权限分配事件处理
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task Handle(RoleDataPermissionAssignedEvent notification, CancellationToken cancellationToken)
    {
        // 读取角色用户
        var userIdList = await _freeSql.Queryable<RoleUser>()
            .Where(p => p.RoleId == notification.GetId())
            .ToListAsync(p => p.UserId, cancellationToken);

        if (userIdList.Count == 0)
        {
            return;
        }

        // 用户ID去重
        userIdList = userIdList.Distinct().ToList();

        // 移除用户数据权限相关的缓存
        foreach (var userId in userIdList)
        {
            var patternKey2 = string.Format(CacheConstant.UserPolicyQueryPatternKey, userId);
            await _cacheManager.RemovePatternAsync(patternKey2, cancellationToken);
            var patternKey = string.Format(CacheConstant.UserPolicyFilterGroupQueryPatternKey, userId);
            await _cacheManager.RemovePatternAsync(patternKey, cancellationToken);
        }
    }

    /// <summary>
    /// 用户数据权限分配事件处理
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task Handle(UserDataPermissionAssignedEvent notification, CancellationToken cancellationToken)
    {
        var userId = notification.GetId();
        var patternKey2 = string.Format(CacheConstant.UserPolicyQueryPatternKey, userId);
        await _cacheManager.RemovePatternAsync(patternKey2, cancellationToken);
        var patternKey = string.Format(CacheConstant.UserPolicyFilterGroupQueryPatternKey, userId);
        await _cacheManager.RemovePatternAsync(patternKey, cancellationToken);
    }
}