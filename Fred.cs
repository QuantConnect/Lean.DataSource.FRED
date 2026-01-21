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
using QuantConnect.Data.UniverseSelection;
using QuantConnect.Configuration;

namespace QuantConnect.DataSource
{
    public partial class Fred : BaseData
    {
        private static string _authCode = "";
        private readonly string _endpoint = "https://www.quantconnect.com/api/v2/proxy/stlouisfed/";

        /// <summary>
        /// Data source ID
        /// </summary>
        public static int DataSourceId { get; } = 2010;

        /// <summary>
        /// Flag indicating whether or not the FRED auth code has been set yet
        /// </summary>
        public static bool IsAuthCodeSet
        {
            get;
            private set;
        }

        /// <summary>
        /// Static constructor for the <see cref="Fred"/>
        /// </summary>
        static Fred()
        {
            // Se the authentication code in FRED if it's set in Config
            var potentialAuthenticationCode = Config.Get("fred-auth-token");
            if (!string.IsNullOrEmpty(potentialAuthenticationCode))
            {
                SetAuthCode(potentialAuthenticationCode);
            }
        }

        /// <summary>
        /// Default <see cref="Fred"/> constructor
        /// </summary>
        public Fred()
        {
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
            var source = $"{_endpoint}/fred/series/observations?file_type=json&observation_start=1998-01-01&api_key={_authCode}&series_id={config.Symbol.Value.ToLowerInvariant()}";
            return new SubscriptionDataSource(source, SubscriptionTransportMedium.RemoteFile, FileFormat.UnfoldingCollection);
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
            if (!IsAuthCodeSet)
            {
                throw new ArgumentNullException("The authentication code has not been set yet. " +
                    "Please, set your FRED authentication code first, before requesting data.");
            }

            var json = JsonConvert.DeserializeObject<FredApi>(line);
            var entries = json.Observations.Where(x => x.Value != ".").Select(x =>
            {
                return new Fred
                {
                    Symbol = config.Symbol,
                    Value = x.Value.ToDecimal(),
                    Time = x.Date,
                    EndTime = x.Date + TimeSpan.FromDays(1)
                };
            });

            return new BaseDataCollection(date, config.Symbol, entries);
        }

        /// <summary>
        /// Set the FRED authentication code to request the data.
        /// </summary>
        /// <param name="authCode"></param>
        public static void SetAuthCode(string authCode)
        {
            if (string.IsNullOrWhiteSpace(authCode)) return;

            _authCode = authCode;
            IsAuthCodeSet = true;
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
    }
}
