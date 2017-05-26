//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="Startup.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-05-25, 12:08 PM
// Last changed by: profesor79, 2017-05-26, 8:21 AM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

using Microsoft.Owin;

using Profesor79.Merge.Web;

[assembly: OwinStartup(typeof(Startup), "Configuration")]

namespace Profesor79.Merge.Web
{
    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app) { app.MapSignalR(); }
    }
}
