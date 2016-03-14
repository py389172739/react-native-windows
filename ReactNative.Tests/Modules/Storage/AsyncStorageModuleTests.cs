﻿using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json.Linq;
using ReactNative.Modules.Storage;
using System.Collections.Generic;
using System.Threading;

namespace ReactNative.Tests.Modules.Storage
{
    [TestClass]
    public class AsyncStorageModuleTests
    {
        [TestMethod]
        public void AsyncStorageModule_InvalidKeyValue_Method()
        {
            var module = new AsyncStorageModule();
            var waitHandle = new AutoResetEvent(false);

            var result = new JArray();

            var callback = new MockCallback( res => { result = (JArray)res[0]; waitHandle.Set(); });

            var array = new JArray
            {
                new JArray
                {
                    5,
                    5,
                },
            };

            module.multiSet(array, callback);
            waitHandle.WaitOne();

            Assert.AreEqual((result[0]).First.Value<string>(), "Invalid key");

            array = new JArray
            {
                new JArray
                {
                    5,
                }
            };

            module.multiSet(array, callback);
            waitHandle.WaitOne();

            Assert.AreEqual((result[0]).First.Value<string>(), "Invalid key value pair");

            array = new JArray
            {
                new JArray
                {
                    5,
                    5,
                    5,
                }
            };

            module.multiSet(array, callback);
            waitHandle.WaitOne();

            Assert.AreEqual((result[0]).First.Value<string>(), "Invalid key value pair");
        }

        [TestMethod]
        public void AsyncStorageModule_Datatypes_Method()
        {
            var module = new AsyncStorageModule();
            var waitHandle = new AutoResetEvent(false);

            var result = new JArray();

            var setCallback = new MockCallback( _ => waitHandle.Set() );
            var getCallback = new MockCallback(res => { result = (JArray)res[0]; waitHandle.Set(); });

            var obj = new JObject();
            obj.Add("ABC", "AQE");
            obj.Add("DEF", "DFG");

            var constr = new JConstructor("ABC", "GHJ");

            var array = new JArray
            {
                new JArray
                {
                    "1",
                    null,
                },
                new JArray
                {
                    "2",
                    true,
                },
                new JArray
                {
                    "3",
                    555,
                },
                new JArray
                {
                    "4",
                    999.999,
                },
                new JArray
                {
                    "5",
                    "Test string",
                },
                new JArray
                {
                    "6",
                    JToken.FromObject(new System.DateTime(2016, 1, 1, 12, 13, 14)),
                },
                new JArray
                {
                    "7",
                    JToken.FromObject(new System.Uri("http://dev.windows.com")),
                },
                new JArray
                {
                    "8",
                    JToken.FromObject(new System.TimeSpan(1, 1, 1)),
                },
                 new JArray
                {
                    "9",
                    JToken.FromObject(new System.Guid("936DA01F-9ABD-4d9d-80C7-02AF85C822A8")),
                },
                new JArray
                {
                    "10",
                    obj,
                },
                new JArray
                {
                    "11",
                    new JArray
                    {
                        1,
                        2,
                        obj,
                    },
                },
                new JArray
                {
                    "12",
                    new JArray
                    {
                        "abc",
                        "def",
                        5,
                        6,
                    },
                },
                new JArray
                {
                    "13",
                    new JRaw(560),
                },
                new JArray
                {
                    "14",
                    new JRaw("ABC"),
                },
                new JArray
                {
                    "15",
                    new JRaw(5.5),
                },
                new JArray
                {
                    "16",
                    constr,
                },
            };

            module.clear(setCallback);
            waitHandle.WaitOne();

            module.multiSet(array, setCallback);
            waitHandle.WaitOne();

            var keys = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16" };
            
            module.multiGet(keys, getCallback);
            waitHandle.WaitOne();

            AssertJArraysAreEqual(array, result); 
        }

        [TestMethod]
        public void AsyncStorageModule_multiGet_Method()
        {
            var module = new AsyncStorageModule();
            var waitHandle = new AutoResetEvent(false);

            var result = new JArray();

            var emptyCallback = new MockCallback(_ => waitHandle.Set());
            var callback = new MockCallback(res => { result = (JArray)res[0]; waitHandle.Set(); });

            var array = new JArray
            {
                new JArray
                {
                    "test1",
                    5,
                }
            };

            module.multiSet(array, callback);
            waitHandle.WaitOne();

            module.multiGet(new string[] { "test1", }, callback);
            waitHandle.WaitOne();

            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual((result[0]).Last.Value<long>(), 5);
        }

