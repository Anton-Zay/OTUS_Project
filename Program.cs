using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace SecretSantaBot
{
    class Program
    {
        private static TelegramBotClient botClient;
        public static List<Entity> list;
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Start();
            Console.ReadLine();
        }

        async private void Start()
        {
            botClient = new TelegramBotClient(Config.Token);
            list = new List<Entity>();

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

            //botClient.OnMessage += OnMessageHandler;
            Console.WriteLine($"Бот начал работать");

            /*var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();*/
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message)
                return;

            if (update.Message!.Type != MessageType.Text)
                return;

            var messageText = update.Message.Text;

            if (messageText == "/join")
            {
                new Command().Join(update.Message, Program.list);
            }

            if (messageText.Length > 5 && messageText.Substring(0, 5) == "/wish")
            {
                new Command().Wish(update.Message, Program.list);
            }

            if (messageText == "/exit")
            {
                new Command().Exit(update.Message, Program.list);
            }

            if (messageText == "/launch")
            {
                new Command().Launch(botClient, update.Message, Program.list);
            }


            PrintEntityList(list);

            //Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            // Echo received message text
            /*Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "You said:\n" + messageText,
                cancellationToken: cancellationToken);*/
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

        public void PrintEntityList(List<Entity> list)
        {
            Console.WriteLine("--------------------\nНовый запрос");
            foreach (var item in list)
            {
                if (item.SantaFor != null && item.Gifted == null)
                {
                    Console.WriteLine($"ID БД = {item.ID}\n" +
                                        $"Chat_Id = {item.Chat_Id}\n" +
                                        $"User_Id = {item.User_Id}\n" +
                                        $"First_Name = {item.First_Name}\n" +
                                        $"Last_Name = {item.Last_Name}\n" +
                                        $"Wish = {item.Wish}\n" +
                                        $"Is_Active = {item.IsActive}\n" +
                                        $"SantaFor = {item.SantaFor.First_Name} {item.SantaFor.Last_Name}\n"
                                        );
                }
                else if (item.SantaFor == null && item.Gifted != null)
                {
                    Console.WriteLine($"ID БД = {item.ID}\n" +
                                        $"Chat_Id = {item.Chat_Id}\n" +
                                        $"User_Id = {item.User_Id}\n" +
                                        $"First_Name = {item.First_Name}\n" +
                                        $"Last_Name = {item.Last_Name}\n" +
                                        $"Wish = {item.Wish}\n" +
                                        $"Is_Active = {item.IsActive}\n" +
                                        $"Gifted = {item.Gifted.First_Name} {item.Gifted.Last_Name}\n"
                                        );
                }
                else if (item.SantaFor == null && item.Gifted == null)
                {
                    Console.WriteLine($"ID БД = {item.ID}\n" +
                                        $"Chat_Id = {item.Chat_Id}\n" +
                                        $"User_Id = {item.User_Id}\n" +
                                        $"First_Name = {item.First_Name}\n" +
                                        $"Last_Name = {item.Last_Name}\n" +
                                        $"Wish = {item.Wish}\n" +
                                        $"Is_Active = {item.IsActive}\n"
                                        );
                }
                else
                {
                    Console.WriteLine($"ID БД = {item.ID}\n" +
                                        $"Chat_Id = {item.Chat_Id}\n" +
                                        $"User_Id = {item.User_Id}\n" +
                                        $"First_Name = {item.First_Name}\n" +
                                        $"Last_Name = {item.Last_Name}\n" +
                                        $"Wish = {item.Wish}\n" +
                                        $"Is_Active = {item.IsActive}\n" +
                                        $"SantaFor = {item.SantaFor.First_Name} {item.SantaFor.Last_Name}\n" +
                                        $"Gifted = {item.Gifted.First_Name} {item.Gifted.Last_Name}\n"
                                        );
                }
            }
        }
    }
}