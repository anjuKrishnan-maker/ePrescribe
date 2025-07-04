using Allscripts.ePrescribe.Common;
using System;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb.AppCode.DurBPL
{
    public class DurFactory : IDurFactory
    {
        public DurFactory(DurType serviceType)
        {
            Type = serviceType;
        }

        public DurType Type { get; set; }
        public IDur GetDurObject()
        {
            switch (Type)
            {
                case DurType.SINGLE_SELECT:
                    return new DurSingleSelect();
                case DurType.MULTI_SELECT:
                    return new DurMultiSelect();
                   
            }
            throw new NotImplementedException(string.Format("{0} Type{1} DurFactory GetDurObject", Constants.ErrorMessages.NotImplemented, Type));
        }

    }

}