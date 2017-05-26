//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="MergeObjectDto.cs">
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
    /// <summary>The merge object dto.</summary>
    public struct MergeObjectDto
    {
        // order that we need to save fields
        // Account ID, First Name, Created On, Status, Status Set On

        /// <summary>Gets or sets the account id.</summary>
        public int DataId { get; set; }

        public int SaleValue { get; set; }
    }
}
