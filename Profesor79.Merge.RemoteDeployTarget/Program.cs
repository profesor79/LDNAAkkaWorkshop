//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="Program.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-05-29, 9:06 PM
// Last changed by: profesor79, 2017-06-01, 10:43 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.RemoteDeployTarget
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Configuration;

    using Serilog;

    internal class Program
    {
        private static string GetClusterHost()
        {
            var needToRead = true;
            var text = string.Empty;
            while (needToRead)
            {
                try
                {
                    text = File.ReadAllText(@"C:\dockerExchange\clusterMaster.txt");

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

        private static void Main(string[] args)
        {
            

            Log.Logger = new LoggerConfiguration().WriteTo.LiterateConsole().CreateLogger();

            Log.Information("Ah, there you are!");
            
            using (var system = ActorSystem.Create("ClusterSystem", ConfigurationFactory.ParseString(GetConfig())))

            {
                var task = new Task(
                    () =>
                        {
                            while (true)
                            {
                                Thread.Sleep(10 * 1000);
                                var hostname2 = Dns.GetHostName();
                                Console.WriteLine($" hostname: {hostname2} is alive {DateTime.Now.ToString("o")}!");
                            }
                        });
                
                system.WhenTerminated.Wait();

            }
        }

        private static string GetConfig()
        {
            var config = @"
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
                        hostname = __hostname__
                    }
                }

  cluster {
        seed-nodes = [""akka.tcp://ClusterSystem@__remoteHost__:8091""]
        roles = [crawler]
    }
    }
my-dispatcher {
	type = TaskDispatcher
		throughput = 1
		throughput-deadline-time = 0ms
}
";
            var remoteHost = GetClusterHost();
            var hostname = Dns.GetHostName();
            Console.WriteLine($"hostname: {hostname}");
            return config.Replace("__hostname__", hostname).Replace("__remoteHost__", remoteHost);
        }
    }
}
