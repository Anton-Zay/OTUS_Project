using LinqToDB;
using LinqToDB.Data;
using SecretSantaBot.Service;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace SecretSantaBot.Commands
{
    class ExitCommand : ICommand
    {
        private readonly DataConnection dbToOtus_SecretSantaBase;

        public ExitCommand(DataConnection connection)
        {
            dbToOtus_SecretSantaBase = connection;
        }

        public async Task Execute(Message message)
        {
            Console.WriteLine("Зашёл в метод Exit");
            var entity = dbToOtus_SecretSantaBase.GetTable<Entity>().SingleOrDefault(x => x.User_Id == message.From.Id &&
                                                                                          x.Chat_Id == message.Chat.Id);

            if (entity == null)
            {
                Console.WriteLine("Ничего в методе Exit не сделал");
                return;
            }
            entity.IsActive = false;
            dbToOtus_SecretSantaBase.Update<Entity>(entity);

            Console.WriteLine("Поменял статус из метода Exit");
        }
    }
}
