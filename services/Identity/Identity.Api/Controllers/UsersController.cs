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
        // Id prijavljenog korisnika dolazi iz tokena (claim "sub"), ne iz zahteva.
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (userId is null)
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId);

        // Token je ispravan, ali korisnik u međuvremenu više ne postoji.
        if (user is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Korisnik ne postoji.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var response = new UserProfileResponse
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = roles.FirstOrDefault() ?? string.Empty,
            CreatedAt = user.CreatedAt,
        };

        return Ok(response);
    }
}
