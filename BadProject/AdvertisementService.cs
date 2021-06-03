using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Caching;
using System.Threading;
using System.Linq;
using ThirdParty;

namespace Adv
{
    public class AdvertisementService
    {
        private static MemoryCache _cache = new MemoryCache("AdvertMemoryCache");
        private static List<Errors> _errors = new List<Errors>();
        private static readonly Object lockObj = new Object();
        private IThirdPartyService _thirdPartyService = null;       

        private int _retryCount = 0;

        public AdvertisementService(int retryCount, IThirdPartyService thirdPartyService)
        {
            if (retryCount == 0)
            {
                if (!int.TryParse(ConfigurationManager.AppSettings["RetryCount"],out _retryCount))
                {
                    throw new ArgumentNullException("RetryCount", "Not found in config file");
                }
            }
            else
                _retryCount = retryCount;

            if (thirdPartyService != null)
                _thirdPartyService = thirdPartyService;
            else
                throw new ArgumentNullException("IThirdPartyService", "Is null");
        }
        public static void SetMemoryCache(MemoryCache cache)
        {
            _cache = cache;
        }
        public static void SetErros(List<Errors> errors)
        {
            _errors = errors;
        }


        // **************************************************************************************************
        // Loads Advertisement information by id
        // from cache or if not possible uses the "mainProvider" or if not possible uses the "backupProvider"
        // **************************************************************************************************
        // Detailed Logic:
        // 
        // 1. Tries to use cache (and retuns the data or goes to STEP2)
        //
        // 2. If the cache is empty it uses the NoSqlDataProvider (mainProvider), 
        //    in case of an error it retries it as many times as needed based on AppSettings
        //    (returns the data if possible or goes to STEP3)
        //
        // 3. If it can't retrive the data or the ErrorCount in the last hour is more than 10, 
        //    it uses the SqlDataProvider (backupProvider)
        public Advertisement GetAdvertisement(string id)
        {
            Advertisement adv = null;
            if (id != null)
            {
                adv = _cache.Get("AdvKey_" +  id,null) as Advertisement;

                if (adv == null)
                {
                    // errors in the last hour
                    var errorCount = (_errors.AsEnumerable()).Where(x => x.ErrDate > DateTime.Now.AddMinutes(-60)).Count();

                    // Cache is empty and ErrorCount < 10 then use HTTP provider
                    if (errorCount < 10)
                    {
                        int retry = 0;
                        do
                        {
                            retry++;
                            try
                            {
                                adv = _thirdPartyService.NoSqlGetAdv(id);
                            }
                            catch (Exception ex)
                            {
                                Thread.Sleep(1000);
                                _errors.Add(new Errors(DateTime.Now, ex.Message)); // Store HTTP error timestamp              
                            }
                        } while ((adv == null) && (retry < _retryCount));

                        // update cache if found in http provider
                        if (adv != null)
                        {
                            lock (lockObj)
                            {
                                _cache.Set($"AdvKey_" + id, adv, DateTimeOffset.Now.AddMinutes(5));
                            }
                        }

                    }

                    // if needed try to use Backup provider
                    if (adv == null)
                    {
                        // can check the cache again too
                        adv = MemoryCache.Default.Get("AdvKey_" + id, null) as Advertisement;
                        if (adv == null)
                        {
                            adv = _thirdPartyService.SqlGetAdv(id);

                            if (adv != null)
                            {
                                lock (lockObj)
                                {
                                    _cache.Set($"AdvKey_" + id, adv, DateTimeOffset.Now.AddMinutes(5));
                                }
                            }
                        }
                    }
                }
            }
            return adv;
        }
    }
}
