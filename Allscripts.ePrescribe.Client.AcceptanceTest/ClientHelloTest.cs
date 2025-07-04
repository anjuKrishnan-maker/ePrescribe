using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Allscripts.ePrescribe.Client.AcceptanceTest
{
    public class ClientHelloTest
    {
        public string Caller { get; set; }
        public string SayHello()
        {
            return String.Format("Client Hello, {0}", Caller);
        }
    }
}
