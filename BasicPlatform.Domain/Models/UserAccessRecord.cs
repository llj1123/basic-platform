namespace BasicPlatform.Domain.Models;

/// <summary>
/// 用户访问记录
/// </summary>
[Table("authority_user_access_records")]
public class UserAccessRecord : ValueObject
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [MaxLength(36)]
    public string UserId { get; set; } = null!;

    /// <summary>
    /// 用户
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// 访问时间
    /// </summary>
    public DateTime AccessTime { get; set; }

    /// <summary>
    /// 访问IP
    /// </summary>
    [MaxLength(50)]
    public string AccessIp { get; set; } = null!;

    /// <summary>
    /// 访问物理地址
    /// </summary>
    [MaxLength(200)]
    public string? AccessPhysicalAddress { get; set; }

    /// <summary>
    /// 访问地址
    /// </summary>
    [MaxLength(200)]
    public string AccessUrl { get; set; } = null!;

    public UserAccessRecord()
    {
    }

    /// <summary>
    /// 添加用户访问记录
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="accessIp"></param>
    /// <param name="accessUrl"></param>
    public UserAccessRecord(string userId, string accessIp, string accessUrl)
    {
        UserId = userId;
        AccessTime = DateTime.Now;
        AccessIp = accessIp;
        AccessPhysicalAddress = NewLife.IP.Ip.GetAddress(accessIp);
        AccessUrl = accessUrl;
    }
}