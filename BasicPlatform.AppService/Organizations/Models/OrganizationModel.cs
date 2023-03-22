namespace BasicPlatform.AppService.Organizations.Models;

/// <summary>
/// 组织架构Dto
/// </summary>
public class OrganizationModel : ModelBase
{
    /// <summary>
    /// 父级Id
    /// </summary>
    [TableColumn(Show = false)]
    public string? ParentId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [TableColumn(Sort = 0, Width = 150)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 部门负责人Id
    /// </summary>
    [MaxLength(36)]
    [TableColumn(Show = false)]
    public string? LeaderId { get; set; }

    /// <summary>
    /// 路径
    /// </summary>
    [TableColumn(Show = false)]
    public string ParentPath { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    [TableColumn(Sort = 2)]
    public string? Remarks { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [TableColumn(Sort = 4)]
    public Status Status { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [TableColumn(Sort = 3)]
    public int Sort { get; set; }
}