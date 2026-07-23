using Microsoft.EntityFrameworkCore;
using SmartFactory.Api.Data;
using SmartFactory.Api.Models;
using SmartFactory.Api.Models.Database;

namespace SmartFactory.Api.Repositories;

/// <summary>
/// EF Core access for the user-management module: list users (with role name),
/// list roles, create a user, and delete a user (blocked when the user is still
/// referenced by other records).
/// </summary>
public sealed class UsersRepository(SmartFactoryDbContext context)
{
    public sealed record UserResult(string Status, string? Error, UserDto? User);

    public bool IsAvailable() => context.Database.CanConnect();

    public IReadOnlyList<UserDto> GetUsers()
    {
        return (from user in context.Users
                join role in context.Roles on user.RoleId equals role.Id
                orderby user.CreatedAt
                select new UserDto(user.Id, user.FullName, user.Email, role.Name, user.Department, user.Status, DateValue(user.CreatedAt)))
            .ToList();
    }

    public IReadOnlyList<RoleDto> GetRoles() =>
        context.Roles.OrderBy(role => role.Name).Select(role => new RoleDto(role.Id, role.Name)).ToList();

    public UserResult Create(string? fullName, string? email, string? roleId, string? department)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return new UserResult("invalid", "Full name is required.", null);
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return new UserResult("invalid", "Email is required.", null);
        }

        if (string.IsNullOrWhiteSpace(roleId) || !context.Roles.Any(role => role.Id == roleId))
        {
            return new UserResult("invalid", "Invalid role.", null);
        }

        if (context.Users.Any(user => user.Email == email))
        {
            return new UserResult("invalid", "Email already exists.", null);
        }

        var id = "user-" + Guid.NewGuid().ToString("N")[..8];
        context.Users.Add(new UserEntity
        {
            Id = id,
            FullName = fullName!.Trim(),
            Email = email!.Trim(),
            RoleId = roleId!,
            Department = string.IsNullOrWhiteSpace(department) ? "Operations" : department!.Trim(),
            Status = "Active",
            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")
        });
        context.SaveChanges();
        return new UserResult("ok", null, GetUser(id));
    }

    /// <summary>Deletes a user. Returns "ok", "not_found", or "in_use" (referenced elsewhere).</summary>
    public string Delete(string id)
    {
        var user = context.Users.FirstOrDefault(item => item.Id == id);
        if (user is null)
        {
            return "not_found";
        }

        try
        {
            context.Users.Remove(user);
            context.SaveChanges();
            return "ok";
        }
        catch (DbUpdateException)
        {
            // Foreign-key restrict: the user is referenced by forms/notifications/etc.
            return "in_use";
        }
    }

    private UserDto? GetUser(string id) =>
        (from user in context.Users
         join role in context.Roles on user.RoleId equals role.Id
         where user.Id == id
         select new UserDto(user.Id, user.FullName, user.Email, role.Name, user.Department, user.Status, DateValue(user.CreatedAt)))
        .FirstOrDefault();

    // Users show the calendar date (yyyy-MM-dd) rather than a time-of-day.
    private static string DateValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Length >= 10 ? value.Substring(0, 10) : value;
    }
}
