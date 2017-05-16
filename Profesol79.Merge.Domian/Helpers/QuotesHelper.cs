//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="WPE" file="QuotesHelper.cs">
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
