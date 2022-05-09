

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using server.Auth;
using server.Extensions;
using server.Model;
using server.Repositories;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : Controller
    {
        private readonly ClientRepository clientRepository;
        private readonly UserRepository userRepository;
        public ClientController(ClientRepository clientRepository, UserRepository userRepository)
        {
            this.clientRepository = clientRepository;
            this.userRepository = userRepository;
        }
        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(string id)
        {
            var client = clientRepository.GetById(new Guid(id));
            return Ok(client);
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("getAll")]
        public IActionResult getAllClients()
        {
            var client = clientRepository.GetAll();
            return Ok(client);
        }
        [Authorize]
        [HttpGet]
        [Route("getByUserId/{id}")]
        public IActionResult GetByUserId(String id)
        {
            var client = clientRepository.GetByUserId(new Guid(id));
            return Ok(client);
        }
        [Authorize(Roles = "client")]
        [HttpGet]
        [Route("")]
        public IActionResult GetByToken()
        {
            var resempl = userRepository.GetByEmail(this.HttpContext.GetEmailFromToken());
            var employee = clientRepository.GetByUserId(resempl.Id);
            return Ok(employee);
        }
        [Authorize(Roles = "client")]
        [HttpPut]
        [Route("edit")]
        public IActionResult Edit([FromBody] UserApiModel user)
        {
            var resclient = userRepository.GetByEmail(this.HttpContext.GetEmailFromToken());
            if (resclient == null) return BadRequest(new { Error = "Client isn't exist" });

            if(user.Email != resclient.Email)
            {
                userRepository.ChangeEmail(resclient, user.Email);
            }
            var response = clientRepository.Edit(new Client ()
            {
                Id = clientRepository.GetByUserId(resclient.Id).Id,
                Name = user.Name,
                Surname = user.Surname,
                Phone = user.Phone,
            });
            return Ok(user);
        }
        [Authorize(Roles = "client")]
        [HttpPut]
        [Route("editBonus")]
        public IActionResult EditBonus([FromBody] int Bonus)
        {
            var resclient = userRepository.GetByEmail(this.HttpContext.GetEmailFromToken());
            if (resclient == null) return BadRequest(new { Error = "Client isn't exist" });

            var response = clientRepository.EditBonus(clientRepository.GetByUserId(resclient.Id).Id, Bonus);
            return Ok(Bonus);
        }
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult Delete(String id)
        {
            var guid = new Guid(id);
            var client = clientRepository.GetById(guid);
            if (client == null) return BadRequest(new { Error = "Client isn't exist" });

            clientRepository.Delete(guid);
            userRepository.Delete(client.UserId);
            return Ok("Client deleted success");
        }
    }
}