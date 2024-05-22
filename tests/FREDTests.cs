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

using System;
using ProtoBuf;
using System.IO;
using System.Linq;
using ProtoBuf.Meta;
using Newtonsoft.Json;
using NUnit.Framework;
using QuantConnect.Data;
using QuantConnect.DataSource;
using Python.Runtime;

namespace QuantConnect.DataLibrary.Tests
{
    [TestFixture]
    public class FREDTests
    {
        private const string _jsonResponse = @"{
    ""realtime_start"": ""2024-05-22"",
    ""realtime_end"": ""2024-05-22"",
    ""observation_start"": ""1998-01-01"",
    ""observation_end"": ""9999-12-31"",
    ""units"": ""lin"",
    ""output_type"": 1,
    ""file_type"": ""json"",
    ""order_by"": ""observation_date"",
    ""sort_order"": ""asc"",
    ""count"": 9587,
    ""offset"": 0,
    ""limit"": 100000,
    ""observations"": [
        {
            ""realtime_start"": ""2024-05-22"",
            ""realtime_end"": ""2024-05-22"",
            ""date"": ""1998-01-01"",
            ""value"": ""64.0""
        }
    ]}";

        [Test]
        public void JsonRoundTrip()
        {
            var expected = CreateNewInstance();
            var type = expected.GetType();
            var serialized = JsonConvert.SerializeObject(expected);
            var result = JsonConvert.DeserializeObject(serialized, type);

            AssertAreEqual(expected, result);
        }

        [Test]
        public void ProtobufRoundTrip()
        {
            var expected = CreateNewInstance();
            var type = expected.GetType();

            RuntimeTypeModel.Default[typeof(BaseData)].AddSubType(2000, type);

            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, expected);

                stream.Position = 0;

                var result = Serializer.Deserialize(type, stream);

                AssertAreEqual(expected, result, filterByCustomAttributes: true);
            }
        }

        [Test]
        public void Clone()
        {
            var expected = CreateNewInstance();
            var result = expected.Clone();

            AssertAreEqual(expected, result);
        }

        [TestCase("JPINTDDMEJPY")]
        [TestCase("USINTDMRKTJPY")]
        [TestCase("TRINTDEXR")]
        [TestCase("DTWEXM")]
        [TestCase("CBETHUSD")]
        public void GetsCorrectValue(string ticker)
        {
            var date = new DateTime(2018, 7, 7);

            var newInstance = new Fred();

            var symbol = Symbol.Create(ticker, 0, "empty");
            var config = new SubscriptionDataConfig(typeof(Fred), symbol,
                Resolution.Daily, TimeZones.Utc, TimeZones.Utc, true, true, false, true);

            var data = newInstance.Reader(config, _jsonResponse, new DateTime(1998, 1, 1), false);
            Assert.AreEqual(64.0, data.Value);
        }

        [TestCase("JPINTDDMEJPY")]
        [TestCase("USINTDMRKTJPY")]
        [TestCase("TRINTDEXR")]
        [TestCase("DTWEXM")]
        [TestCase("CBETHUSD")]
        public void GetsCorrectValueInPython(string ticker)
        {
            PythonEngine.Initialize();
            dynamic instance;
            using (Py.GIL())
            {
                PyObject test = PyModule.FromString("testModule",
                    @"
from QuantConnect.DataSource import *

class Test(Fred):
    def __init__(self):
        super().__init__()").GetAttr("Test");
                instance = test.CreateType().GetBaseDataInstance();
            }

            var symbol = Symbol.Create(ticker, 0, "empty");
            var config = new SubscriptionDataConfig(typeof(Fred), symbol,
                Resolution.Daily, TimeZones.Utc, TimeZones.Utc, true, true, false, true);

            var data = instance.Reader(config, _jsonResponse, new DateTime(1998, 1, 1), false);
            Assert.AreEqual(64.0, data.Value);
        }

        private void AssertAreEqual(object expected, object result, bool filterByCustomAttributes = false)
        {
            foreach (var propertyInfo in expected.GetType().GetProperties())
            {
                // we skip Symbol which isn't protobuffed
                if (filterByCustomAttributes && propertyInfo.CustomAttributes.Count() != 0)
                {
                    Assert.AreEqual(propertyInfo.GetValue(expected), propertyInfo.GetValue(result));
                }
            }
            foreach (var fieldInfo in expected.GetType().GetFields())
            {
                Assert.AreEqual(fieldInfo.GetValue(expected), fieldInfo.GetValue(result));
            }
        }

        private BaseData CreateNewInstance()
        {
            return new Fred
            {
                Symbol = Symbol.Empty,
                Time = DateTime.Today,
                DataType = MarketDataType.Base,
                Value = 0.1m
            };
        }
    }
}
