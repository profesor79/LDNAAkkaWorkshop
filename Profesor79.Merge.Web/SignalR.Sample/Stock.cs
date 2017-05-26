//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="Stock.cs">
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

namespace Microsoft.AspNet.SignalR.StockTicker
{
    using System;

    public class Stock
    {
        private decimal _price;

        public decimal Change
        {
            get
            {
                return Price - DayOpen;
            }
        }

        public decimal DayHigh { get; private set; }

        public decimal DayLow { get; private set; }

        public decimal DayOpen { get; private set; }

        public decimal LastChange { get; private set; }

        public double PercentChange
        {
            get
            {
                return (double)Math.Round(Change / Price, 4);
            }
        }

        public decimal Price
        {
            get
            {
                return _price;
            }
            set
            {
                if (_price == value)
                {
                    return;
                }

                LastChange = value - _price;
                _price = value;

                if (DayOpen == 0)
                {
                    DayOpen = _price;
                }
                if (_price < DayLow || DayLow == 0)
                {
                    DayLow = _price;
                }
                if (_price > DayHigh)
                {
                    DayHigh = _price;
                }
            }
        }

        public string Symbol { get; set; }
    }
}
