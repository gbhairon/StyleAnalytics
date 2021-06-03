using Adv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdParty;

namespace UnitTestBadProject
{
    public class TestThirdPartyService : IThirdPartyService
    {

        public Advertisement NoSqlGetAdv(string id)
        {
            var advert = new Advertisement();
            advert.Description = "No SQL Advert Description";
            advert.Name= "No SQL Advert";
            advert.WebId = id;
            return advert;
        }

        public Advertisement SqlGetAdv(string id)
        {
            var advert = new Advertisement();
            advert.Description = "SQL Advert Description";
            advert.Name = "SQL Advert";
            advert.WebId = id;
            return advert;
        }
    }
}
