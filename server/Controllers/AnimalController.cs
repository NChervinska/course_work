

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
    public class AnimalController : Controller
    {
        private readonly AnimalRepository animalRepository;
        private readonly ClientRepository clientRepository;
        private readonly EmployeeRepository employeeRepository;
        private readonly UserRepository userRepository;
        public AnimalController(AnimalRepository animalRepository, 
            ClientRepository clientRepository,
            EmployeeRepository employeeRepository,
            UserRepository userRepository)
        {
            this.animalRepository = animalRepository;
            this.clientRepository = clientRepository;
            this.employeeRepository = employeeRepository;
            this.userRepository = userRepository;
        }
        [Authorize]
        [HttpPost]
        [Route("add")]
        public IActionResult Add([FromBody] AnimalApiModel model)
        {   
            var dbAnimal = animalRepository.Insert(new Animal()
            {
                Name = model.Name,
                Type = model.Type,
                Weight = model.Weight,
                Age = model.Age,
                ClientId = model.ClientId,
                EmployeeId = model.EmployeeId
            });
            return Ok(dbAnimal);
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("statistic")]
        public IActionResult GetAll()
        {
            var animals = animalRepository.GetAll();
            return Ok(animals);
        }
        [Authorize(Roles = "client")]
        [HttpGet]
        [Route("getByClientId")]
        public IActionResult GetByClientId()
        {
            var user = userRepository.GetByEmail(this.HttpContext.GetEmailFromToken());
            var client = clientRepository.GetByUserId(user.Id);
            var animals = animalRepository.GetByClientId(client.Id);
            return Ok(animals);
        }
        [Authorize(Roles = "employee")]
        [HttpGet]
        [Route("getByEmployeeId")]
        public IActionResult GetByEmployeeId()
        {
            var user = userRepository.GetByEmail(this.HttpContext.GetEmailFromToken());
            var employee = employeeRepository.GetByUserId(user.Id);
            var animals = animalRepository.GetByEmployeeId(employee.Id);
            return Ok(animals);
        }
        [Authorize]
        [HttpGet]
        [Route("get/{id}")]
        public IActionResult Get(String id)
        {
            var animals = animalRepository.GetById(new Guid(id));
            return Ok(animals);
        }
        [Authorize]
        [HttpGet]
        [Route("care/{id}")]
        public IActionResult CareRecommendations(String id)
        {
            var animals = animalRepository.GetById(new Guid(id));
            if(animals.Type == "Dog")
            {
                return Ok("https://www.royalcanin.com/ua/dogs/thinking-of-getting-a-dog/how-to-care-for-a-dog");
            }else if(animals.Type == "Cat")
            {
                return Ok("https://www.royalcanin.com/ua/cats/thinking-of-getting-a-cat/how-to-care-for-a-kitten-or-cat");
            }else if(animals.Type == "Parrot")
            {
                return Ok("https://kultura.poltava.ua/yak-pravilno-doglyadati-za-papugami/");
            }else if(animals.Type == "Humster")
            {
                return Ok("https://vn.20minut.ua/Podii/instruktsiya-yak-doglyadati-za-homyachkom-10244476.html#:~:text=%D0%9D%D0%B5%20%D0%B4%D0%B0%D0%B2%D0%B0%D0%B9%D1%82%D0%B5%20%D1%85%D0%BE%D0%BC'%D1%8F%D0%BA%D0%BE%D0%B2%D1%96%20%D0%BD%D1%96%D1%87%D0%BE%D0%B3%D0%BE,%D0%BF%D0%BE%D1%82%D1%80%D1%96%D0%B1%D0%BD%D0%BE%20%D0%B2%D1%96%D0%BD%20%D0%BD%D0%B5%20%D0%B7'%D1%97%D1%81%D1%82%D1%8C.");
            }else if(animals.Type == "Turtle")
            {
                return Ok("http://poradum.com/poradi-dlya-domu/kimnatni-roslyny/yak-doglyadati-za-cherepaxoyu-v-domashnix-umovax-vazhlivi-znannya.html");
            }
            return BadRequest("This animals is not in the database");
        }
        [Authorize]
        [HttpGet]
        [Route("feed/{id}")]
        public IActionResult FeedingRecommendations(String id)
        {
            var animals = animalRepository.GetById(new Guid(id));
            if (animals.Type == "Dog")
            {
                return Ok("https://www.royalcanin.com/ua/dogs/thinking-of-getting-a-dog/how-to-care-for-a-dog");
            }
            else if (animals.Type == "Cat")
            {
                return Ok("https://www.royalcanin.com/ua/cats/thinking-of-getting-a-cat/how-to-care-for-a-kitten-or-cat");
            }
            else if (animals.Type == "Parrot")
            {
                return Ok("https://uk.observatoriodepaliativos.org/Papageien-f-ttern-172");
            }
            else if (animals.Type == "Humster")
            {
                return Ok("https://zoocomplex.com.ua/ua-chem-kormit-homyaka/");
            }
            else if (animals.Type == "Turtle")
            {
                return Ok("https://kultura.poltava.ua/yak-chasto-goduvati-krasnouxuyu-cherepaxu/");
            }
            return BadRequest("This animals is not in the database");
        }
        [Authorize]
        [HttpPut]
        [Route("edit/{id}")]
        public IActionResult Edit(String id, [FromBody] AnimalApiModel model)
        {
            var guid = new Guid(id);
            var resAnimal = animalRepository.GetById(guid);
            if (resAnimal == null)
                return BadRequest(new { Error = "Animal isn't exist" });

            var response = animalRepository.Edit(new Animal()
            {
                Name = resAnimal.Name,
                Type = resAnimal.Type,
                Id = guid,
                Weight = resAnimal.Weight,
                Age = resAnimal.Age,
                ClientId = resAnimal.ClientId,
                EmployeeId = resAnimal.EmployeeId,
            });
            return Ok(response);
        }

        [Authorize]
        [HttpDelete]
        [Route("delete")]
        public IActionResult Delete([FromBody] String animalId)
        {
            var guid = new Guid(animalId);
            var note = animalRepository.GetById(guid);
            if (note == null)
                return BadRequest(new { Error = "Animal isn't exist" });

            animalRepository.Delete(guid);
            return Ok("Animal deleted success");
        }
    }
}