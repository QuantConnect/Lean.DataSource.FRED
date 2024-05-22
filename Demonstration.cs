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

namespace QuantConnect.DataLibrary.Tests
{
    public class Demonstration : QCAlgorithm
    {
        private Symbol _fredSymbol;
        private decimal? _lastValue = null;

        public override void Initialize()
        {
            SetStartDate(2021, 1, 1);  //Set Start Date
            SetEndDate(2021, 7, 1);    //Set End Date
            AddEquity("SPY", Resolution.Daily);
            _fredSymbol = AddData<Fred>("JPINTDDMEJPY").Symbol;

            // Historical data
            var history = History<Fred>(_fredSymbol, 10, Resolution.Daily);
            Debug($"We got {history.Count()} items from our history request for JPINTDDMEJPY FRED data");
        }

        public override void OnData(Slice slice)
        {
            var data = slice.Get<Fred>();

            if (!data.IsNullOrEmpty())
            {
                Debug(data.ToString());

                // based on the "JPINTDDMEJPY" Fred Symbol, we will buy or short the underlying equity
                if (_lastValue != null && data[_fredSymbol].Value > _lastValue)
                {
                    SetHoldings("SPY", 1);
                }
                else
                {
                    SetHoldings("SPY", -1);
                }

                _lastValue = data[_fredSymbol].Value;
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
