using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspHttpSecurity.data
{
    public class PatternConfig
    {
        public string path { get; set; }
        public HttpSecurityMethods method { get; set; }

        public Rolesadd? rolesadd { get; private set; }

        public bool auth {  get; private set; }

        private RequestAtributes atribute;

        public PatternConfig(string path, HttpSecurityMethods method, RequestAtributes atribute)
        {
            this.path = path;
            this.method = method;
            this.atribute = atribute;
        }

        public RequestAtributes permitAll()
        {
            auth = false;
            return atribute;
        }

        public Rolesadd authenticate()
        {
            auth = true;
            rolesadd = new(atribute);
            return rolesadd;
        }

    }
}
