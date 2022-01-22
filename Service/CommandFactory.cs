using LinqToDB.Data;
using SecretSantaBot.Commands;
using System;
using Telegram.Bot;

namespace SecretSantaBot.Service
{
    public class CommandFactory : ICommandFactory
    {
        private readonly DataConnection dbToOtus_SecretSantaBase;
        private readonly ITelegramBotClient _botClient;
        public CommandFactory(ITelegramBotClient botClient, DataConnection connection)
        {
            _botClient = botClient;
            dbToOtus_SecretSantaBase = connection;
        }

        public ICommand GetCommand(string message)
        {
            ICommand command = null;

            if (Bot.isWish)
            {
                command = new WishCommand(dbToOtus_SecretSantaBase);
                Bot.isWish = false;
                return command;
            }

            switch (message)
            {
                case CommandListConstants.START:
                    command = new StartCommand(_botClient);
                    break;
                case CommandListConstants.JOIN:
                    command = new JoinCommand(dbToOtus_SecretSantaBase);
                    break;
                case CommandListConstants.EXIT:
                    command = new ExitCommand(dbToOtus_SecretSantaBase);
                    break;
                case CommandListConstants.WISH:
                    Bot.isWish = true;
                    break;
                case CommandListConstants.LAUNCH:
                    command = new LaunchCommand(_botClient, dbToOtus_SecretSantaBase);
                    break;
            }

            return command;
        }
    }
}
