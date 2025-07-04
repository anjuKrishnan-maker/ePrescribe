using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eRxWeb.AppCode.Interfaces
{
    public interface IEmail
    {
        bool Send(string personalEmail, StringWriter writer,string emailSubject);

        StringWriter CreateXsltTemplate(string xslPath, string userName, string activationCode);
    }
}
