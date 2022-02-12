using LinqToDB;
using LinqToDB.Data;
using SecretSantaBot.Service;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace SecretSantaBot.Commands
{
    class JoinCommand : ICommand
    {
        private readonly DataConnection dbToOtus_SecretSantaBase;

        public JoinCommand(DataConnection connection)
        {
            dbToOtus_SecretSantaBase = connection;
        }

        public async Task Execute(Message message)
        {
            Console.WriteLine("Зашёл в метод Join");

            var entity = dbToOtus_SecretSantaBase.GetTable<Entity>().SingleOrDefault(x => x.User_Id == message.From.Id &&
                                                                                          x.Chat_Id == message.Chat.Id);

            if (entity != null)
            {
                entity.IsActive = true;
                dbToOtus_SecretSantaBase.Update(entity);
                Console.WriteLine("Поменял статус");
                return;
            }

            entity = new Entity
            {
                Chat_Id = message.Chat.Id,
                User_Id = message.From.Id,
                First_Name = message.From?.FirstName,
                Last_Name = message.From?.LastName,
                IsActive = true
            };

            dbToOtus_SecretSantaBase.Insert(entity);
            Console.WriteLine("Добавил нового пользователя в базу данных");
        }
    }
}
