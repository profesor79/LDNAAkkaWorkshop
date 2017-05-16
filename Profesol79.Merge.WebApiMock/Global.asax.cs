//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="WPE" file="Global.asax.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-05-15, 2:37 PM
// Last changed by: A happy WPE candidate, 2017-05-16, 10:48 AM 
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
