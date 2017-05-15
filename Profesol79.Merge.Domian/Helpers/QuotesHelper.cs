//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="WPE" file="QuotesHelper.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-21, 10:35 PM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.Domain.Helpers
{
    /// <summary>The quotes helper.</summary>
    public static class QuotesHelper
    {
        /// <summary>The escape string if needed.</summary>
        /// <param name="data">The data.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string EscapeStringIfNeeded(this string data)
        {
            if (data == null)
            {
                return data;
            }

            if (data.Contains(",") && !data.Trim().StartsWith("\""))
            {
                data = $"\"{data.Trim()}\"";
            }

            return data;
        }
    }
}
