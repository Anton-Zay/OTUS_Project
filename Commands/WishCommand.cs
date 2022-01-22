using LinqToDB;
using LinqToDB.Data;
using SecretSantaBot.Service;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace SecretSantaBot.Commands
{
    class WishCommand : ICommand
    {
        private readonly DataConnection dbToOtus_SecretSantaBase;

        public WishCommand(DataConnection connection)
        {
            dbToOtus_SecretSantaBase = connection;
        }

        public async Task Execute(Message message)
        {
            Console.WriteLine("Зашёл в метод Wish");

            var entity = dbToOtus_SecretSantaBase.GetTable<WishToDataBase>().SingleOrDefault(x => x.User_Id == message.From.Id);

            if (entity != null)
            {
                entity.Wish = message.Text;
                dbToOtus_SecretSantaBase.Update(entity);
                Console.WriteLine("обновил Wish в базе данных");
                return;
            }

            entity = new WishToDataBase
            {
                User_Id = message.From.Id,
                Wish = message.Text
            };

            entity.Wish = message.Text;
            dbToOtus_SecretSantaBase.Insert(entity);
            Console.WriteLine("Добавил Wish в БД");
        }
    }
}
