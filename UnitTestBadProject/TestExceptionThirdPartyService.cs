using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdParty;
using Adv;

namespace UnitTestBadProject
{
    public class TestExceptionThirdPartyService :    IThirdPartyService
    {


        public Advertisement NoSqlGetAdv(string id)
        {
            throw new Exception("Test Exception");
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
