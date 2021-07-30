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
from QuantConnect.DataSource import *

class CachedFREDDataAlgorithm(QCAlgorithm):

    def Initialize(self):
        self.SetStartDate(2003, 1, 1)
        self.SetEndDate(2019, 10, 11)
        self.SetCash(100000)

        # QuantConnect caches a small subset of alternative data for easy consumption for the community.
        # You can use this in your algorithm as demonstrated below:

        # FRED data
        self.fredPeakToTrough = self.AddData(Fred, Fred.OECDRecessionIndicators.UnitedStatesFromPeakThroughTheTrough, Resolution.Daily).Symbol

    def OnData(self, data):
        if data.ContainsKey(self.fredPeakToTrough):
            peakToTrough = data.Get(Fred, self.fredPeakToTrough)
            self.Log(f"OECD based Recession Indicator for the United States from the Peak through the Trough: {peakToTrough}")

