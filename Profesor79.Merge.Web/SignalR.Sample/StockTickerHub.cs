//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="StockTickerHub.cs">
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
    using System.Collections.Generic;

    using Microsoft.AspNet.SignalR.Hubs;

    [HubName("stockTicker")]
    public class StockTickerHub : Hub
    {
        private readonly StockTicker _stockTicker;

        public StockTickerHub()
            : this(StockTicker.Instance) { }

        public StockTickerHub(StockTicker stockTicker) { _stockTicker = stockTicker; }

        public void CloseMarket() { _stockTicker.CloseMarket(); }

        public IEnumerable<Stock> GetAllStocks() { return _stockTicker.GetAllStocks(); }

        public string GetMarketState() { return _stockTicker.MarketState.ToString(); }

        public void OpenMarket() { _stockTicker.OpenMarket(); }

        public void Reset() { _stockTicker.Reset(); }
    }
}
