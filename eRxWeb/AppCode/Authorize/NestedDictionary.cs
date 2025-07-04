using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.Authorize
{
    public class NestedDictionary<V> : Dictionary<string, NestedDictionary<V>>
    {
        public V Value { set; get; }

        public new NestedDictionary<V> this[string key]
        {
            set
            {
                base[key.ToLower()] = value;
            }

            get
            {
                if (!base.Keys.Contains(key.ToLower()))
                {
                    base[key.ToLower()] = new NestedDictionary<V>();
                }
                return base[key.ToLower()];
            }
        }
    }
}