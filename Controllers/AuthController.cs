using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwipeMate.Application.Common.Models;
using SwipeMate.Application.Features.Auth;
using SwipeMate.Application.Features.Auth.Commands.ForgotPassword;
using SwipeMate.Application.Features.Auth.Commands.Login;
using SwipeMate.Application.Features.Auth.Commands.RefreshToken;
using SwipeMate.Application.Features.Auth.Commands.Register;

namespace SwipeMate.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]

public class AuthController: ControllerBase
{
    private readonly RegisterCommandHandler _registerHandler;
    private readonly LoginCommandHandler _loginHandler;
    private readonly RefreshTokenCommandHandler _refreshHandler;
    private readonly ForgotPasswordCommandHandler _forgotPasswordHandler;

    public AuthController(
        RegisterCommandHandler registerHandler,
        LoginCommandHandler loginHandler,
        RefreshTokenCommandHandler refreshHandler,
        ForgotPasswordCommandHandler forgotPasswordHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
        _refreshHandler = refreshHandler;
        _forgotPasswordHandler = forgotPasswordHandler;
    }


    // POST api/v1/auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResult>> Register(
        [FromBody] RegisterCommand command,
        CancellationToken ct)
    {
        // RegisterCommand -> AuthResult
        var result = await _registerHandler.Handle(command, ct);
        return Ok(result);
    }

    // POST api/v1/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResult>> Login(
        [FromBody] LoginCommand command,
        CancellationToken ct)
    {
        // LoginCommand -> AuthResult
        var result = await _loginHandler.Handle(command, ct);
        return Ok(result);
    }

    // POST api/v1/auth/refresh
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResult>> Refresh(
        [FromBody] RefreshTokenCommand command,
        CancellationToken ct)
    {
        // RefreshTokenCommand -> AuthResult
        var result = await _refreshHandler.Handle(command, ct);
        return Ok(result);
    }

    // POST api/v1/auth/forgot-password
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result>> ForgotPassword(
        [FromBody] ForgotPasswordCommand command,
        CancellationToken ct)
    {
        var result = await _forgotPasswordHandler.Handle(command, ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
