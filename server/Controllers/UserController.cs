
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Auth;
using server.Extensions;
using server.Model;
using server.Backup;
using server.Repositories;
using System.Threading.Tasks;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserRepository userRepository;
        private readonly ClientRepository clientRepository;
        private readonly EmployeeRepository employeeRepository;
        private readonly MongoBackup backup;
        public UserController(UserRepository userRepository, 
            ClientRepository clientRepository,
            EmployeeRepository employeeRepository,
            MongoBackup backup)
        {
            this.userRepository = userRepository;
            this.clientRepository = clientRepository;
            this.employeeRepository = employeeRepository;
            this.backup = backup; 
        }
        [HttpPost]
        [Route("register")]
        public IActionResult Register(UserApiModel model)
        {
            const string CLIENT = "client";
            const string EMPLOYEE = "employee";

            var existing = userRepository.GetByEmail(model.Email);
            if (existing != null)
                return BadRequest(new
                {
                    Error = "User already exist"
                });
            else if (!model.Email.Contains('@'))
                return BadRequest(new
                {
                    Error = "Not valid email"
                });
            var dbUser = userRepository.Insert(new User()
            {
                Email = model.Email,
                Password = model.Password,
                Role = model.Role
            });
           if (model.Role == CLIENT)
            {
                Client dbClient = clientRepository.Insert(new Client()
                {
                    UserId = dbUser.Id,
                    Name = model.Name,
                    Surname = model.Surname,
                    Phone = model.Phone
                });
               
            }
            else if (model.Role == EMPLOYEE)
            {
                Employee dbClient = employeeRepository.Insert(new Employee()
                {
                    UserId = dbUser.Id,              
                    Name = model.Name,
                    Surname = model.Surname,
                    Phone = model.Phone     
                });

            }
            else return BadRequest(new {Error = "Not valid role"});

            return Ok(new UserResponseModel
            {
                Id = dbUser.Id,
                Email = dbUser.Email,
                Role = dbUser.Role
            });
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("getAll")]
        public IActionResult GetAll()
        {
            var users = userRepository.GetAll()
            .Select(x => new UserResponseModel
            {
                Id = x.Id,
                Email = x.Email,
                Role = x.Role
            });
            return Ok(users);
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("{id}")]
        public IActionResult get(string id)
        {
            var user = userRepository.GetById(new Guid(id));
            return Ok(new UserResponseModel
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            });
        }
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginApiModel model)
        {
            var user = userRepository.GetByEmailAndPassword(model.Email, model.Password);
            if (user == null)
                return BadRequest(new
                {
                    Error = "Not valid credits"
                });
            var identity = GetIdentity(user.Email, user.Role);
            var token = JWTTokenizer.GetEncodedJWT(identity, AuthOptions.Lifetime);
            return new JsonResult(new
            {
                JWT = token
            });
        }
        [Authorize]
        [HttpPost]
        [Route("changePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordModel model)
        {
            var user = userRepository.GetByEmail(this.HttpContext.GetEmailFromToken());
            if (user == null)
                return BadRequest(new { Error = "Not valid credits" });
            else if (user.Password != model.OldPassword)
                return BadRequest(new { Error = "Could not change password. Old Password wrong" });

            userRepository.ChangePassword(new User()
            {
                Email = user.Email,
                Password = model.NewPassword
            });
            return Ok("Password changed success");
        }
        [Authorize]
        [HttpPost]
        [Route("changeEmail")]
        public IActionResult ChangeEmail([FromBody] String email)
        {
            var user = userRepository.GetByEmail(this.HttpContext.GetEmailFromToken());
            if (user == null)
                return BadRequest(new { Error = "Not valid credits" });

            userRepository.ChangeEmail(user, email);
            return Ok("Email changed success");
        }
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("delete")]
        public IActionResult Delete([FromBody] String userId)
        {
            var guid = new Guid(userId);
            var user = userRepository.GetById(guid);
            if (user == null) return BadRequest(new { Error = "User isn't exist" });

            userRepository.Delete(guid);
            return Ok("User deleted success");
        }
        [Authorize]
        [HttpGet]
        [Route("backup")]
        public async Task<IActionResult> Backup()
        {
            await backup.BackupDatabase();
            return Ok("Backup success");
        }
        [Authorize]
        [HttpGet]
        [Route("restore")]
        public async Task<IActionResult> Restore()
        {
            await backup.RestoreDatabase();
            return Ok("Restore success");
        }
        private ClaimsIdentity GetIdentity(string login, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
            };
            ClaimsIdentity claimsIdentity = new
            ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
             ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

    }
}
