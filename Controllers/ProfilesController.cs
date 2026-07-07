using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SwipeMate.Application.Features.Profiles;
using SwipeMate.Application.Features.Profiles.Commands.DeletePhoto;
using SwipeMate.Application.Features.Profiles.Commands.UpdateProfile;
using SwipeMate.Application.Features.Profiles.Commands.UploadPhoto;
using SwipeMate.Application.Features.Profiles.Queries.GetMyProfile;

namespace SwipeMate.Api.Controllers;

[ApiController]
[Route("api/v1/profiles")]
[Authorize]

public class ProfilesController : ControllerBase
{
    private readonly GetMyProfileQueryHandler _getMyProfile;
    private readonly UpdateProfileCommandHandler _updateProfile;
    private readonly UploadPhotoCommandHandler _uploadPhoto;
    private readonly DeletePhotoCommandHandler _deletePhoto;

    public ProfilesController(
        GetMyProfileQueryHandler getMyProfile,
        UpdateProfileCommandHandler updateProfile,
        UploadPhotoCommandHandler uploadPhoto,
        DeletePhotoCommandHandler deletePhoto)
    {
        _getMyProfile = getMyProfile;
        _updateProfile = updateProfile;
        _uploadPhoto = uploadPhoto;
        _deletePhoto = deletePhoto;
    }

    // GET /api/v1/profiles/me
    [HttpGet]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProfileDto>> GetMyProfile(CancellationToken ct)
    {
        var query = new GetMyProfileQuery(); // userId comes from JWT in handler
        var profile = await _getMyProfile.Handle(query, ct);
        return Ok(profile);
    }

    // PUT /api/v1/profiles/me
    [HttpPut]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProfileDto>> UpdateProfile(
        [FromBody] UpdateProfileCommand command,
        CancellationToken ct)
    {
        var profile = await _updateProfile.Handle(command, ct);
        return Ok(profile);
    }

    // POST /api/v1/profiles/me/photos
    // multipart/form-data: file, sortOrder, isPrimary
    [HttpPost("photos")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProfilePhotoDto), StatusCodes.Status201Created)]
    [RequestSizeLimit(10_485_760)] // 10 MB example
    public async Task<ActionResult<ProfilePhotoDto>> UploadPhoto(
        [FromForm] IFormFile file,
        [FromForm] int sortOrder,
        [FromForm] bool isPrimary,
        CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        var command = new UploadPhotoCommand(
            stream,
            file.FileName,
            file.ContentType,
            sortOrder,
            isPrimary);

        var photo = await _uploadPhoto.Handle(command, ct);
        return CreatedAtAction(nameof(GetMyProfile), null, photo);
    }

    // DELETE /api/v1/profiles/me/photos/{photoId}
    [HttpDelete("photos/{photoId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePhoto(
        [FromRoute] Guid photoId,
        CancellationToken ct)
    {
        var command = new DeletePhotoCommand(photoId);
        var result = await _deletePhoto.Handle(command, ct);

        return result.IsSuccess ? NoContent() : NotFound();
    }
}
