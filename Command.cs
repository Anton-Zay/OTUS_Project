using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SecretSantaBot
{
    class Command
    {
        public List<Entity> entities { get; set; }
        public void Join(Message message, List<Entity> entities)
        {
            this.entities = entities;

            if (this.entities.Count > 0)
            {
                var joinEntity = this.entities.SingleOrDefault(Entity => Entity.User_Id == message.From.Id);
                if (joinEntity != null)
                {
                    joinEntity.IsActive = true;
                    return;
                }
            }

            Entity entity = new Entity
            {
                ID = entities.Count + 1,
                Chat_Id = message.Chat.Id,
                User_Id = message.From.Id,
                First_Name = message.From.FirstName,
                Last_Name = message.From.LastName,
                IsActive = true
            };
            if (MyContains(entity, this.entities))
            {
                return;
            }
            this.entities.Add(entity);
        }

        public void Wish(Message message, List<Entity> entities)
        {
            Entity user = SearchEntity(message, entities);

            if (user == null)
            {
                return;
            }
            user.Wish = message.Text.Substring(6);
        }

        public void Exit(Message message, List<Entity> entities)
        {
            Entity user = SearchEntity(message, entities);

            if (user == null)
            {
                return;
            }
            user.IsActive = false;

        }

        async public void Launch(ITelegramBotClient botClient, Message message, List<Entity> entities)
        {
            this.entities = entities;
            if (this.entities.Count == 0)
            {
                return;
            }

            var launchEntity = this.entities.First(Entity => Entity.User_Id == message.From.Id);
            if (launchEntity.SantaFor != null)
            {
                return;
            }

            var validList = new List<Entity>();

            foreach (var item in this.entities)
            {
                if (message.From.Id != item.User_Id &&
                    item.Chat_Id == message.Chat.Id &&
                    item.IsActive == true &&
                    item.Gifted == null)
                {
                    validList.Add(item);
                }
            }

            if (validList.Count == 0)
            {
                return;
            }

            var random = new Random();
            Entity user;

            do
            {
                user = validList[random.Next(validList.Count)];
            }
            while (user.User_Id == message.From.Id);

            var mes = $"Вы являетесь тайным сантой для пользователя {user.First_Name} {user.Last_Name}\n" +
                $"Его предпочтения: {user.Wish}";

            await botClient.SendTextMessageAsync(message.From.Id, mes);

            launchEntity.SantaFor = user;
            user.Gifted = launchEntity;

        }

        private bool MyContains(Entity entity, List<Entity> entities)
        {
            foreach (var item in entities)

                if (entity.Chat_Id == item.Chat_Id && entity.User_Id == item.User_Id && entity.IsActive == item.IsActive)
                {
                    return true;
                }

            return false;
        }

        private Entity SearchEntity(Message message, List<Entity> entities)
        {
            var user_id = message.From.Id;
            Entity user = null;
            foreach (var item in entities)
            {
                if (item.User_Id == user_id)
                {
                    user = item;
                    break;
                }
            }
            return user;
        }
    }
}