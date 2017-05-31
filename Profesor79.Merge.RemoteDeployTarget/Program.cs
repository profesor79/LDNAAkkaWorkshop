using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Profesor79.Merge.RemoteDeployTarget
{
    using Akka.Actor;
    using Akka.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
    .WriteTo.LiterateConsole()
    .CreateLogger();

            Log.Information("Ah, there you are!");
            var hostname = System.Net.Dns.GetHostName();
            Console.WriteLine($"hostname: {hostname}");
            using (var system = ActorSystem.Create("DeployTarget", ConfigurationFactory.ParseString(@"
            akka {  
        stdout-loglevel = DEBUG
		loglevel = DEBUG
		akka.actor.serialize-messages = on
		 loggers=[""Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog""]

        actor.provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
    DEBUG {
			log-config-on-start = on
				receive = off
				autoreceive = on
				lifecycle = on
				event-stream = on
				unhandled = on
				stdout-loglevel = DEBUG
				log-received-messages = off
		}
        
remote {
                    helios.tcp {
                        port = 8090
                        hostname = " + hostname + @"
                    }
                }
            }
my-dispatcher {
	type = TaskDispatcher
		throughput = 1
		throughput-deadline-time = 0ms
}
")))
            {
                while (true)
                {
                    Thread.Sleep(10 * 1000);
                    Console.WriteLine($" hostname: {hostname} is alive {DateTime.Now.ToString("o")}!");
                }
            }
        }
    }
}
