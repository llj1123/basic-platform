using BasicPlatform.AppService.Roles.Requests;

namespace BasicPlatform.AppService.FreeSql.Roles;

/// <summary>
/// 角色请求处理程序
/// </summary>
public class RoleRequestHandler : ServiceBase<Domain.Models.Role>,
    IRequestHandler<CreateRoleRequest, string>,
    IRequestHandler<UpdateRoleRequest, string>,
    IRequestHandler<RoleStatusChangeRequest, string>
{
    private readonly ISecurityContextAccessor _contextAccessor;

    public RoleRequestHandler(UnitOfWorkManager unitOfWorkManager, ISecurityContextAccessor contextAccessor)
        : base(unitOfWorkManager)
    {
        _contextAccessor = contextAccessor;
    }

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> Handle(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Models.Role(
            request.Name,
            request.Remarks,
            _contextAccessor.UserId
        );
        await RegisterNewAsync(entity, cancellationToken);
        return entity.Id;
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> Handle(UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var entity = await Queryable
            .Where(p => p.Id == request.Id)
            .FirstAsync(cancellationToken);
        if (entity == null)
        {
            throw FriendlyException.NotData();
        }

        entity.Update(request.Name, request.Remarks, _contextAccessor.UserId);
        await RegisterDirtyAsync(entity, cancellationToken);
        return entity.Id;
    }

    /// <summary>
    /// 状态变更
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> Handle(RoleStatusChangeRequest request, CancellationToken cancellationToken)
    {
        var entity = await Queryable
            .Where(p => p.Id == request.Id)
            .FirstAsync(cancellationToken);

        if (entity == null)
        {
            throw FriendlyException.Of("角色不存在");
        }

        entity.StatusChange(_contextAccessor.UserId);
        await RegisterDirtyAsync(entity, cancellationToken);
        return entity.Id;
    }
}