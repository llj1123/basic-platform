namespace BasicPlatform.Infrastructure.Events;

/// <summary>
/// 
/// </summary>
public class UserCreatedEvent : IntegrationEventBase
{
    /// <summary>
    /// 
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string UserName { get; set; } = null!;
}