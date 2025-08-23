using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrekifyBackend.Models
{
    public class Trek
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("Sr No.")]
        public int SerialNumber { get; set; }

        [BsonElement("State")]
        public string State { get; set; } = string.Empty;

        [BsonElement("Trek Name")]
        public string TrekName { get; set; } = string.Empty;

        [BsonElement("Trek Type")]
        public string TrekType { get; set; } = string.Empty;

        [BsonElement("Difficulty Level")]
        public string DifficultyLevel { get; set; } = string.Empty;

        [BsonElement("Season")]
        public string Season { get; set; } = string.Empty;

        [BsonElement("Duration")]
        public string Duration { get; set; } = string.Empty;

        [BsonElement("Distance")]
        public string Distance { get; set; } = string.Empty;

        [BsonElement("Max Altitude")]
        public string MaxAltitude { get; set; } = string.Empty;

        [BsonElement("Trek Description")]
        public string TrekDescription { get; set; } = string.Empty;

        [BsonElement("Image")]
        public string Image { get; set; } = string.Empty;
    }
}
