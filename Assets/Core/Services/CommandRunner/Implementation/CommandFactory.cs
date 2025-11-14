using System;
using System.Collections.Generic;
using Core.Helpers;
using Core.Services.CommandRunner.Interfaces;
using Core.Services.CommandRunner.Interfaces.Command;
using Zenject;

namespace Core.Services.CommandRunner.Implementation
{
    public class CommandFactory : ICommandFactory
    {
        private readonly DiContainer diContainer;
        private readonly Dictionary<Type, ICommandEntity> commandsCache = new();

        public CommandFactory(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }

        public T GetCommand<T>()
            where T : class, ICommandEntity, new()
        {
            var commandType = typeof(T);
            if (commandsCache.TryGetValue(commandType, out var command))
            {
                return command as T;
            }

            return InjectCommandAndAddToCache(new T());
        }

        private T InjectCommandAndAddToCache<T>(T command)
            where T : class, ICommandEntity
        {
            ContainerHelper
                .GetCurrentContainer(diContainer)
                .Inject(command);
            commandsCache.Add(command.GetType(), command);

            return command;
        }
    }
}