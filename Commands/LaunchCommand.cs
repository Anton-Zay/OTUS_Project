using LinqToDB;
using LinqToDB.Data;
using SecretSantaBot.Service;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace SecretSantaBot.Commands
{
    class LaunchCommand : ICommand
    {
        private readonly ITelegramBotClient botClient;
        private readonly DataConnection dbToOtus_SecretSantaBase;

        public LaunchCommand(ITelegramBotClient botClient, DataConnection connection)
        {
            this.botClient = botClient;
            dbToOtus_SecretSantaBase = connection;
        }
        public async Task Execute(Message message)
        {
            Console.WriteLine("Зашёл в метод Launch");

            var table = dbToOtus_SecretSantaBase.GetTable<Entity>().Where(x => x.Chat_Id == message.Chat.Id &&
                                                                               x.User_Id != message.From.Id &&
                                                                               x.IsActive == true);

            var launchEntity = dbToOtus_SecretSantaBase.GetTable<Entity>().FirstOrDefault(x => x.Chat_Id == message.Chat.Id &&
                                                                                               x.User_Id == message.From.Id);
            if (launchEntity == null)
            {
                return;
            }

            string mess;
            WishToDataBase wishEntity;

            if (launchEntity.SantaForUserId != 0)
            {
                var tmpEntity = table.SingleOrDefault(x => x.User_Id == launchEntity.SantaForUserId);
                mess = $"Вы уже являетесь тайным сантой для пользователя {tmpEntity.First_Name} {tmpEntity.Last_Name}";
                wishEntity = dbToOtus_SecretSantaBase.GetTable<WishToDataBase>().SingleOrDefault(x => x.User_Id == launchEntity.SantaForUserId);

                if (wishEntity != null)
                {
                    mess += $"\nЕго предпочтения: {wishEntity.Wish}";
                }
                await botClient.SendTextMessageAsync(message.From.Id, mess);
                Console.WriteLine("Вы уже являетесь тайным сантой");
                return;
            }

            if (table.Count() == 0 || table == null)
            {
                await botClient.SendTextMessageAsync(message.From.Id, "Нет участников для распределения");
                Console.WriteLine("Слишком малое количество участников");
                return;
            }

            var validList = table.Where(x => x.GiftedForUserId == 0).ToList();

            var random = new Random();
            Entity user;

            user = validList[random.Next(validList.Count)];

            mess = $"Вы являетесь тайным сантой для пользователя {user.First_Name} {user.Last_Name}";

            wishEntity = dbToOtus_SecretSantaBase.GetTable<WishToDataBase>().SingleOrDefault(x => x.User_Id == user.User_Id);
            if (wishEntity != null)
            {
                mess += $"\nЕго предпочтения: {wishEntity.Wish}";
            }

            await botClient.SendTextMessageAsync(message.From.Id, mess);

            launchEntity.SantaForUserId = user.User_Id;
            user.GiftedForUserId = launchEntity.User_Id;
            dbToOtus_SecretSantaBase.Update<Entity>(launchEntity);
            dbToOtus_SecretSantaBase.Update<Entity>(user);
        }
    }
}
