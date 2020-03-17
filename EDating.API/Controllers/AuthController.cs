using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EDating.API.Data;
using EDating.API.Dto;
using EDating.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EDating.API.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController (IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        } 
            
            

        [HttpPost ("register")]
        public async Task<IActionResult> Register ([FromBody] UserForRegisterDto userForRegisterDto) {

            //Niepotrzebne jesli posiadamy ApiController - tak samo ^ [FromBody]
            // if(!ModelState.IsValid)
            //     return BadRequest(ModelState);

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower ();
            if (await _repo.UserExists (userForRegisterDto.Username))
                return BadRequest ("Username already exists");

            var userToCreate = new User {
                Username = userForRegisterDto.Username,

            };

            var createdUser = await _repo.Register (userToCreate, userForRegisterDto.Password);

            return StatusCode (201);

        }

        [HttpPost ("login")]
        public async Task<IActionResult> Login (UserForLoginDto user) {
            var userFromRepo = await _repo.Login (user.Username.ToLower(), user.Password);

            if (userFromRepo == null)
                return Unauthorized ();

            var claims = new [] {
                new Claim (ClaimTypes.NameIdentifier, userFromRepo.Id.ToString ()),
                new Claim (ClaimTypes.Name, userFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }

    }
}