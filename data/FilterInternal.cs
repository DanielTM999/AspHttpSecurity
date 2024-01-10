using AspHttpSecurity.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspHttpSecurity.data
{
    public class FilterInternal
    {
        private HttpConfiguration _config;

        public FilterInternal(HttpConfiguration config)
        {
            _config = config;
        }

        public FilterInternal addFilterBefore(Type filter)
        {
            Type interfaceType = typeof(InternalFilter);
            if (!filter.Equals(interfaceType))
            {
                if (_config.filterBefore == null)
                {
                    _config.filterBefore = new List<Type>();
                }

                _config.filterBefore.Add(filter);
            }
            else
            {
                throw new ArgumentException("O tipo fornecido não implementa a interface desejada", nameof(filter));
            }

            return this;
        }

        public FilterInternal addFilterAfter(Type filter)
        {
            Type interfaceType = typeof(InternalFilter);
            if (!filter.Equals(interfaceType))
            {
                if (_config.filterBefore == null)
                {
                    _config.filterAfter = new List<Type>();
                }

                _config.filterAfter.Add(filter);
            }
            else
            {
                throw new ArgumentException("O tipo fornecido não implementa a interface desejada", nameof(filter));
            }

            return this;
        }
    }
}
