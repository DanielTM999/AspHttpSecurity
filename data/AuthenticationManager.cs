using AspHttpSecurity.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspHttpSecurity.data
{
    public class AuthenticationManager
    {
        public AuthDetails? NextAuthDetails { get; set; }

        public void ClearAuthDetails()
        {
            NextAuthDetails = null;
        }

        public bool isPresent()
        {
            return NextAuthDetails != null;
        }
    }
}
