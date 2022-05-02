using MongoDB.Bson.Serialization.Attributes;
using System;

namespace server.Model
{
    public class User
    {
        [BsonId]
        public Guid Id { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }
        public string Role { get; set; }
    }

}
