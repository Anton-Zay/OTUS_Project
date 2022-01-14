using LinqToDB;
using System;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SecretSantaBot
{
    public class Command
    {
        LinqToDB.Data.DataConnection dbToOtus_SecretSantaBase = new LinqToDB.Data.DataConnection(LinqToDB.ProviderName.PostgreSQL, Config.SqlConnectionStringToOtus_SecretSantaBase);

        public async void Start(ITelegramBotClient botClient, Message message)
        {
            var mes = "Всем PEACE! я - бот \"тайный санта\". Меня нужно добавить в чат с участниками.\n" +
                "Я могу выполнить следующие команды в чате, куда я добавлен:\n" +
                "- напиши \"/join\" и я добавлю тебя в перечень участников.\n" +
                "- напиши \"/exit\" если ты передумал, и не хочешь быть участником.\n" +
                "- напиши \"/launch\" если решился стать тайным сантой!\n" +
                "Ну и в дополнение, здесь, в личной переписке со мной можешь ввести команду \"/wish\" и следующим сообщением " +
                "я приму твои пожелания по подаркам.";
            await botClient.SendTextMessageAsync(message.From.Id, mes);

        }
        public void Join(Message message)
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

        public void Wish(Message message)
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

        public void Exit(Message message)
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

        public async void Launch(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine("Зашёл в метод Launch");

            var table = dbToOtus_SecretSantaBase.GetTable<Entity>().Where(x => x.Chat_Id == message.Chat.Id &&
                                                                               x.User_Id != message.From.Id &&
                                                                               x.IsActive == true);

            var launchEntity = dbToOtus_SecretSantaBase.GetTable<Entity>().First(x => x.Chat_Id == message.Chat.Id &&
                                                                                      x.User_Id == message.From.Id);
            string wish = null;
            string mess;
            var wishEntity = dbToOtus_SecretSantaBase.GetTable<WishToDataBase>().SingleOrDefault(x => x.User_Id == launchEntity.SantaForUserId);

            if (launchEntity.SantaForUserId != 0)
            {
                var tmpEntity = table.SingleOrDefault(x => x.User_Id == launchEntity.SantaForUserId);
                mess = $"Вы уже являетесь тайным сантой для пользователя {tmpEntity.First_Name} {tmpEntity.Last_Name}";

                if(wishEntity != null)
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