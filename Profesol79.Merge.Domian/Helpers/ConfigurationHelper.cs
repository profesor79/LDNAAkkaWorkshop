//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="SystemLauncher.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-05-15, 2:37 PM
// Last changed by: profesor79, 2017-05-26, 8:20 AM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.Domain.Helpers
{
    using System;
    using System.Threading;

    public static class ConfigurationHelper
    {
        public static string GetConfiguration()
        {
            var needToRead = true;
            string text = string.Empty;
            while (needToRead)
            {
                try
                {
                    text = System.IO.File.ReadAllText(@"C:\dockerExchange\cluster.config");

                    // check readings
                    needToRead = string.IsNullOrWhiteSpace(text);

                    if (!needToRead)
                    {
                        text = text.Replace("__hostname__", System.Net.Dns.GetHostName());
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("config file is absent");
                    Thread.Sleep(250);
                }


            }

            return text;
        }
    }
}
