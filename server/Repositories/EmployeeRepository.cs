
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using server.Model;

namespace server.Repositories
{
    public class EmployeeRepository
    {
        private readonly IMongoCollection<Employee> collection;
        public EmployeeRepository(IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("MongoDBConnection");
            collection = new MongoClient(connString)
                .GetDatabase("pet_hotels").GetCollection<Employee>("employees");
        }
        public Employee Insert(Employee employee)
        {
            employee.Id = Guid.NewGuid();
            collection.InsertOne(employee);
            return employee;
        }
        public Employee Edit(Employee employee)
        {
            var filter = Builders<Employee>.Filter.Eq("Id", employee.Id);
            var update = Builders<Employee>.Update.Set("Name", employee.Name)
                .Set("Surname", employee.Surname)
                .Set("Phone", employee.Phone);

            collection.UpdateOne(filter, update);
            return employee;
        }
        public Employee GetById(Guid id)
        {
            return collection.Find(x => x.Id == id).FirstOrDefault();
        }
        public Employee GetByUserId(Guid id)
        {
            return collection.Find(x => x.UserId == id).FirstOrDefault();
        }
        public IReadOnlyCollection<Employee> GetAll()
        {
            return collection.Find(x => true).ToList();
        }
        public void Delete(Guid employeeId)
        {
            collection.DeleteOne((x) => x.Id == employeeId);
        }
    }
}
