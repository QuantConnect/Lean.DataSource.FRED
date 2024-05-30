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
    def initialize(self): 
        self.set_start_date(2021, 1, 1)
        self.set_end_date(2021, 7, 1)

        number_of_points = {"JPINTDDMEJPY" : 180, "TRINTDEXR" : 180, "CBETHUSD" : 179, "VXGOGCLS" : 126, "DCPN3M" : 114, "BAMLEMPTPRVICRPITRIV" : 129}
        self.symbols = {}

        Fred.set_auth_code("your_authentication_code")
        self.add_equity("SPY", Resolution.DAILY)
        for ticker in number_of_points.keys():
            self.symbols[ticker] = self.add_data(Fred, ticker, Resolution.DAILY).symbol

            historical_data_count = len(self.history(Fred, self.symbols[ticker], 180, Resolution.DAILY))
            if historical_data_count < number_of_points[ticker]:
                raise Exception(f"We expected more than {number_of_points[ticker]} points for {ticker} FRED symbol, but just {historical_data_count} points were obtained.");

            self.debug(f"We got {historical_data_count} items from our history request for {ticker} FRED data")

        self.greatest_value = -1;

    def on_data(self, data):
        data = data.get(Fred)

        if len(data.Values) != 0 and (not self.portfolio.invested):
            self.debug(str(data))

            symbols_with_data = len(data.Values)
            greatest_value = max(list(map(lambda x: x.Value, data.Values)))

            if symbols_with_data >= 5 and self.greatest_value != -1 and greatest_value > self.greatest_value:
                self.set_holdings("SPY", 1)
            else:
                self.set_holdings("SPY", -1)

            self.greatest_value = greatest_value

    def on_order_event(self, order_event):
        if order_event.status == 3:
            self.debug(f"Purchased Stock: {order_event.symbol}")
