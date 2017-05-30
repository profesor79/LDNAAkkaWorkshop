using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profesor79.Merge.RemoteDeployTarget
{
    using Akka.Actor;
    using Akka.Configuration;

    class Program
    {
        static void Main(string[] args)
        {

            var hostname = System.Net.Dns.GetHostName();
            Console.WriteLine($"hostname: {hostname}");
            using (var system = ActorSystem.Create("DeployTarget", ConfigurationFactory.ParseString(@"
            akka {  
    stdout-loglevel = debug
		loglevel = debug
		akka.actor.serialize-messages = on
		loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]

        actor.provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
    debug {
			log-config-on-start = on
				receive = off
				autoreceive = on
				lifecycle = on
				event-stream = on
				unhandled = on
				stdout-loglevel = debug
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
                Console.ReadKey();
            }
        }
    }
}
