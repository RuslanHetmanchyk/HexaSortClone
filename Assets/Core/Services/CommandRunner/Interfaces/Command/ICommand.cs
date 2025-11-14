namespace Core.Services.CommandRunner.Interfaces.Command
{
    public interface ICommand : ICommandEntity
    {
        void Execute();
    }
}