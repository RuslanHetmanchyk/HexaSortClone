using Core.Services.CommandRunner.Interfaces;
using Core.Services.CommandRunner.Interfaces.Command;

namespace Core.Services.CommandRunner.Implementation
{
    public class CommandExecutionService : ICommandExecutionService
    {
        private readonly ICommandFactory commandFactory;

        public CommandExecutionService(ICommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
        }

        public void Execute<TCommand>()
            where TCommand : class, ICommand, new()
        {
            var command = commandFactory.GetCommand<TCommand>();
            command.Execute();
        }

        public void Execute<TCommand, TCommandData>(TCommandData commandData)
            where TCommand : class, ICommandWithData<TCommandData>, new()
            where TCommandData : struct
        {
            var command = commandFactory.GetCommand<TCommand>();
            command.Execute(commandData);
        }
    }
}