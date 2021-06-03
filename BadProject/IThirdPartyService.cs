using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdParty;

namespace Adv
{
    public interface IThirdPartyService
    {
       Advertisement  NoSqlGetAdv(string id);
       Advertisement  SqlGetAdv(string id);
    }
}
