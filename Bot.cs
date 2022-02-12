using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using SecretSantaBot.Service;
using LinqToDB.Data;

namespace SecretSantaBot
{
    class Bot
    {
        private static TelegramBotClient botClient;
        private DataConnection dbToOtus_SecretSantaBase;

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

            var messageText = update.Message;

            dbToOtus_SecretSantaBase = new DataConnection(LinqToDB.ProviderName.PostgreSQL, Config.SqlConnectionStringToOtus_SecretSantaBase);
            var commandFactory = new CommandFactory(botClient, dbToOtus_SecretSantaBase);
            var command = commandFactory.GetCommand(messageText);
            if(command == null)
            {
                return;
            }
            await command.Execute(update.Message);

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