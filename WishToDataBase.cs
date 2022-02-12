using LinqToDB.Mapping;

namespace SecretSantaBot
{
    [Table(Name = "UsersWish")]
    class WishToDataBase
    {
        [Column(Name = "ID"), PrimaryKey, Identity]
        public int ID { get; set; }

        [Column(Name = "userid"), NotNull]
        public long User_Id { get; set; }

        [Column(Name = "wish")]
        public string Wish { get; set; }

        [Column(Name = "iswishaccept")]
        public bool IsWish { get; set; }
    }
}
