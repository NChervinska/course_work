using MongoDB.Bson.Serialization.Attributes;
using System;

namespace server.Model
{
    public class AnimalApiModel
    {
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("type")]
        public string Type { get; set; }
        [BsonElement("weight")]
        public int Weight { get; set; }
        [BsonElement("age")]
        public int Age { get; set; }
        [BsonElement("clientId")]
        public Guid ClientId { get; set; }
        [BsonElement("employeeId")]
        public Guid EmployeeId { get; set; }
     
    }
}
