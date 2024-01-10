using AspHttpSecurity.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspHttpSecurity.core
{
    public interface ISecurity
    {
        public delegate void Configure(HttpConfiguration config);
        public delegate void RequestAtributesConfig(RequestAtributes req);
    }

    public interface ErrorHandlingService
    {
        public object? Error(Exception e);
    }
}
