using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdParty;

namespace Adv
{
    public class ThirdParyService : IThirdPartyService
    {
        private NoSqlAdvProvider _noSqlDataProvider;
        public ThirdParyService(NoSqlAdvProvider sqldataProvider)
        {
            _noSqlDataProvider = sqldataProvider;

        }

        public Advertisement NoSqlGetAdv(string id)
        {
            return _noSqlDataProvider.GetAdv(id);
        }

        public Advertisement SqlGetAdv(string id)
        {
            return SQLAdvProvider.GetAdv(id); ;
        }
    }
}
