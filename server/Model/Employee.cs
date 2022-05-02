using MongoDB.Bson.Serialization.Attributes;
using System;

namespace server.Model
{
    public class Employee
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        [BsonElement("userId")]
        public Guid UserId { get; set; }
    }
}
