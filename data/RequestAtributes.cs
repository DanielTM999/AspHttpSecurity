using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspHttpSecurity.data
{
    public class RequestAtributes
    {
        public Dictionary<string, PatternConfig> req;
        public bool autenticate = true;

        public RequestAtributes()
        {
            req = new();
        }

        public PatternConfig Request(String pattern)
        {
            var patern = new PatternConfig(pattern, HttpSecurityMethods.ALL, this);
            req.Add(pattern, patern);
            return patern;
        }

        public PatternConfig Request(String pattern, HttpSecurityMethods httpMethods)
        {
            var patern = new PatternConfig(pattern, httpMethods, this);
            req.Add(pattern, patern);
            return patern;
        }

        public Allreqs any()
        {
            var allreqs = new Allreqs(this);
            return allreqs;
        }
    }
}
