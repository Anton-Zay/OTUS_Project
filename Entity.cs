using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaBot
{
    class Entity
    {
        public int ID { get; set; } 
        public long Chat_Id { get; set; }
        public long User_Id { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Wish { get; set; }
        public bool IsActive { get; set; }
        public Entity SantaFor { get; set; } = null; // ссылка тому кому дарит
        public Entity Gifted { get; set; } = null; // ссылка на того кто дарит этому
    }
}
