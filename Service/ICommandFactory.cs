namespace SecretSantaBot.Service
{
    public interface ICommandFactory
    {
        ICommand GetCommand(string message);
    }
}