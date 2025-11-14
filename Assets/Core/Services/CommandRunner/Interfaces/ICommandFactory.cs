using Core.Services.CommandRunner.Interfaces.Command;

namespace Core.Services.CommandRunner.Interfaces
{
    public interface ICommandFactory
    {
        T GetCommand<T>()
            where T : class, ICommandEntity, new();
    }
}