
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using server.Model;

namespace server.Repositories
{
    public class ClientRepository
    {
        private readonly IMongoCollection<Client> collection;
        public ClientRepository(IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("MongoDBConnection");
            collection = new MongoClient(connString)
                .GetDatabase("pet_hotels").GetCollection<Client>("clients");
        }
        public Client Insert(Client client)
        {
            client.Id = Guid.NewGuid();
            collection.InsertOne(client);
            return client;
        }
        public Client Edit(Client client)
        {
            var filter = Builders<Client>.Filter.Eq("Id", client.Id);
            var update = Builders<Client>.Update.Set("Name", client.Name)
                .Set("Surname", client.Surname)
                .Set("Phone", client.Phone);

            collection.UpdateOne(filter, update);
            return client;
        }
        public int EditBonus(Guid id, int bonus)
        {
            var filter = Builders<Client>.Filter.Eq("Id", id);
            var update = Builders<Client>.Update.Set("Bonus", bonus);

            collection.UpdateOne(filter, update);
            return bonus;
        }
        public Client GetById(Guid id)
        {
            return collection.Find(x => x.Id == id).FirstOrDefault();
        }
        public Client GetByUserId(Guid id)
        {
            return collection.Find(x => x.UserId == id).FirstOrDefault();
        }
        public IReadOnlyCollection<Client> GetAll()
        {
            return collection.Find(x => true).ToList();
        }
        public void Delete(Guid clientId)
        {
            collection.DeleteOne((x) => x.Id == clientId);
        }
    }
}
