using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreVS2017Docker.Controllers
{
    using System;
        using System.Threading;


        using Microsoft.AspNetCore.Mvc;



        /// <summary>The account controller.</summary>
        public class ShopController : Controller
        {
            // GET: api/Default/5
            /// <summary>The get.</summary>
            /// <param name="id">The id.</param>
            /// <returns>The <see cref="IHttpActionResult"/>.</returns>
            [Route("api/shop/{id}")]
            public WebApiResponseDto Get(int id)
            {


                if (id == 8172)
                {
                    throw new OutOfMemoryException();
                }

                var random = new Random();
                var response = new WebApiResponseDto { DataId = id, SaleValue = random.Next(30, 50000) };

//              var sleepTime = random.Next(30, 100);
//              Thread.Sleep(sleepTime);
                return response;
            }
        }

        public class WebApiResponseDto
        {
            /// <summary>Gets or sets the data id.</summary>
            public int DataId { get; set; }

            /// <summary>Gets or sets a value indicating whether IsError.</summary>
            public bool IsError { get; set; }

            /// <summary>Gets or sets the sale value.</summary>
            public int SaleValue { get; set; }
        }


}
