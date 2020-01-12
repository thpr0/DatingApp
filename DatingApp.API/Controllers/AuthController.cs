using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _service;
        private readonly IConfiguration _config;
        private readonly IMapper mapper;

        public AuthController(IAuthRepository service, IConfiguration config, IMapper mapper)
        {
            this.mapper = mapper;
            this._config = config;
            this._service = service;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto UserForRegisterDto)
        {
            UserForRegisterDto.Username = UserForRegisterDto.Username.ToLower();
            if (await _service.userExists(UserForRegisterDto.Username)) return BadRequest("Username already taken");

            var UserToCreate = mapper.Map<User>(UserForRegisterDto);

            var userCreated = await _service.Register(UserToCreate, UserForRegisterDto.Password);
            var userToReturn = mapper.Map<UserForDetailDto>(userCreated);
            return CreatedAtRoute("GetUser", new { Controller = "Users", id = UserToCreate.Id }, userToReturn);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserForLoginDto user)
        {



            var userFromRepo = await _service.LoginAsync(user.username, user.password);
            if (userFromRepo == null)
            {
                return Unauthorized();
            }
            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.Username)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var userToReturn = mapper.Map<UserForListDto>(userFromRepo);
            return Ok(
                new
                {
                    token = tokenHandler.WriteToken(token),
                    userToReturn
                }
            );


        }
    }
}