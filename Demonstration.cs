/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

using System.Linq;
using QuantConnect.Data;
using QuantConnect.Util;
using QuantConnect.Orders;
using QuantConnect.Algorithm;
using QuantConnect.DataSource;
using System.Collections.Generic;

namespace QuantConnect.DataLibrary.Tests
{
    public class Demonstration : QCAlgorithm
    {
        private decimal? _greatestValue = null;
        private List<string> _tickers = new()
        {
            "JPINTDDMEJPY",
            "USINTDMRKTJPY",
            "TRINTDEXR",
            "DTWEXM",
            "CBETHUSD",
            "VXGOGCLS",
            "CHINTDCHFDM",
            "DCPN3M",
            "BAMLEMPTPRVICRPITRIV",
            "VXGDXCLS"
        };

        private Dictionary<string, Symbol> _symbols = new Dictionary<string, Symbol>();

        public override void Initialize()
        {
            SetStartDate(2021, 1, 1);  //Set Start Date
            SetEndDate(2021, 7, 1);    //Set End Date
            AddEquity("SPY", Resolution.Daily);

            Fred.SetAuthCode("your_authentication_code");
            foreach (var ticker in _tickers)
            {
                _symbols[ticker] = AddData<Fred>(ticker).Symbol;

                //Historical data
                Debug($"We got {(History<Fred>(_symbols[ticker], 10, Resolution.Daily)).Count()} items from our history request for {ticker} FRED data");
            }
        }

        public override void OnData(Slice slice)
        {
            var data = slice.Get<Fred>();

            if (!data.IsNullOrEmpty())
            {
                Debug(data.ToString());
                var symbolsWithData = data.Values.Count;
                var greatestValue = data.Values.Select(x => x.Value).Max();
                if (symbolsWithData >= 5
                    && _greatestValue != null
                    && greatestValue > _greatestValue)
                {
                    SetHoldings("SPY", 1);
                }
                else
                {
                    SetHoldings("SPY", -1);
                }

                _greatestValue = greatestValue;
            }
        }

        public override void OnOrderEvent(OrderEvent orderEvent)
        {
            if (orderEvent.Status.IsFill())
            {
                Debug($"Purchased Stock: {orderEvent.Symbol}");
            }
        }
    }
}
