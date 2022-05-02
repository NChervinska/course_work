

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
    public class EmployeeController : Controller
    {
        private readonly EmployeeRepository employeeRepository;
        private readonly UserRepository userRepository;
        public EmployeeController(EmployeeRepository employeeRepository, UserRepository userRepository)
        {
            this.employeeRepository = employeeRepository;
            this.userRepository = userRepository;
        }
        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(string id)
        {
            var employee = employeeRepository.GetById(new Guid(id));
            return Ok(employee);
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("getAll")]
        public IActionResult getAllEmployees()
        {
            var employee = employeeRepository.GetAll();
            return Ok(employee);
        }
        [Authorize]
        [HttpGet]
        [Route("getByUserId/{id}")]
        public IActionResult GetByUserId(String id)
        {
            var employee = employeeRepository.GetByUserId(new Guid(id));
            return Ok(employee);
        }

        [Authorize(Roles = "employee")]
        [HttpPut]
        [Route("edit")]
        public IActionResult Edit([FromBody] UserApiModel user)
        {
            var resempl = userRepository.GetByEmail(this.HttpContext.GetEmailFromToken());
            if (resempl == null) return BadRequest(new { Error = "Employee isn't exist" });

            if (user.Email != resempl.Email)
            {
                userRepository.ChangeEmail(resempl, user.Email);
            }
            var response = employeeRepository.Edit(new Employee()
            {
                Id = employeeRepository.GetByUserId(resempl.Id).Id,
                Name = user.Name,
                Surname = user.Surname,
                Phone = user.Phone,
            });
            return Ok(user);
        }
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("delete")]
        public IActionResult Delete([FromBody] String employeeId)
        {
            var guid = new Guid(employeeId);
            var employee = employeeRepository.GetById(guid);
            if (employee == null) return BadRequest(new { Error = "Employee isn't exist" });

            employeeRepository.Delete(guid);
            userRepository.Delete(employee.UserId);
            return Ok("Employee deleted success");
        }
    }
}