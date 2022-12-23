namespace BasicPlatform.Domain.Models;

/// <summary>
/// 角色组与用户关联
/// </summary>
[Table("authority_role_group_users")]
public class RoleGroupUser : ValueObject
{
    /// <summary>
    /// 角色组ID
    /// </summary>
    /// <value></value>
    [MaxLength(36)]
    public string RoleGroupId { get; set; } = null!;

    /// <summary>
    /// 角色组
    /// </summary>
    /// <value></value>
    public virtual RoleGroup RoleGroup { get; set; } = null!;

    /// <summary>
    /// 用户ID
    /// </summary>
    /// <value></value>
    [MaxLength(36)]
    public string UserId { get; set; } = null!;

    /// <summary>
    /// 用户
    /// </summary>
    /// <value></value>
    public virtual User User { get; set; } = null!;
}