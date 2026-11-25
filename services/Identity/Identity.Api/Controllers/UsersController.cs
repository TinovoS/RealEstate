using Identity.Api.Dtos;
using Identity.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;

    public UsersController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var user = await FindCurrentUserAsync();

        if (user is null)
        {
            return CurrentUserNotFound();
        }

        return Ok(await ToProfileAsync(user));
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateProfileRequest request)
    {
        var user = await FindCurrentUserAsync();

        if (user is null)
        {
            return CurrentUserNotFound();
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        return Ok(await ToProfileAsync(user));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Korisnik ne postoji.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var response = new PublicProfileResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = roles.FirstOrDefault() ?? string.Empty,
        };

        return Ok(response);
    }

    // Prijavljeni korisnik iz baze. Id dolazi iz tokena (claim "sub"), ne iz zahteva.
    private async Task<AppUser?> FindCurrentUserAsync()
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (userId is null)
        {
            return null;
        }

        return await _userManager.FindByIdAsync(userId);
    }

    // Token je ispravan, ali korisnik u međuvremenu više ne postoji.
    private ObjectResult CurrentUserNotFound()
    {
        return Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Korisnik ne postoji.");
    }

    private async Task<UserProfileResponse> ToProfileAsync(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        return new UserProfileResponse
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = roles.FirstOrDefault() ?? string.Empty,
            CreatedAt = user.CreatedAt,
        };
    }
}
