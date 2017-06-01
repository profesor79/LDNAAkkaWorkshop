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

            var remoteHost = GetClusterHost();

            Log.Logger = new LoggerConfiguration()
    .WriteTo.LiterateConsole()
    .CreateLogger();

            Log.Information("Ah, there you are!");
            var hostname = System.Net.Dns.GetHostName();
            Console.WriteLine($"hostname: {hostname}");
            using (var system = ActorSystem.Create("ClusterSystem", ConfigurationFactory.ParseString(@"
            akka {  
        stdout-loglevel = DEBUG
		loglevel = DEBUG
		akka.actor.serialize-messages = on
		 loggers=[""Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog""]

        actor.provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""

  deployment {
        /api/myClusterPoolRouter {
          router = round-robin-pool # routing strategy
          nr-of-instances = 10 # max number of total routees
          cluster {
             enabled = on
             allow-local-routees = on
             use-role = crawler
             max-nr-of-instances-per-node = 1

          }
        }
      }

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
                        port = 0 #let os pick random port
                        hostname = " + hostname + @"
                    }
                }

  cluster {
        seed-nodes = [""akka.tcp://ClusterSystem@" + remoteHost + @":8091""]
        roles = [crawler]
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

        private static string GetClusterHost()
        {
            var needToRead = true;
            string text = string.Empty;
            while (needToRead)
            {
                try
                {
                    text = System.IO.File.ReadAllText(@"C:\dockerExchange\clusterMaster.txt");

                    // check readings
                    needToRead = string.IsNullOrWhiteSpace(text);
                }
                catch (Exception)
                {
                    Console.WriteLine($"waiting for host file:{DateTime.Now.ToString("O")}");
                    Thread.Sleep(250);
                }


            }

            return text;
        }
    }
}