        [TestMethod]
        public void AsyncStorageModule_multiRemove_Method()
        {
            var module = new AsyncStorageModule();
            var waitHandle = new AutoResetEvent(false);

            var result = new JArray();

            var emptyCallback = new MockCallback(_ => waitHandle.Set());
            var callback = new MockCallback(res => { result = (JArray)res[0]; waitHandle.Set(); });

            var array = new JArray
            {
                new JArray
                {
                    "test1",
                    5,
                },
                new JArray
                {
                    "test2",
                    10.5,
                },
                new JArray
                {
                    "test3",
                    new JArray
                    {
                        new JArray
                        {
                            1,
                            false,
                            "ABCDEF",
                        },
                        30,
                        40,
                    },
                },
                new JArray
                {
                    "test4",
                    true,
                },
                new JArray
                {
                    "test5",
                    "ABCDEF",
                },
                new JArray
                {
                    "test6",
                    JValue.CreateNull(),
                }
            };

            module.clear(emptyCallback);
            waitHandle.WaitOne();

            module.multiSet(array, callback);
            waitHandle.WaitOne();

            module.getAllKeys(callback);
            waitHandle.WaitOne();

            Assert.AreEqual(result.Count, 6);

            var strArray = new string[result.Count];
            int idx = 0;
            foreach (var item in result)
            {
                strArray[idx++] = item.Value<string>();
            }

            module.multiGet(strArray, callback);
            waitHandle.WaitOne();

            AssertJArraysAreEqual(result, array);
          
            var keys = new string[] 
            {
                "test1",
                "test2",
            };
            module.multiRemove(keys, callback);
            waitHandle.WaitOne();

            module.getAllKeys(callback);
            waitHandle.WaitOne();

            Assert.AreEqual(result.Count, 4);
        }

        /// **************
        /// Android tests
        /// **************

        [TestMethod]
        public void AsyncStorageModule_Android_testMultiSetMultiGet()
        {
            var mStorage = new AsyncStorageModule();
            var waitHandle = new AutoResetEvent(false);

            var result = new JArray();
            var setCallback = new MockCallback(_ => waitHandle.Set());
            var getCallback = new MockCallback(res => { result = (JArray)res[0]; waitHandle.Set(); });

            mStorage.clear(setCallback);
            waitHandle.WaitOne();


            string key1 = "foo1";
            string key2 = "foo2";
            string fakeKey = "fakeKey";
            string value1 = "bar1";
            string value2 = "bar2";

            var keyValues = new JArray();
            keyValues.Add(new JArray { key1, value1 });
            keyValues.Add(new JArray { key2, value2 });
            
            mStorage.multiSet(keyValues, setCallback);
            waitHandle.WaitOne();

            var keys = new List<string>();
            keys.Add(key1);
            keys.Add(key2);

            mStorage.multiGet(keys.ToArray(), getCallback);
            waitHandle.WaitOne();

            Assert.IsTrue(JToken.DeepEquals(result, keyValues));

            keys.Add(fakeKey);
            keyValues.Add(new JArray { fakeKey, null });

            mStorage.multiGet(keys.ToArray(), getCallback);
            waitHandle.WaitOne();

            Assert.IsTrue(JToken.DeepEquals(result, keyValues));
        }

