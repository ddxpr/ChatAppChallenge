namespace ChatApp.Interfaces
{
    public interface IBotService
    {
        Task ProcessCommandAsync(string message);
    }
}
