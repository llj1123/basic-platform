using BasicPlatform.AppService.ExternalPages.Models;
using BasicPlatform.AppService.Users.Models;
using BasicPlatform.AppService.Users.Requests;
using BasicPlatform.AppService.Users.Responses;

namespace BasicPlatform.AppService.Users;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserQueryService
{
    /// <summary>
    /// 读取分页列表
    /// </summary>
    /// <param name="request">请求类</param>
    /// <returns></returns>
    Task<Paging<GetUserPagingResponse>> GetPagingAsync(GetUserPagingRequest request);

    /// <summary>
    /// 读取分页列表
    /// </summary>
    /// <param name="request">请求类</param>
    /// <returns></returns>
    Task<Paging<GetUserAccessRecordPagingResponse>> GetAccessRecordPagingAsync(GetCommonPagingRequest request);

    /// <summary>
    /// 读取信息
    /// </summary>
    /// <param name="id">Id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<GetUserByIdResponse> GetAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取信息
    /// </summary>
    /// <param name="userName">帐户名</param>
    /// <returns></returns>
    Task<GetUserByUserNameResponse> GetByUserNameAsync(string userName);

    /// <summary>
    /// 根据角色Id读取用户列表
    /// </summary>
    /// <param name="roleId">角色Id</param>
    /// <returns></returns>
    Task<List<string?>> GetIdsByRoleIdAsync(string roleId);

    /// <summary>
    /// 读取当前登录用户信息
    /// </summary>
    /// <returns></returns>
    Task<GetCurrentUserResponse> GetCurrentUserAsync();

    /// <summary>
    /// 读取用户数据
    /// </summary>
    /// <returns></returns>
    Task<List<SelectViewModel>> GetSelectListAsync(string? organizationId);

    /// <summary>
    /// 读取组织架构用户树形列表
    /// </summary>
    /// <returns></returns>
    Task<List<CascaderViewModel>> GetOrganizationUserTreeSelectListAsync();

    /// <summary>
    /// 读取组织架构和用户树形列表
    /// </summary>
    /// <returns></returns>
    Task<List<CascaderViewModel>> GetOrganizationAndUserTreeSelectListAsync();

    /// <summary>
    /// 读取用户资源
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<ResourceModel>> GetUserResourceAsync(string? userId);

    /// <summary>
    /// 读取当前登录用户外部页面列表
    /// </summary>
    /// <returns></returns>
    Task<IList<ExternalPageModel>> GetCurrentUserExternalPagesAsync();

    /// <summary>
    /// 读取用户拥有的资源信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    Task<GetUserResourceCodeInfoResponse> GetResourceCodeInfoAsync(string userId);

    /// <summary>
    /// 读取用户拥有的资源代码列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    Task<List<string>> GetResourceCodesAsync(string userId);

    /// <summary>
    /// 读取当前用户自定表格列列表
    /// </summary>
    /// <param name="moduleName">模块名</param>
    /// <returns></returns>
    Task<List<UserCustomColumnModel>> GetCurrentUserCustomColumnsAsync(string moduleName);
    
    /// <summary>
    /// 读取数据权限
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<List<GetUserDataPermissionsResponse>> GetDataPermissionsAsync(string id);
}