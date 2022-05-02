
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using server.Model;

namespace server.Repositories
{
    public class AnimalRepository
    {
        private readonly IMongoCollection<Animal> collection;
        public AnimalRepository(IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("MongoDBConnection");
            collection = new MongoClient(connString)
                .GetDatabase("pet_hotels").GetCollection<Animal>("animals");
        }
        public Animal Insert(Animal animal)
        {
            animal.Id = Guid.NewGuid();
            collection.InsertOne(animal);
            return animal;
        }
        public Animal Edit(Animal animal)
        {
            var filter = Builders<Animal>.Filter.Eq("Id", animal.Id);
            var update = Builders<Animal>.Update.Set("name", animal.Name)
                .Set("type", animal.Type).Set("weight", animal.Weight)
                .Set("age", animal.Age).Set("clientId", animal.ClientId)
                .Set("employeeId", animal.EmployeeId);

            collection.UpdateOne(filter, update);
            return animal;
        }
        public Animal GetById(Guid id)
        {
            return collection.Find(x => x.Id == id).FirstOrDefault();
        }
        public IReadOnlyCollection<Animal> GetAll()
        {
            return collection.Find(x => true).ToList();
        }
        public IReadOnlyCollection<Animal> GetByClientId(Guid clientId)
        {
            return collection.Find(x => x.ClientId == clientId).ToList();
        }
        public IReadOnlyCollection<Animal> GetByEmployeeId(Guid employeeId)
        {
            return collection.Find(x => x.EmployeeId == employeeId).ToList();
        }
      
        public void Delete(Guid noteId)
        {
            collection.DeleteOne((x) => x.Id == noteId);
        }
      
    }
}
