
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using server.Model;

namespace server.Repositories
{
    public class ActivityRepository
    {
        private readonly IMongoCollection<Activity> collection;
        public ActivityRepository(IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("MongoDBConnection");
            collection = new MongoClient(connString)
                .GetDatabase("pet_hotels").GetCollection<Activity>("activity");
        }
        public Activity Insert(Activity activity)
        {
            activity.Id = Guid.NewGuid();
            collection.InsertOne(activity);
            return activity;
        }
        public Activity Edit(Activity activity)
        {
            var filter = Builders<Activity>.Filter.Eq("Id", activity.Id);
            var update = Builders<Activity>.Update.Set("sleepHour", activity.SleepHour)
                .Set("activeHour", activity.ActiveHour).Set("animalId", activity.AnimalId);

            collection.UpdateOne(filter, update);
            return activity;
        }
        public Activity GetById(Guid id)
        {
            return collection.Find(x => x.Id == id).FirstOrDefault();
        }
        public IReadOnlyCollection<Activity> GetByAnimalId(Guid animalId)
        {
            return collection.Find(x => x.AnimalId == animalId).ToList();
        }
        public void Delete(Guid Id)
        {
            collection.DeleteOne((x) => x.Id == Id);
        }
    }
}
