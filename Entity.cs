using LinqToDB.Mapping;
// ReSharper disable StringLiteralTypo

namespace SecretSantaBot
{
    [Table(Name = "EntityBaseSecretSanta")]
    public class Entity
    {
        [Column(Name = "id"), PrimaryKey, Identity]
        public int ID { get; set; }

        [Column(Name = "chatid"), NotNull]
        public long Chat_Id { get; set; }

        [Column(Name = "userid"), NotNull]
        public long User_Id { get; set; }

        [Column(Name = "firstname")]
        public string First_Name { get; set; }

        [Column(Name = "lastname")]
        public string Last_Name { get; set; }

        [Column(Name = "wish")]
        public string Wish { get; set; }

        [Column(Name = "isactive")]
        public bool IsActive { get; set; }

        [Column(Name = "santaforuserid")]
        public long SantaForUserId { get; set; }

        [Column(Name = "giftedforuserid")]
        public long GiftedForUserId { get; set; }

        //public Entity SantaFor { get; set; } = null; // ссылка тому кому дарит
        //public Entity Gifted { get; set; } = null; // ссылка на того кто дарит этому
    }
}