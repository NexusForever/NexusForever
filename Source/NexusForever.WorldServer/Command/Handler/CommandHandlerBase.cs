﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    public abstract class CommandHandlerBase : ICommandHandler
    {
        public ILogger Logger { get; }
        public abstract int Order { get; }

        protected CommandHandlerBase(ILogger logger)
        {
            Logger = logger;
        }

        protected static void ParseCommand(string value, out string command, out string[] parameters)
        {
            string[] split = value.Split(' ');
            command = split[0];
            parameters = split.Skip(1).ToArray();
        }

        public abstract IEnumerable<string> GetCommands();
        public abstract void Handle(CommandContext session, string text);
        public abstract bool Handles(CommandContext session, string input);
    }
}