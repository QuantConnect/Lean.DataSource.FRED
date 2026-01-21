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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace QuantConnect.DataSource
{
    public partial class Fred
    {
        private class FredApi
        {
            [JsonProperty("realtime_start")]
            public string RealtimeStart { get; set; }

            [JsonProperty("realtime_end")]
            public string RealtimeEnd { get; set; }

            [JsonProperty("observation_start")]
            public string ObservationStart { get; set; }

            [JsonProperty("observation_end")]
            public string ObservationEnd { get; set; }

            [JsonProperty("units")]
            public string Units { get; set; }

            [JsonProperty("output_type")]
            public int OutputType { get; set; }

            [JsonProperty("file_type")]
            public string FileType { get; set; }

            [JsonProperty("order_by")]
            public string OrderBy { get; set; }

            [JsonProperty("sort_order")]
            public string SortOrder { get; set; }

            [JsonProperty("count")]
            public int Count { get; set; }

            [JsonProperty("offset")]
            public int Offset { get; set; }

            [JsonProperty("limit")]
            public int Limit { get; set; }

            [JsonProperty("observations")]
            public IList<Observation> Observations { get; set; }

            public class Observation
            {
                [JsonProperty("realtime_start")]
                public string RealtimeStart { get; set; }

                [JsonProperty("realtime_end")]
                public string RealtimeEnd { get; set; }

                [JsonProperty("date")]
                public DateTime Date { get; set; }

                [JsonProperty("value")]
                public string Value { get; set; }
            }
        }
    }
}
