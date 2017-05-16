//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79" file="Global.asax.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-18, 3:59 PM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.WebApiMock
{
    using System.Web;
    using System.Web.Http;

    /// <summary>The web api application.</summary>
    public class WebApiApplication : HttpApplication
    {
        /// <summary>The application_ start.</summary>
        protected void Application_Start() { GlobalConfiguration.Configure(WebApiConfig.Register); }
    }
}
