using HornetEngine.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HornetEngine
{
    public class Config
    {
        private static Config instance = null;
        private static readonly object padlock = new object();

        private JsonRoot jsonRoot;

        /// <summary>
        /// The constructor of the Config class
        /// </summary>
        public Config()
        {
            // Locate the config json
            var resourceName = "resources\\config.json";

            // Initialize the json string
            StreamReader r = new StreamReader(resourceName);
            String configJson = r.ReadToEnd();

            // Convert the json string to the cusom data class
            jsonRoot = JsonConvert.DeserializeObject<JsonRoot>(configJson);
        }

        /// <summary>
        /// A function which can be used to get the json root
        /// </summary>
        /// <returns>A JsonRoot object</returns>
        public JsonRoot GetJsonRoot()
        {
            return jsonRoot;
        }

        /// <summary>
        /// A function which can be used to get the json data
        /// </summary>
        /// <returns>A JsonData object</returns>
        public JsonData GetJsonData()
        {
            return jsonRoot.data;
        }

        /// <summary>
        /// A function which can be used to get the json rules
        /// </summary>
        /// <returns>A List of JsonRule objects</returns>
        public List<JsonRule> GetJsonRules()
        {
            return jsonRoot.rules;
        }

        /// <summary>
        /// A function to get the instance of the Config
        /// The lock ensures that singleton is thread safe
        /// </summary>
        public static Config Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Config();
                    }
                    return instance;
                }
            }
        }
    }
}
