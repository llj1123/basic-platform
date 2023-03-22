namespace BasicPlatform.AppService.Roles.Models;

/// <summary>
/// 角色模型
/// </summary>
public class RoleModel : ModelBase
{
    /// <summary>
    /// 名称
    /// </summary>
    [Required]
    [MaxLength(32)]
    [TableColumn(Width = 150, Sort = 1)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 数据访问范围
    /// </summary>
    [TableColumn(Width = 150, Sort = 2)]
    public RoleDataScope DataScope { get; set; }

    /// <summary>
    /// 自定义数据访问范围(组织Id)
    /// <remarks>多个组织使用逗号分割</remarks>
    /// </summary>
    [MaxLength(-1)]
    [TableColumn(Width = 150, Sort = 1, Show = false)]
    public string? DataScopeCustom { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(1024)]
    [TableColumn(Width = 150, Sort = 3)]
    public string? Remarks { get; set; }
}