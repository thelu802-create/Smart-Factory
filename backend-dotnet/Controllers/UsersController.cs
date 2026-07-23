using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Models.Requests;
using SmartFactory.Api.Repositories;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("users")]
public sealed class UsersController(UsersRepository users) : ControllerBase
{
    [HttpGet]
    public IActionResult GetUsers() => Ok(users.GetUsers());

    [HttpGet("roles")]
    public IActionResult GetRoles() => Ok(users.GetRoles());

    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest? request)
    {
        if (!users.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "User actions require the SmartFactory database." });
        }

        var result = users.Create(request?.FullName, request?.Email, request?.RoleId, request?.Department);
        return result.Status == "ok"
            ? Created($"/users/{result.User!.Id}", result.User)
            : BadRequest(new { detail = result.Error });
    }

    [HttpDelete("{userId}")]
    public IActionResult DeleteUser(string userId)
    {
        if (!users.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "User actions require the SmartFactory database." });
        }

        return users.Delete(userId) switch
        {
            "not_found" => NotFound(new { detail = "User not found" }),
            "in_use" => Conflict(new { detail = "Cannot delete: the user still has related records (forms, notifications, alerts...)." }),
            _ => NoContent()
        };
    }
}
