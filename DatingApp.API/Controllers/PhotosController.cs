using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/Photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _CloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> CloudinaryConfig)
        {
            this._CloudinaryConfig = CloudinaryConfig;
            this._mapper = mapper;
            this._repo = repo;
            Account acc = new Account(_CloudinaryConfig.Value.CloudName, _CloudinaryConfig.Value.ApiKey, _CloudinaryConfig.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
        }
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            var photoToReturn = _mapper.Map<PhotoToReturnDto>(photoFromRepo);
            return Ok(photoToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
            var userFromRepo = await _repo.GetUser(userId);

            var file = photoForCreationDto.File;
            var UploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    UploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoForCreationDto.Url = UploadResult.Uri.ToString();
            photoForCreationDto.PublicId = UploadResult.PublicId;
            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if (!userFromRepo.Photos.Any(u => u.IsMain)) { photo.IsMain = true; }
            userFromRepo.Photos.Add(photo);
            if (await _repo.SaveAll())
            {
                var PhotoToReturnDto = _mapper.Map<PhotoToReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.ID }, PhotoToReturnDto);
            }
            return BadRequest("Could not add Photo");
        }
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
            var userFromRep = await _repo.GetUser(userId);
            if (!userFromRep.Photos.Any(p => p.ID == id)) return Unauthorized();
            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo.IsMain) return BadRequest("this is already your main photo");

            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;
            if (await _repo.SaveAll()) return NoContent();
            return BadRequest("Could not set Photo To main");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
            var userFromRep = await _repo.GetUser(userId);
            if (!userFromRep.Photos.Any(p => p.ID == id)) return Unauthorized();
            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo.IsMain) return BadRequest("you cannot delete your main photo");
           
            if (photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);
                if (result.Result == "ok")
                {
                    _repo.Delete(photoFromRepo);
                }
            }
            if (photoFromRepo.PublicId == null)
            {
                _repo.Delete(photoFromRepo);
            }

            if (await _repo.SaveAll())
            {
                return Ok();
            }
            return BadRequest("Could not delete thee photo");
        }
    }
}