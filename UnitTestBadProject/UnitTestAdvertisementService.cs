using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using Adv;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThirdParty;

namespace UnitTestBadProject
{
    [TestClass]
    public class UnitTestAdvertisementService
    {
        [TestMethod]
        public void AdvertisementServiceConstructor_NUllThirdPartyService_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(()=>new AdvertisementService(0, null));
        }

        [TestMethod]
        public void AdvertisementServiceConstructor_ValidThirdPartyService_DoesNotThrowArgumentNullException()
        {
            try
            {
                var adv = new AdvertisementService(0, new TestThirdPartyService());
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsFalse(true);
            }
        }


        [TestMethod]
        public void GetAdvertisement_NullAdvertId_ReturnsNullAdvert()
        {
            try
            {
                var adv = new AdvertisementService(0, new TestThirdPartyService());
                var advertisement = adv.GetAdvertisement(null);
                Assert.IsTrue(advertisement == null );
            }
            catch
            {
                Assert.IsFalse(true);
            }
        }

        [TestMethod]
        public void GetAdvertisement_DataInCache_CacheRetursAdvert()
        {
            try
            {
                var advertService = new AdvertisementService(0, new TestThirdPartyService());
                MemoryCache _cache = new MemoryCache("AdvertMemoryCache");
                var advert = new Advertisement();
                string adId = "123";
                advert.Description = "123 Cache Advert Description";
                advert.Name = "123 cache Advert";
                advert.WebId = adId;
                _cache.Set($"AdvKey_"+ adId, advert, DateTimeOffset.Now.AddMinutes(5));
                AdvertisementService.SetMemoryCache(_cache);
                var advertisement = advertService.GetAdvertisement(adId);
                Assert.IsTrue(advertisement != null && advertisement.Name == advert.Name);
            }
            catch (Exception ex)
            {
                Assert.IsFalse(true);
            }
        }



        [TestMethod]
        public void GetAdvertisement_DataNotInCacheAndErrorsLessThan10_httpReturnsNoSQLAdvert()
        {
            try
            {
                MemoryCache _cache = new MemoryCache("AdvertMemoryCache"); ;                
                var adv = new AdvertisementService(0, new TestThirdPartyService());
                List<Errors> errors = new List<Errors>();
                AdvertisementService.SetErros(errors);
                AdvertisementService.SetMemoryCache(_cache);
                var advertisement = adv.GetAdvertisement("123");
                Assert.IsTrue(advertisement != null && advertisement.Name == "No SQL Advert");
            }
            catch
            {
                Assert.IsFalse(true);
            }
        }

        [TestMethod]
        public void GetAdvertisement_DataNotInCacheAndErrorsLessThan10_httpReturnsNoSQLAdvertCacheSet()
        {
            try
            {
                MemoryCache _cache = new MemoryCache("AdvertMemoryCache");                
                var adv = new AdvertisementService(0, new TestThirdPartyService());
                List<Errors> errors = new List<Errors>();
                AdvertisementService.SetErros(errors);
                AdvertisementService.SetMemoryCache(_cache);
                var advertisement = adv.GetAdvertisement("123");              
                Assert.IsTrue(((Advertisement)_cache["AdvKey_123"]).WebId == advertisement.WebId);
            }
            catch
            {
                Assert.IsFalse(true);
            }
        }

        [TestMethod]
        public void GetAdvertisement_DataNotInCacheAndErrorsMoreThan10_httpReturnsSQLAdvert()
        {
            try
            {
                MemoryCache _cache = new MemoryCache("AdvertMemoryCache");
                List<Errors> errors = new List<Errors>();
                // 11 errors 
                for (var i = 0; i < 11; i++)
                    errors.Add(new Errors(DateTime.Now, "Error test " + i));
                AdvertisementService.SetErros(errors);
                AdvertisementService.SetMemoryCache(_cache);
                var adv = new AdvertisementService(0, new TestThirdPartyService());
                var advertisement = adv.GetAdvertisement("123");
                Assert.IsTrue(advertisement != null && advertisement.Name == "SQL Advert");
            }
            catch
            {
                Assert.IsFalse(true);
            }
        }

        [TestMethod]
        public void GetAdvertisement_DataNotInCacheAndNoSQLProviderThrowsException_SQLAdvertReturned()
        {
            try
            {
                MemoryCache _cache = new MemoryCache("AdvertMemoryCache");
                List<Errors> errors = new List<Errors>();
                AdvertisementService.SetErros(errors);
                AdvertisementService.SetMemoryCache(_cache);
                var adv = new AdvertisementService(0, new TestExceptionThirdPartyService());
                var advertisement = adv.GetAdvertisement("123");
                Assert.IsTrue(advertisement != null && advertisement.Name == "SQL Advert");
            }
            catch
            {
                Assert.IsFalse(true);
            }
        }



    }
}
