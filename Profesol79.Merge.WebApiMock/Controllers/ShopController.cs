//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="ShopController.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-05-15, 2:37 PM
// Last changed by: profesor79, 2017-05-26, 8:21 AM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.WebApiMock.Controllers
{
    using System;
    using System.Threading;
    using System.Web.Http;

    using Profesor79.Merge.Models;

    /// <summary>The account controller.</summary>
    public class ShopController : ApiController
    {
        // GET: api/Default/5
        /// <summary>The get.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="IHttpActionResult"/>.</returns>
        [Route("api/shop/{id}")]
        public IHttpActionResult Get(int id)
        {
            if (id == 222222)
            {
                return NotFound();
            }

            if (id == 8172)
            {
                throw new OutOfMemoryException();
            }

            var random = new Random();
            var response = new WebApiResponseDto { DataId = id, SaleValue = random.Next(30, 50000) };

            var sleepTime = random.Next(30, 1500);
            Thread.Sleep(sleepTime);
            return Ok(response);
        }
    }
}
