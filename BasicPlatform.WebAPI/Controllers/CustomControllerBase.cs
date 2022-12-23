using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BasicPlatform.WebAPI.Controllers;

/// <summary>
/// 控制器基类
///     所有需要登录后才能操作的接口都需要继承此类
/// </summary>
[EnableCors]
[Route("api/[controller]/[action]")]
[ApiController]
// [Authorize]
// [PermissionAuthorizeFilter]
public class CustomControllerBase : ControllerBase
{
}