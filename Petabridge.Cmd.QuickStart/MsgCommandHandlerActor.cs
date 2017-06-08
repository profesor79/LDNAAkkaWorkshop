using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Petabridge.Cmd.Host;

namespace Petabridge.Cmd.QuickStart
{
    /// <summary>
    /// Actor responsible for handling all <see cref="Command"/>s defined inside <see cref="MsgCommands.Palette"/>
    /// </summary>
    public class MsgCommandHandlerActor : CommandHandlerActor
    {
        private readonly IActorRef _messageMemorizer;
        private static readonly Regex TimeRegex = new Regex(@"^(?<value>([0-9]+(\.[0-9]+)?))\s*(?<unit>(ms|s|h|m|d))$", RegexOptions.Compiled);

        public MsgCommandHandlerActor(IActorRef messageMemorizer) : base(MsgCommands.Palette)
        {
            _messageMemorizer = messageMemorizer;
            Process(MsgCommands.StartSystem.Name, HandleStartSystem);
            
        }

        public void HandleStartSystem(Command purge)
        {
            RootActorRef.SystemRoot.Tell(new StartFromCli());
            Sender.Tell(CommandResponse.Empty);
        }
        
      
    }

    public class StartFromCli
    {
    }

    public static class RootActorRef
    {
        public static IActorRef SystemRoot { get; private set; }

        public static void SetReference(IActorRef messageMemorizer) { SystemRoot = messageMemorizer; }
    }
}
