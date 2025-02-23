using BasicPlatform.AppService.Applications;
using BasicPlatform.AppService.ExternalPages.Models;
using BasicPlatform.AppService.Users;
using BasicPlatform.AppService.Users.Requests;
using BasicPlatform.AppService.Users.Responses;

namespace BasicPlatform.WebAPI.Controllers.System;

/// <summary>
/// 用户管理
/// </summary>
[FrontEndRouting("用户管理",
    ModuleCode = "system",
    ModuleName = "系统管理",
    ModuleRoutePath = "/system",
    ModuleSort = 1,
    RoutePath = "/system/user",
    Sort = 0,
    Description = "系统基于角色授权，每个角色对不同的功能模块具备添删改查以及自定义权限等多种权限设定"
)]
// ReSharper disable once InconsistentNaming
public class UserController : CustomControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserQueryService _queryService;
    private readonly ILogger<UserController> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="queryService"></param>
    /// <param name="loggerFactory"></param>
    public UserController(IMediator mediator, IUserQueryService queryService, ILoggerFactory loggerFactory)
    {
        _mediator = mediator;
        _queryService = queryService;
        _logger = loggerFactory.CreateLogger<UserController>();
    }

    #region 基础接口

    /// <summary>
    /// 读取数据列
    /// </summary>
    /// <param name="commonService"></param>
    /// <returns></returns>
    [HttpGet]
    [SkipApiPermissionVerification]
    [ApiPermission(IsVisible = false)]
    public Task<GetTableColumnsResponse> GetColumnsAsync(
        [FromServices] ICommonService commonService)
    {
        return commonService.GetColumnsAsync<GetUserPagingResponse>();
    }

    /// <summary>
    /// 读取列表
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiPermission(AdditionalRules = new[]
    {
        ApiPermissionConstant.OrgTreeList
    })]
    public Task<Paging<GetUserPagingResponse>> GetPagingAsync([FromBody] GetUserPagingRequest request)
    {
        return _queryService.GetPagingAsync(request);
    }

    /// <summary>
    /// 根据ID读取
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiPermission(ApiPermissionConstant.UserDetail, DisplayName = "详情")]
    public Task<GetUserByIdResponse> GetAsync([FromQuery] string id)
    {
        return _queryService.GetAsync(id);
    }

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiPermission(AdditionalRules = new[]
    {
        ApiPermissionConstant.PositionSelectList,
        ApiPermissionConstant.OrgCascaderList,
        ApiPermissionConstant.RoleSelectList
    })]
    public Task<string> PostAsync([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        return _mediator.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    [ApiPermission(AdditionalRules = new[]
    {
        ApiPermissionConstant.PositionSelectList,
        ApiPermissionConstant.OrgCascaderList,
        ApiPermissionConstant.RoleSelectList,
        ApiPermissionConstant.UserDetail
    })]
    public Task<string> PutAsync([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        return _mediator.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// 状态变更
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    public Task<string> StatusChangeAsync([FromBody] UserStatusChangeRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// 分配资源
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    public Task<string> AssignResourcesAsync([FromBody] AssignUserResourcesRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// 分配权限
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    [ApiPermission(AdditionalRules = new[]
    {
        ApiPermissionConstant.UserDataPermissions
    })]
    public Task<string> AssignDataPermissionsAsync([FromBody] AssignUserDataPermissionsRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// 更新表格列表信息
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiPermission(IsVisible = false)]
    [AllowAnonymous]
    public Task<long> UpdateUserCustomColumnsAsync([FromBody] UpdateUserCustomColumnsRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>新密码</returns>
    [HttpPost]
    public Task<string> ResetPasswordAsync([FromBody] ResetUserPasswordRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.SendAsync(request, cancellationToken);
    }

    #endregion

    #region 扩展接口

    /// <summary>
    /// 下拉列表
    /// </summary>
    /// <param name="organizationId">组织Id</param>
    /// <returns></returns>
    [HttpGet]
    // [ApiPermission(ApiPermissionConstant.UserSelectList, IsVisible = false)]
    [SkipApiPermissionVerification]
    public Task<List<SelectViewModel>> GetSelectListAsync(string? organizationId = null)
    {
        return _queryService.GetSelectListAsync(organizationId);
    }

    /// <summary>
    /// 读取应用资源
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiPermission(IsVisible = false)]
    [AllowAnonymous]
    public async Task<IList<ApplicationResourceInfo>> GetApplicationResourcesAsync(
        [FromServices] IApiPermissionService service,
        [FromServices] ISecurityContextAccessor accessor,
        [FromServices] IConfiguration configuration,
        [FromServices] IApplicationQueryService applicationQueryService
    )
    {
        var result = new List<ApplicationResourceInfo>();

        #region 系统资源

        var assemblies = new List<Assembly>
        {
            Assembly.GetExecutingAssembly()
        };
        var resourceAssemblies = configuration.GetSection("ResourceAssemblies").Get<List<string>>();
        if (resourceAssemblies != null)
        {
            assemblies.AddRange(resourceAssemblies.Select(Assembly.Load));
        }

        var list = new List<MenuTreeInfo>();
        if (accessor.IsRoot)
        {
            foreach (var assembly in assemblies)
            {
                list.AddRange(service.GetFrontEndRoutingResources(assembly, GlobalConstant.DefaultAppId));
            }

            result.Add(new ApplicationResourceInfo
            {
                ApplicationId = GlobalConstant.DefaultAppId,
                ApplicationName = "系统应用",
                Resources = list
            });
        }
        else
        {
            var resources = await _queryService.GetUserResourceAsync(null, null);
            var keys = resources
                .Where(p => p.ApplicationId == GlobalConstant.DefaultAppId ||
                            string.IsNullOrEmpty(p.ApplicationId))
                .Select(p => p.Key)
                .ToList();

            foreach (var assembly in assemblies)
            {
                list.AddRange(service.GetPermissionFrontEndRoutingResources(assembly, keys, GlobalConstant.DefaultAppId));
            }

            result.Add(new ApplicationResourceInfo
            {
                ApplicationId = GlobalConstant.DefaultAppId,
                ApplicationName = "系统应用",
                Resources = list
            });
        }

        #endregion

        // 读取应用信息
        var apps = await applicationQueryService.GetListAsync();

        foreach (var app in apps.Where(p => !string.IsNullOrEmpty(p.MenuResourceRoute)))
        {
            var resourceUrl = $"{app.ApiUrl}{app.MenuResourceRoute}";
            try
            {
                var res = await resourceUrl.GetAsync()
                    .ReceiveJson<ApiResult<List<MenuTreeInfo>>>();

                if (res.Data != null && res.Success && res.Data.Count > 0)
                {
                    result.Add(new ApplicationResourceInfo
                    {
                        ApplicationId = app.ClientId,
                        ApplicationName = app.Name,
                        Resources = res.Data
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "加载应用资源失败，应用ID:{ClientId},资源地址:{Url}", app.ClientId, resourceUrl);
            }
        }

        return result;
    }

    /// <summary>
    /// 读取资源
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IList<MenuTreeInfo>> GetResourcesAsync(
        [FromServices] IApiPermissionService service,
        [FromServices] ISecurityContextAccessor accessor,
        [FromServices] IApplicationQueryService applicationQueryService
    )
    {
        IList<MenuTreeInfo> result = new List<MenuTreeInfo>();

        #region 系统应用

        IList<MenuTreeInfo> systemResources;
        var assembly = Assembly.GetExecutingAssembly();
        if (accessor.IsRoot)
        {
            systemResources = service.GetFrontEndRoutingResources(assembly, GlobalConstant.DefaultAppId);
        }
        else
        {
            var resources = await _queryService.GetUserResourceAsync(null, null);
            var keys = resources
                .Where(p => p.ApplicationId == GlobalConstant.DefaultAppId || string.IsNullOrEmpty(p.ApplicationId))
                .Select(p => p.Key)
                .ToList();
            systemResources = service.GetPermissionFrontEndRoutingResources(assembly, keys, GlobalConstant.DefaultAppId);
        }

        result.Add(new MenuTreeInfo
        {
            AppId = GlobalConstant.DefaultAppId,
            Code = "basic_platform",
            Description = "系统应用",
            Icon = "AppstoreOutlined",
            IsAuth = false,
            IsVisible = true,
            Name = "系统应用",
            Path = "/",
            Sort = 0,
            Children = systemResources.ToList()
        });

        #endregion

        #region 其他应用

        // 其他业务应用
        var apps = await applicationQueryService.GetListAsync();

        foreach (var app in apps.Where(p => !string.IsNullOrEmpty(p.MenuResourceRoute)))
        {
            var resourceUrl = $"{app.ApiUrl}{app.MenuResourceRoute}";
            try
            {
                var res = await resourceUrl.GetAsync()
                    .ReceiveJson<ApiResult<List<MenuTreeInfo>>>();

                if (res.Data != null && res.Success && res.Data.Count > 0)
                {
                    foreach (var item in res.Data)
                    {
                        item.Path = "/app/" + app.ClientId + item.Path;
                        foreach (var child in item.Children ?? new List<MenuTreeInfo>())
                        {
                            child.Path = "/app/" + app.ClientId + child.Path;
                        }
                    }

                    result.Add(new MenuTreeInfo
                    {
                        AppId = app.ClientId,
                        Code = app.ClientId,
                        Icon = "AppstoreOutlined",
                        IsAuth = false,
                        IsVisible = true,
                        Name = app.Name,
                        Path = "/app/" + app.ClientId,
                        Sort = 0,
                        Children = res.Data
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "加载应用资源失败，应用ID:{ClientId},资源地址:{Url}", app.ClientId, resourceUrl);
            }
        }

        #endregion

        return result;
    }

    /// <summary>
    /// 读取外部页面列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public Task<IList<ExternalPageModel>> GetExternalPagesAsync(string? userId = null)
    {
        return string.IsNullOrEmpty(userId)
            ? _queryService.GetCurrentUserExternalPagesAsync()
            : _queryService.GetUserExternalPagesAsync(userId);
    }

    /// <summary>
    /// 读取拥有的资源代码
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [ApiPermission(ApiPermissionConstant.UserResourceCodeInfo, IsVisible = false)]
    public Task<GetUserResourceCodeInfoResponse> GetResourceCodeInfoAsync([FromQuery] string id, string? appId)
    {
        return _queryService.GetResourceCodeInfoAsync(id, appId);
    }

    /// <summary>
    /// 读取数据权限
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ApiPermission(ApiPermissionConstant.RoleDataPermissions, IsVisible = false)]
    public Task<List<GetUserDataPermissionsResponse>> GetDataPermissionsAsync([FromQuery] string id)
    {
        return _queryService.GetDataPermissionsAsync(id);
    }

    #endregion
}