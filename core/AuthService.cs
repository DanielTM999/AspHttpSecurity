using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspHttpSecurity.core
{
    public interface IAuthService
    {
        AuthDetails GetUserDetails(string user);
    }
}
