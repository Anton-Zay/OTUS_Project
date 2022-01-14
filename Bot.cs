using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSantaBot
{
    class Bot
    {
        private static TelegramBotClient botClient;

        bool isWish = false;

        public async void Start()
        {
            botClient = new TelegramBotClient(Config.Token);

            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[] { UpdateType.Message }

            };
            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

            Console.WriteLine($"Бот начал работать");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message)
                return;

            if (update.Message!.Type != MessageType.Text)
                return;

            var messageText = update.Message.Text;

            Console.WriteLine($"ID этого сообщения - {update.Message.MessageId}");

            if (messageText == CommandListConstants.START)
            {
                new Command().Start(botClient, update.Message);
            }

            if (messageText == CommandListConstants.JOIN)
            {
                new Command().Join(update.Message);
            }

            if (isWish)
            {
                new Command().Wish(update.Message);
                isWish = false;
            }

            if (messageText == CommandListConstants.WISH)
            {
                isWish = true;
            }

            if (messageText == CommandListConstants.EXIT)
            {
                new Command().Exit(update.Message);
            }

            if (messageText == CommandListConstants.LAUNCH)
            {
                new Command().Launch(botClient, update.Message);
            }
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
