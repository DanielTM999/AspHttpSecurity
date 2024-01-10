using AspHttpSecurity.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AspHttpSecurity.core.ISecurity;

namespace AspHttpSecurity.data
{
    public class HttpConfiguration
    {
        public SessionPolicy _sessionPolicy { get; private set; }
        public RequestAtributes _pattern;
        public List<Type> filterBefore;
        public List<Type> filterAfter;
        public bool NoDataBase { get; private set; }
        public ErrorHandlingService? exceptionValid { get; private set; }

        public HttpConfiguration()
        {
            _pattern = new RequestAtributes();
            filterBefore = new();
            filterAfter = new();
        }

        public HttpConfiguration NoDatabase()
        {
            NoDataBase = true;
            return this;
        }

        public HttpConfiguration sessionPolicy(SessionPolicy sessionPolicy)
        {
            _sessionPolicy = sessionPolicy;
            return this;
        }

        public HttpConfiguration ExceptionValidation<E>() where E : ErrorHandlingService, new()
        {
            E instanceOfE = new E();
            exceptionValid = instanceOfE;
            return this;
        }

        public FilterInternal RequestPatterns(RequestAtributesConfig pattern)
        {
            pattern(_pattern);
            return new FilterInternal(this);
        }

    }
}
