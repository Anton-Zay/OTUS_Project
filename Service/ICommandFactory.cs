using Telegram.Bot.Types;

namespace SecretSantaBot.Service
{
    public interface ICommandFactory
    {
        ICommand GetCommand(Message message);
    }
}