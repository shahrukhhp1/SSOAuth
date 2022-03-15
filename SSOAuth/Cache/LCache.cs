using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Cache
{
    public sealed class LCache
    {
        private static volatile LCache instance; //  Locks var until assignment is complete for double safety
        private static MemoryCache memoryCache;
        private static object syncRoot = new Object();
        private static string settingMemoryCacheName;
        private static double settingCacheExpirationTimeInMinutes;
        private LCache() { }

        /// <summary>
        /// Singleton Cache Instance
        /// </summary>
        public static LCache Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            InitializeInstance();

                        }
                    }
                }
                return instance;
            }
        }

        private static void InitializeInstance()
        {
            settingMemoryCacheName = "Test";
            if (settingMemoryCacheName == null)
                throw new Exception("Please enter a name for the cache in app.config, under 'MemoryCacheName'");

            settingCacheExpirationTimeInMinutes = 500;

            instance = new LCache();
            memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        /// <summary>
        /// Writes Key Value Pair to Cache
        /// </summary>
        /// <param name="Key">Key to associate Value with in Cache</param>
        /// <param name="Value">Value to be stored in Cache associated with Key</param>
        public void Write(string Key, object Value)
        {
            memoryCache.Set(Key, Value, DateTimeOffset.Now.AddMinutes(settingCacheExpirationTimeInMinutes));
        }

        /// <summary>
        /// Returns Value stored in Cache
        /// </summary>
        /// <param name="Key"></param>
        /// <returns>Value stored in cache</returns>
        public object Read(string Key)
        {
            return memoryCache.Get(Key);
        }

        /// <summary>
        /// Returns Value stored in Cache, null if non existent
        /// </summary>
        /// <param name="Key"></param>
        /// <returns>Value stored in cache</returns>
        public object TryRead(string Key)
        {
            try
            {
                return memoryCache.Get(Key);
            }
            catch (Exception)
            {
                return null;
            }

        }

    }
}
