using Core.Services.CommandRunner.Interfaces.Command;

namespace Core.Services.CommandRunner.Interfaces
{
    public interface ICommandExecutionService
    {
        void Execute<T>()
            where T : class, ICommand, new();

        void Execute<TCommand, TCommandData>(TCommandData commandData)
            where TCommand : class, ICommandWithData<TCommandData>, new()
            where TCommandData : struct;
    }
}