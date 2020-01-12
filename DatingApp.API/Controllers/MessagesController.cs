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
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {


        public IDatingRepository _repo { get; }
        public IMapper _mapper { get; }
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {

            _repo = repo;
            _mapper = mapper;
        }


        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var messageFromRepo = await _repo.GetMessage(id);
            if (messageFromRepo == null)
                return NotFound();
            return Ok(messageFromRepo);
        }
        [HttpGet]
        public async Task<IActionResult> GetMessageForUser(int userid, [FromQuery]MessageParams messageParams)
        {
            if (userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            messageParams.UserID = userid;
            var MessageFromRepo = await _repo.GetMessageForUser(messageParams);
            var message=_mapper.Map<IEnumerable<MessageToReturnDto>>(MessageFromRepo);
            Response.AddPagination(MessageFromRepo.CurrentPage,MessageFromRepo.PageSize,
                            MessageFromRepo.TotalCount,MessageFromRepo.TotalPages);
            return Ok(message);
        }


        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDto.SenderId = userId;
            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);

            if (recipient == null)
                return BadRequest("Could not find user ");
            var message = _mapper.Map<Message>(messageForCreationDto);

            _repo.add(message);

            var MessageToReturn = _mapper.Map<MessageForCreationDto>(message);
            if (await _repo.SaveAll())
            {
                return CreatedAtRoute("GetMessage", new { id = message.Id }, MessageToReturn);
            }

            throw new Exception("Creating the message failed on save");
        }
    }
}