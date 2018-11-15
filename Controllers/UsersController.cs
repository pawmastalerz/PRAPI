using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using PRAPI.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using PRAPI.Services;
using PRAPI.Dtos;
using PRAPI.Models;
 
namespace PRAPI.Controllers
{
    // [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService userService;
        private IMapper mapper;
        private readonly AppSettings appSettings;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            this.userService = userService;
            this.mapper = mapper;
            this.appSettings = appSettings.Value;
        }
 
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody]UserDto userDto)
        {
            var user = this.userService.Login(userDto.Username, userDto.Password);
 
            if (user == null)
                return Unauthorized();
 
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(480),
                NotBefore = DateTime.UtcNow,
                // NotBefore = DateTime.UtcNow.AddSeconds(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
 
            return Ok(new {
                Token = tokenString
            });
        }
 
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]UserDto userDto)
        {
            var user = this.mapper.Map<User>(userDto);
 
            try
            {
                this.userService.Create(user, userDto.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
 
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users =  this.userService.GetAll();
            var userDtos = this.mapper.Map<IList<UserDto>>(users);
            return Ok(userDtos);
        }
 
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user =  this.userService.GetById(id);
            var userDto = this.mapper.Map<UserDto>(user);
            return Ok(userDto);
        }
 
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]UserDto userDto)
        {
            var user = this.mapper.Map<User>(userDto);
            user.Id = id;
 
            try
            {
                this.userService.Update(user, userDto.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
 
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            this.userService.Delete(id);
            return Ok();
        }
    }
}