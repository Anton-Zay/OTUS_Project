using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using SecretSantaBot.Service;
using System;

namespace SecretSantaBot.Commands
{
    class StartCommand : ICommand
    {
        private readonly ITelegramBotClient _botClient;
        public StartCommand(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }
        public async Task Execute(Message message)
        {
            Console.WriteLine("Зашёл в метод Start");

            var mes = "Всем PEACE! Я - бот \"тайный санта\". Меня нужно добавить в чат с участниками.\n" +
                "Я могу выполнить следующие команды в чате, куда я добавлен:\n" +
                "- напиши \"/join\" и я добавлю тебя в перечень участников.\n" +
                "- напиши \"/exit\" если ты передумал, и не хочешь быть участником.\n" +
                "- напиши \"/launch\" если решился стать тайным сантой!\n" +
                "Ну и в дополнение, здесь, в личной переписке со мной можешь ввести команду \"/wish\" и следующим сообщением " +
                "я приму твои пожелания по подаркам.";
            await _botClient.SendTextMessageAsync(message.From.Id, mes);
        }
    }
}
