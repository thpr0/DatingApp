using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository Repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = Repo;

        }
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currenUserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(currenUserID);
            userParams.UserID = currenUserID;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.gender == "male" ? "female" : "male";
            }
            var users = await _repo.GetUsers(userParams);
            var usertoreturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(usertoreturn);
        }
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailDto>(user);
            return Ok(userToReturn);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var UserFromRepo = await _repo.GetUser(id);
            _mapper.Map(userForUpdateDto, UserFromRepo);
            if (await _repo.SaveAll()) return NoContent();
            throw new Exception("Update User {id} failed on save");
        }

        [HttpPost("{id}/like/{RecipientID}")]
        public async Task<ActionResult> LikeUser(int id, int recipientID)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _repo.GetLike(id, recipientID);

            if (like != null)
                return BadRequest("You already like this user");

            if (await _repo.GetUser(recipientID) == null)
                return NotFound();

            like = new Like()
            {
                LikerId = id,
                LikeeId = recipientID
            };

            _repo.add(like);

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest("Failled to like user");
        }

    }
}