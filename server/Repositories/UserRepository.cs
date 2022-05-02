
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using server.Model;

namespace server.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> collection;
        public UserRepository(IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("MongoDBConnection");
            collection = new MongoClient(connString)
                .GetDatabase("pet_hotels").GetCollection<User>("users");
        }
        public User Insert(User user)
        {
            var existingUser = GetByEmail(user.Email);
            if (existingUser != null)
            {
                throw new Exception("User with same email already exists");
            }

            user.Id = Guid.NewGuid();
            collection.InsertOne(user);
            return user;
        }
        public User ChangePassword(User user)
        {
            var filter = Builders<User>.Filter.Eq("email", user.Email);
            var update = Builders<User>.Update.Set("password", user.Password);

            collection.UpdateOne(filter, update);
            return user;
        }
        public User ChangeEmail(User user, String newEmail)
        {
            var filter = Builders<User>.Filter.Eq("email", user.Email);
            var update = Builders<User>.Update.Set("email", newEmail);

            collection.UpdateOne(filter, update);
            return user;
        }
        public IReadOnlyCollection<User> GetAll()
        {
            return collection.Find(x => true).ToList();
        }
        public User GetById(Guid id)
        {
            return collection.Find(x => x.Id == id).FirstOrDefault();
        }
        public User GetByEmail(string email)
        {
            return collection.Find(x => x.Email == email)
           .FirstOrDefault();
        }
        public User GetByEmailAndPassword(string email, string password)
        {
            return collection.Find(x => x.Email == email &&
            x.Password == password).FirstOrDefault();
        }
        public void Delete(Guid employeeId)
        {
            collection.DeleteOne((x) => x.Id == employeeId);
        }
        public async void CreateIndexes()
        {
            await collection.Indexes.CreateOneAsync(new
           CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(_ => _.Id)))
            .ConfigureAwait(false);
            await collection.Indexes.CreateOneAsync(new
           CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(_ => _.Email)))
            .ConfigureAwait(false);
        }

    }
}
