using BasicPlatform.AppService.Positions.Models;

namespace BasicPlatform.AppService.Positions.Responses;

/// <summary>
/// 读取职位分页列表响应类
/// </summary>
[DataPermission(typeof(Position), "职位管理模块")]
public class GetPositionPagingResponse : PositionQueryModel
{
    /// <summary>
    /// 组织/部门
    /// </summary>
    [TableColumn(Width = 180, Sort = 0)]
    public string? OrganizationName { get; set; }
}