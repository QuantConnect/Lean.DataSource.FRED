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

using QuantConnect.Data;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace QuantConnect.DataSource
{
    public partial class Fred : BaseData
    {
        private static string _apiKey = "";

        /// <summary>
        /// Data source ID
        /// </summary>
        public static int DataSourceId { get; } = 2010;

        /// <summary>
        /// Flag indicating whether or not the FRED API key has been set yet
        /// </summary>
        public static bool IsAPIKeySet
        {
            get;
            private set;
        }

        /// <summary>
        /// Return the URL string source of the file. This will be converted to a stream
        /// </summary>
        /// <param name="config">Configuration object</param>
        /// <param name="date">Date of this source file</param>
        /// <param name="isLiveMode">true if we're in live mode, false for backtesting mode</param>
        /// <returns>
        /// String URL of source file.
        /// </returns>
        public override SubscriptionDataSource GetSource(SubscriptionDataConfig config, DateTime date, bool isLiveMode)
        {
            var source = $"https://api.stlouisfed.org/fred/series/observations?file_type=json&observation_start=1998-01-01&api_key={_apiKey}&series_id={config.Symbol.Value.ToLowerInvariant()}";
            return new SubscriptionDataSource(source, SubscriptionTransportMedium.Rest);
        }

        /// <summary>
        /// Parses the data from the line provided and loads it into LEAN
        /// </summary>
        /// <param name="config">Subscription configuration</param>
        /// <param name="line">Line of data</param>
        /// <param name="date">Date</param>
        /// <param name="isLiveMode">Is live mode</param>
        /// <returns>New instance of FRED data</returns>
        public override BaseData Reader(SubscriptionDataConfig config, string line, DateTime date, bool isLiveMode)
        {
            var json = JsonConvert.DeserializeObject<DataResponse>(line);

            var requestedEntry = json.Observations.Where(x => x.Date == date.Date).Single();

            return new Fred
            {
                Symbol = config.Symbol,
                Value = requestedEntry.Value,
                Time = requestedEntry.Date,
                EndTime = requestedEntry.Date + TimeSpan.FromDays(1)
            };
        }

        /// <summary>
        /// Set the FRED API key to request the data.
        /// </summary>
        /// <param name="apiKey"></param>
        public static void SetAPIKey(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) return;

            _apiKey = apiKey;
            IsAPIKeySet = true;
        }

        /// <summary>
        /// Clones the data
        /// </summary>
        /// <returns>A clone of the object</returns>
        public override BaseData Clone()
        {
            return new Fred()
            {
                Symbol = Symbol,
                Value = Value,
                Time = Time,
                EndTime = EndTime
            };
        }

        /// <summary>
        /// Indicates whether the data source is tied
        /// to an underlying symbol and requires that corporate
        /// events be applied to it as well, such as renames and delistings
        /// </summary>
        /// <returns>false</returns>
        public override bool RequiresMapping()
        {
            return false;
        }

        /// <summary>
        /// Indicates whether the data is sparse.
        /// If true, we disable logging for missing files
        /// </summary>
        /// <returns>true</returns>
        public override bool IsSparseData()
        {
            return true;
        }

        /// <summary>
        /// Converts the instance to string
        /// </summary>
        public override string ToString()
        {
            return $"{Symbol} - {Value}";
        }

        /// <summary>
        /// Gets the default resolution for this data and security type
        /// </summary>
        public override Resolution DefaultResolution()
        {
            return Resolution.Daily;
        }

        /// <summary>
        /// Gets the supported resolution for this data and security type
        /// </summary>
        public override List<Resolution> SupportedResolutions()
        {
            return DailyResolution;
        }

        /// <summary>
        /// Class containing part of the response from FRED API when requesting data for a symbol
        /// </summary>
        private class DataResponse
        {
            [JsonProperty(PropertyName = "observations")]
            public List<DataEntry> Observations { get; set; }
        }

        /// <summary>
        /// Class for wrapping each data entry in the response from FRED API when requesting data for a symbol
        /// </summary>
        private class DataEntry
        {
            [JsonProperty(PropertyName = "realtime_start")]
            public DateTime RealTimeStart { get; set; }

            [JsonProperty(PropertyName = "realtime_end")]
            public DateTime RealTimeEnd { get; set;}

            [JsonProperty(PropertyName = "date")]
            public DateTime Date { get; set; }

            [JsonProperty(PropertyName = "value")]
            public decimal Value { get; set; }
        }
    }
}
