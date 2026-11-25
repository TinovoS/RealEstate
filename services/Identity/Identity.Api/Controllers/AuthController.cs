using Identity.Api.Auth;
using Identity.Api.Data;
using Identity.Api.Dtos;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _db;
    private readonly TokenService _tokenService;

    public AuthController(UserManager<AppUser> userManager, AppDbContext db, TokenService tokenService)
    {
        _userManager = userManager;
        _db = db;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    [ProducesResponseType<RegisterResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (!Roles.All.Contains(request.Role))
        {
            ModelState.AddModelError(nameof(request.Role), $"Uloga mora biti jedna od: {string.Join(", ", Roles.All)}.");
            return ValidationProblem(ModelState);
        }

        if (await _userManager.FindByEmailAsync(request.Email) is not null)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Korisnik sa ovim mejlom već postoji.");
        }

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };

        // Korisnik i njegova uloga se upisuju zajedno: ili oba, ili nijedno.
        await using var transaction = await _db.Database.BeginTransactionAsync();

        var createResult = await _userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, request.Role);

        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException($"Uloga '{request.Role}' nije dodeljena korisniku.");
        }

        await transaction.CommitAsync();

        var response = new RegisterResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = request.Role,
        };

        return Created($"/api/users/{user.Id}", response);
    }

    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        // Isti odgovor za nepostojeći mejl i pogrešnu lozinku, da se ne otkrije koji mejlovi postoje.
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Pogrešan mejl ili lozinka.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(_tokenService.CreateToken(user, roles));
    }
}
