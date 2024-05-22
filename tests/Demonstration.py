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

        self.AddEquity("SPY", Resolution.Daily)
        self.fredSymbol = self.AddData(Fred, "JPINTDDMEJPY", Resolution.Daily).Symbol

        self.lastValue = -1;
        history = self.History(Fred, self.fredSymbol, 10, Resolution.Daily)
        self.Debug(f"We got {len(history)} items from our history request for JPINTDDMEJPY FRED data")

    def OnData(self, data):
        data = data.Get(Fred)

        if len(data.Values) != 0:
            self.Debug(str(data[self.fredSymbol]))

            if self.lastValue == -1 and data[self.fredSymbol].Value > self.lastValue:
                self.SetHoldings("SPY", 1)
            else:
                self.SetHoldings("SPY", -1)

            _lastValue = data[self.fredSymbol].Value

    def OnOrderEvent(self, orderEvent):
        if orderEvent.Status == 3:
            self.Debug(f"Purchased Stock: {orderEvent.Symbol}")
