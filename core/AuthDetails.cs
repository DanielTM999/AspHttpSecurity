using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspHttpSecurity.core
{
    public interface AuthDetails
    {
        public string getUserName();
        public string getPassword();
        public List<string> getRoles();
        public bool isBlock();
    }
}
