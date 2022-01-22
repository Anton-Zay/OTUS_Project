using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace SecretSantaBot.Service
{
    public interface ICommand
    {
        Task Execute(Message message);
    }
}
