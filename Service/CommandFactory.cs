using LinqToDB.Data;
using SecretSantaBot.Commands;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SecretSantaBot.Service
{
    public class CommandFactory : ICommandFactory
    {
        private readonly DataConnection dbToOtus_SecretSantaBase;
        private readonly ITelegramBotClient _botClient;
        private Dictionary<string, ICommand> dictionary;
        public CommandFactory(ITelegramBotClient botClient, DataConnection connection)
        {
            _botClient = botClient;
            dbToOtus_SecretSantaBase = connection;

            dictionary = new Dictionary<string, ICommand>(){
                { CommandListConstants.START, new StartCommand(_botClient) },
                { CommandListConstants.JOIN, new JoinCommand(dbToOtus_SecretSantaBase) },
                { CommandListConstants.EXIT, new ExitCommand(dbToOtus_SecretSantaBase) },
                { CommandListConstants.WISH, new WishCommand(dbToOtus_SecretSantaBase) },
                { CommandListConstants.LAUNCH, new LaunchCommand(_botClient, dbToOtus_SecretSantaBase) }
            };
        }

        public ICommand GetCommand(Message message)
        {
            ICommand command = null;

            {
                var entity = dbToOtus_SecretSantaBase.GetTable<WishToDataBase>().SingleOrDefault(x => x.User_Id == message.From.Id);

                if (entity != null && entity.IsWish == true)
                {
                    return new AddWishCommand(dbToOtus_SecretSantaBase, entity);
                }
            }

            command = dictionary.GetValueOrDefault(message.Text);

            return command;
        }
    }
}