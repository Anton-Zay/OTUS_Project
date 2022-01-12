using LinqToDB;
using System;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SecretSantaBot
{
    public class Command
    {
        LinqToDB.Data.DataConnection db = new LinqToDB.Data.DataConnection(LinqToDB.ProviderName.PostgreSQL, Config.SqlConnectionString);
        public void Join(Message message)
        {
            Console.WriteLine("Зашёл в метод Join");

            var entity = db.GetTable<Entity>().SingleOrDefault(x => x.User_Id == message.From.Id &&
                                                                    x.Chat_Id == message.Chat.Id);

            if (entity != null)
            {
                entity.IsActive = true;
                db.Update(entity);
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

            db.Insert(entity);
            Console.WriteLine("Добавил нового пользователя в базу данных");

        }

        public void Wish(Message message)
        {
            Console.WriteLine("Зашёл в метод Wish");

            var entity = db.GetTable<Entity>().SingleOrDefault(x => x.User_Id == message.From.Id &&
                                                                    x.Chat_Id == message.Chat.Id);

            if (entity == null)
            {
                Console.WriteLine("Ничего в методе Wish не сделал");
                return;
            }

            entity.Wish = message.Text.Substring(6);
            db.Update(entity);
            Console.WriteLine("Добавил Wish в БД");

        }

        public void Exit(Message message)
        {
            Console.WriteLine("Зашёл в метод Exit");
            var entity = db.GetTable<Entity>().SingleOrDefault(x => x.User_Id == message.From.Id &&
                                                                    x.Chat_Id == message.Chat.Id);

            if (entity == null)
            {
                Console.WriteLine("Ничего в методе Exit не сделал");
                return;
            }
            entity.IsActive = false;
            db.Update<Entity>(entity);

            Console.WriteLine("Поменял статус из метода Exit");

        }

        public async void Launch(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine("Зашёл в метод Launch");

            var table = db.GetTable<Entity>().Where(x => x.Chat_Id == message.Chat.Id &&
                                                         x.User_Id != message.From.Id &&
                                                         x.IsActive == true);

            var launchEntity = db.GetTable<Entity>().First(x => x.User_Id == message.From.Id);
            if (launchEntity.SantaForUserId != 0)
            {
                var tmpEntity = table.SingleOrDefault(x => x.User_Id == launchEntity.SantaForUserId);
                var mess = $"Вы уже являетесь тайным сантой для пользователя {tmpEntity.First_Name} {tmpEntity.Last_Name}";
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

            var mes = $"Вы являетесь тайным сантой для пользователя {user.First_Name} {user.Last_Name}\n" +
                $"Его предпочтения: {user.Wish}";

            await botClient.SendTextMessageAsync(message.From.Id, mes);

            launchEntity.SantaForUserId = user.User_Id;
            user.GiftedForUserId = launchEntity.User_Id;
            db.Update<Entity>(launchEntity);
            db.Update<Entity>(user);
        }
    }
}