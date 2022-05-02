

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
    public class ActivityController : Controller
    {
        private readonly ActivityRepository activityRepository;
        private readonly AnimalRepository animalRepository;
        public ActivityController(ActivityRepository activityRepository, AnimalRepository animalRepository)
        {
            this.activityRepository = activityRepository;
            this.animalRepository = animalRepository;
        }
        [Authorize]
        [HttpPost]
        [Route("add")]
        public IActionResult Add([FromBody] ActivityApiModel model)
        {
            var dbNote = activityRepository.Insert(new Activity()
            {
                ActiveHour = model.ActiveHour,
                SleepHour = model.SleepHour,
                LastUpdate = DateTime.Now,
                AnimalId = model.AnimalId,
            });
            return Ok(dbNote);
        }
        [Authorize]
        [HttpGet]
        [Route("statistic")]
        public IActionResult Statistic([FromBody] String id)
        {
            var animal = animalRepository.GetById(new Guid(id));
            var activities = activityRepository.GetByAnimalId(animal.Id);
            return Ok(activities);
        }
        [Authorize]
        [HttpGet]
        [Route("getByAnimalId")]
        public IActionResult GetByAnimalId([FromBody] String id)
        {
            var animal = animalRepository.GetById(new Guid(id));
            var activities = activityRepository.GetByAnimalId(animal.Id);
            return Ok(activities);
        }
        [Authorize(Roles = "client")]
        [HttpGet]
        [Route("getCheck")]
        public IActionResult GetChech([FromBody] String id)
        {
            var animal = animalRepository.GetById(new Guid(id));
            var activities = activityRepository.GetByAnimalId(animal.Id);
            return Ok(activities.Count * 300);
        }
        [Authorize]
        [HttpGet]
        [Route("get")]
        public IActionResult Get([FromBody] String Id)
        {
            var activities = activityRepository.GetById(new Guid(Id));
            return Ok(activities);
        }
        [Authorize]
        [HttpPut]
        [Route("edit/{id}")]
        public IActionResult Edit(String id,[FromBody] ActivityApiModel model)
        {
            var guid = new Guid(id);
            var resActivity = activityRepository.GetById(guid);
            if (resActivity == null)
                return BadRequest(new { Error = "Activity isn't exist" });

            var response = activityRepository.Edit(new Activity()
            {
                ActiveHour = model.ActiveHour,
                SleepHour = model.SleepHour,
                LastUpdate = DateTime.Now,
                Id = guid,
                AnimalId = model.AnimalId,
            });
            return Ok(model);
        }

        [Authorize]
        [HttpDelete]
        [Route("delete")]
        public IActionResult Delete([FromBody] String activityId)
        {
            var guid = new Guid(activityId);
            var note = activityRepository.GetById(guid);
            if (note == null)
                return BadRequest(new { Error = "Activity isn't exist" });

            activityRepository.Delete(guid);
            return Ok("Activity deleted success");
        }
    }
}