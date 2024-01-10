using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AspHttpSecurity.data
{
    public class Rolesadd
    {
        public List<string> roles;

        private RequestAtributes _atribute;
        public Rolesadd(RequestAtributes atribute) 
        {
            _atribute = atribute;
            roles = new();
        }

        public RequestAtributes hasRole(params string[] roles)
        {
            if (roles == null || roles.Length == 0)
            {
                this.roles = new();
                return _atribute;
            }
            this.roles = roles.ToList();
            return _atribute;
        }
    }
}
