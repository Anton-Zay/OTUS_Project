using LinqToDB;
using LinqToDB.Data;
using SecretSantaBot.Service;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace SecretSantaBot.Commands
{
    class AddWishCommand : ICommand
    {
        private readonly DataConnection dbToOtus_SecretSantaBase;
        WishToDataBase _entity;

        public AddWishCommand(DataConnection connection, WishToDataBase entity)
        {
            dbToOtus_SecretSantaBase = connection;
            _entity = entity;
        }

        public async Task Execute(Message message)
        {
            Console.WriteLine("Зашёл в метод AddWish");
            _entity.Wish = message.Text;
            _entity.IsWish = false;
            dbToOtus_SecretSantaBase.Update(_entity);
            Console.WriteLine("Добавил Wish в БД");
        }
    }
}
