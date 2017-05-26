//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="WebApiResponseDto.cs">
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

namespace Profesor79.Merge.Models
{
    /// <summary>The web api object.</summary>
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
