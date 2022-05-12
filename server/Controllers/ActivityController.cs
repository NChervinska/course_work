

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
            if(model.SleepHour + model.ActiveHour > 24)
                return BadRequest(new { Error = "Not valid activity" });
            var now = DateTime.Now;
            var date = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            var activity = activityRepository.GetByDate(date);
            if(activity != null) return BadRequest(new { Error = "you have already added the activity" });
            var dbNote = activityRepository.Insert(new Activity()
            {
                ActiveHour = model.ActiveHour,
                SleepHour = model.SleepHour,
                LastUpdate = date,
                AnimalId = model.AnimalId,
            });
            return Ok(dbNote);
        }
        [Authorize]
        [HttpGet]
        [Route("statistic/{id}")]
        public IActionResult Statistic(String id)
        {
            var animal = animalRepository.GetById(new Guid(id));
            var activities = activityRepository.GetByAnimalId(animal.Id);
            return Ok(activities);
        }
        [Authorize]
        [HttpGet]
        [Route("getByAnimalId/{id}")]
        public IActionResult GetByAnimalId( String id)
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
            return Ok(new 
            {
                SleepHour = activities.SleepHour,
                ActiveHour = activities.ActiveHour,
                Animal = animalRepository.GetById(activities.AnimalId),
                Id = activities.Id
            });
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
        [Route("delete/{id}")]
        public IActionResult Delete(String id)
        {
            var guid = new Guid(id);
            var note = activityRepository.GetById(guid);
            if (note == null)
                return BadRequest(new { Error = "Activity isn't exist" });

            activityRepository.Delete(guid);
            return Ok("Activity deleted success");
        }
    }
}