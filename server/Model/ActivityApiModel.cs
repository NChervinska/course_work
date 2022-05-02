using MongoDB.Bson.Serialization.Attributes;
using System;

namespace server.Model
{
    public class ActivityApiModel
    {
        [BsonElement("sleepHour")]
        public int SleepHour { get; set; }
        [BsonElement("activeHour")]
        public int ActiveHour { get; set; }
        [BsonElement("animalId")]
        public Guid AnimalId { get; set; }
    }
}
