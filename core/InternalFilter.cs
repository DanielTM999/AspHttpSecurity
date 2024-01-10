using AspHttpSecurity.data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspHttpSecurity.core
{
    public interface InternalFilter
    {
        public abstract void doFilter(HttpContext context, AuthenticationManager authenticationManager);
    }
}