        [TestMethod]
        public void AsyncStorageModule_Android_testMultiRemove()
        {
            var mStorage = new AsyncStorageModule();
            var waitHandle = new AutoResetEvent(false);

            var result = new JArray();
            var setCallback = new MockCallback(_ => waitHandle.Set());
            var getCallback = new MockCallback(res => { result = (JArray)res[0]; waitHandle.Set(); });

            mStorage.clear(setCallback);
            waitHandle.WaitOne();

            string key1 = "foo1";
            string key2 = "foo2";
            string value1 = "bar1";
            string value2 = "bar2";

            var keyValues = new JArray();
            keyValues.Add(new JArray { key1, value1 });
            keyValues.Add(new JArray { key2, value2 });

            mStorage.multiSet(keyValues, setCallback);
            waitHandle.WaitOne();

            var keys = new List<string>();
            keys.Add(key1);
            keys.Add(key2);

            mStorage.multiRemove(keys.ToArray(), setCallback);
            waitHandle.WaitOne();

            mStorage.getAllKeys(getCallback);
            waitHandle.WaitOne();

            Assert.AreEqual(result.Count, 0);

            mStorage.multiSet(keyValues, setCallback);
            waitHandle.WaitOne();

            keys.Add("fakeKey");
            mStorage.multiRemove(keys.ToArray(), setCallback);
            waitHandle.WaitOne();

            mStorage.getAllKeys(getCallback);
            waitHandle.WaitOne();

            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void AsyncStorageModule_Android_testMultiMerge()
        {
            var mStorage = new AsyncStorageModule();
            var waitHandle = new AutoResetEvent(false);

            var result = new JArray();
            var setCallback = new MockCallback(_ => waitHandle.Set());
            var getCallback = new MockCallback(res => { result = (JArray)res[0]; waitHandle.Set(); });

            mStorage.clear(setCallback);
            waitHandle.WaitOne();

            string mergeKey = "mergeTest";

            var value = new JObject();
            value.Add("foo1", "bar1");

            value.Add("foo2", new JArray
            {
                "val1",
                "val2",
                3,
            });

            value.Add("foo3", 1001);

            var val = new JObject();
            val.Add("key1", "randomValueThatWillNeverBeUsed");
            value.Add("foo4", val);

            var array = new JArray
            {
                new JArray
                {
                    mergeKey,

                    value,
                },
            };

            mStorage.multiSet(array, setCallback);
            waitHandle.WaitOne();

            var str = new string[] { mergeKey };

            mStorage.multiGet(str, getCallback);
            waitHandle.WaitOne();

            Assert.IsTrue(JToken.DeepEquals(result, array));

            value.Remove("foo1");
            value.Remove("foo2");
            value.Remove("foo3");
            value.Remove("foo4");

            value.Add("foo1", 1001);

            var val2 = new JObject();
            val2.Add("key1", "val1");
            value.Add("foo2", val2);

            value.Add("foo3", "bar1");

            value.Add("foo4", new JArray
            {
                "val1",
                "val2",
                3
            });

            var newValue = new JObject();
            var val3 = new JObject();
            val3.Add("key2", "val2");
            newValue.Add("foo2", val3);

            var newValue2 = new JObject();
            var val4 = new JObject();
            val4.Add("key1", "val3");
            newValue2.Add("foo2", val4);

            var array2 = new JArray
            {
                new JArray
                {
                    mergeKey,
                    value,
                },
            };

            mStorage.multiMerge(array2, setCallback);
            waitHandle.WaitOne();

            var array3 = new JArray
            {
                new JArray
                {
                    mergeKey,
                    newValue,
                },
            };

            mStorage.multiMerge(array3, setCallback);
            waitHandle.WaitOne();

            var array4 = new JArray
            {
                new JArray
                {
                    mergeKey,
                    newValue2,
                },
            };

            mStorage.multiMerge(array4, setCallback);
            waitHandle.WaitOne();

            value.Remove("foo2");
            var val5 = new JObject();
            val5.Add("key1", "val3");
            val5.Add("key2", "val2");
            value.Add("foo2", val5);

            mStorage.multiGet(str, getCallback);
            waitHandle.WaitOne();

            Assert.IsTrue(JToken.DeepEquals(value, result.Last.Value<JArray>().Last.Value<JObject>()));
        }

        [TestMethod]
        public void AsyncStorageModule_Android_testGetAllKeys()
        {
            var mStorage = new AsyncStorageModule();
            var waitHandle = new AutoResetEvent(false);

            var result = new JArray();
            var setCallback = new MockCallback(_ => waitHandle.Set());
            var getCallback = new MockCallback(res => { result = (JArray)res[0]; waitHandle.Set(); });

            mStorage.clear(setCallback);
            waitHandle.WaitOne();

            string[] keys = { "foo", "foo2" };
            string[] values = { "bar", "bar2" };

            var keyValues = new JArray
            {
                new JArray
                {
                    keys[0],
                    values[0],
                },
                new JArray
                {
                    keys[1],
                    values[1],
                },
            };

            mStorage.multiSet(keyValues, setCallback);
            waitHandle.WaitOne();

            mStorage.getAllKeys(getCallback);
            waitHandle.WaitOne();

            var storedKeys = new JArray
            {
                keys[0],
                keys[1],
            };

            var set = new SortedSet<string>();
            IEnumerable<string> enumerator = storedKeys.Values<string>();

            foreach (var value in enumerator)
            {
                set.Add(value);
            }

            set.SymmetricExceptWith(result.Values<string>());
            Assert.AreEqual(set.Count, 0);

            mStorage.multiRemove(keys, getCallback);
            waitHandle.WaitOne();

            mStorage.getAllKeys(getCallback);
            waitHandle.WaitOne();

            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void AsyncStorageModule_Android_testClear()
        {
            var mStorage = new AsyncStorageModule();
            var waitHandle = new AutoResetEvent(false);

            var result = new JArray();
            var setCallback = new MockCallback(_ => waitHandle.Set());
            var getCallback = new MockCallback(res => { result = (JArray)res[0]; waitHandle.Set(); });

            mStorage.clear(setCallback);
            waitHandle.WaitOne();

            string[] keys = { "foo", "foo2" };
            string[] values = { "bar", "bar2" };

            var keyValues = new JArray
            {
                new JArray
                {
                    keys[0],
                    values[0],
                },
                new JArray
                {
                    keys[1],
                    values[1],
                },
            };

            mStorage.multiSet(keyValues, setCallback);
            waitHandle.WaitOne();

            mStorage.clear(setCallback);
            waitHandle.WaitOne();

            mStorage.getAllKeys(getCallback);
            waitHandle.WaitOne();

            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void AsyncStorageModule_Android_testHugeMultiGetMultiGet()
        {
            var mStorage = new AsyncStorageModule();
            var waitHandle = new AutoResetEvent(false);

            var result = new JArray();
            var setCallback = new MockCallback(_ => waitHandle.Set());
            var getCallback = new MockCallback(res => { result = (JArray)res[0]; waitHandle.Set(); });

            mStorage.clear(setCallback);
            waitHandle.WaitOne();

            // Limitation on Android - not a limitation on Windows
            // Test with many keys, so that it's above the 999 limit per batch imposed by SQLite.
            int keyCount = 1001;
            // don't set keys that divide by this magical number, so that we can check that multiGet works,
            // and returns null for missing keys
            int magicalNumber = 343;

            var keyValues = new JArray();
            for (int i = 0; i < keyCount; i++)
            {
                if (i % magicalNumber > 0)
                {
                    var key = "key" + i;
                    var value = "value" + i;
                    keyValues.Add(new JArray
                    {
                        key,
                        value,
                    });
                }
            }
            mStorage.multiSet(keyValues, setCallback);
            waitHandle.WaitOne();

            var keys = new List<string>();
            for (int i = 0; i < keyCount; i++)
            {
                keys.Add("key" + i);
            }

            mStorage.multiGet(keys.ToArray(), getCallback);
            waitHandle.WaitOne();

            Assert.AreEqual(result.Count, keys.Count);

            var keyReceived = new bool[keyCount];

            for (int i = 0; i < keyCount; i++)
            {
                var keyValue = result[i];
                var key = keyValue.Value<JArray>().First.Value<string>().Substring(3);

                int idx = System.Int32.Parse(key);
                Assert.IsFalse(keyReceived[idx]);
                keyReceived[idx] = true;

                if (idx % magicalNumber > 0)
                {
                    var value = keyValue.Value<JArray>().Last.Value<string>().Substring(5);
                    Assert.AreEqual(key, value);
                }
                else
                {
                    Assert.IsTrue(keyValue.Value<JArray>().Last.Type == JTokenType.Null);
                }   
            }

            var keyRemoves = new List<string>();
            for (int i = 0; i < keyCount; i++)
            {
                if (i % 2 > 0)
                {
                    keyRemoves.Add("key" + i);
                }
            }

            mStorage.multiRemove(keyRemoves.ToArray(), setCallback);
            waitHandle.WaitOne();

            mStorage.getAllKeys(getCallback);
            waitHandle.WaitOne();

            Assert.AreEqual(result.Count, 499);
            for (int i = 0; i < result.Count; i++)
            {
                var key = result[i].Value<string>().Substring(3); ;
                int idx = System.Int32.Parse(key);
                Assert.AreEqual(idx % 2,0);
            }
        }

        private void AssertJArraysAreEqual(JArray a, JArray b)
        {
            foreach (var item in a)
            {
                string key = item.First.Value<string>();
                object value = item.Last.Value<object>();

                var found = false;
                var item_ = b.First;

                while (!found && item_ != null)
                {
                    if ((item_.First).Value<string>().CompareTo(key) == 0)
                    {
                        object o = item_.Last.Value<object>();
                        if (value == null) Assert.IsNull(o);
                        else if (o.GetType() != value.GetType()) Assert.Fail();
                        else
                        {
                            if (value is bool || value is long || value is double || value is System.Guid || 
                                value is System.TimeSpan || value is System.Uri || value is System.DateTime)
                            {
                                Assert.AreEqual(value, o);
                            }
                            else if (value is string)
                            {
                                Assert.IsTrue((value as string).CompareTo(o as string) == 0);
                            }
                            else if (value is JArray || value is JRaw || value is JConstructor)
                            {
                                Assert.IsTrue(value.ToString().CompareTo(o.ToString()) == 0);
                            }
                            else if (value is JObject)
                            {
                                Assert.IsTrue(JToken.DeepEquals(value as JObject, o as JObject));
                            }
                        }
                        found = true;
                    }
                    item_ = item_.Next;
                }
                Assert.IsTrue(found);
            }
        }
    }
}