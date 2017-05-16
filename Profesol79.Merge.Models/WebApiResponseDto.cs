// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="WebApiResponseDto.cs">
//   
// </copyright>
// <summary>
//   Created: 2017-04-18, 10:03 PM
//   Last changed by: A happy WPE candidate, 2017-05-02, 10:33 AM
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
