# QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
# Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

from AlgorithmImports import *

class Demonstration(QCAlgorithm):
    def Initialize(self):
        self.SetStartDate(2021, 1, 1)
        self.SetEndDate(2021, 7, 1)

        numberOfPoints = {"JPINTDDMEJPY" : 180, "TRINTDEXR" : 180, "CBETHUSD" : 179, "VXGOGCLS" : 126, "DCPN3M" : 114, "BAMLEMPTPRVICRPITRIV" : 129}
        self.symbols = {}

        Fred.SetAuthCode("your_authentication_code")
        self.AddEquity("SPY", Resolution.Daily)
        for ticker in numberOfPoints.keys():
            self.symbols[ticker] = self.AddData(Fred, ticker, Resolution.Daily).Symbol

            historicalDataCount = len(self.History(Fred, self.symbols[ticker], 180, Resolution.Daily))
            if historicalDataCount < numberOfPoints[ticker]:
                raise Exception(f"We expected more than {numberOfPoints[ticker]} points for {ticker} FRED symbol, but just {historicalDataCount} points were obtained.");

            self.Debug(f"We got {historicalDataCount} items from our history request for {ticker} FRED data")

        self.greatestValue = -1;

    def OnData(self, data):
        data = data.Get(Fred)

        if len(data.Values) != 0 and (not self.Portfolio.Invested):
            self.Debug(str(data))

            symbolsWithData = len(data.Values)
            greatestValue = max(list(map(lambda x: x.Value, data.Values)))

            if symbolsWithData >= 5 and self.greatestValue != -1 and greatestValue > self.greatestValue:
                self.SetHoldings("SPY", 1)
            else:
                self.SetHoldings("SPY", -1)

            self.greatestValue = greatestValue

    def OnOrderEvent(self, orderEvent):
        if orderEvent.Status == 3:
            self.Debug(f"Purchased Stock: {orderEvent.Symbol}")
