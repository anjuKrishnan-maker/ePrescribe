using System.Collections.Generic;
using System.Linq;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using Property = eRxWeb.ePrescribeSvc.Property;

namespace eRxWeb.AppCode
{
    public class UserProperties : IUserProperties
    {
        public string GetUserPropertyValue(string name, IStateContainer pageState)
        {
            if (pageState[Constants.SessionVariables.UserAppProperties] is Property[] props)
            {
                var prop = props.ToList().Find(x => x.PropertyName == name);
                if (prop != null)
                {
                    return prop.PropertyStatus;
                }
            }
            return string.Empty;
        }

        internal static bool GetBoolOrFalseUserPropertyValue(string name, IStateContainer pageState, IUserProperties userProperties)
        {
            bool value;

            var val = userProperties.GetUserPropertyValue(name, pageState);

            return bool.TryParse(val, out value) && value;
        }
    }
}