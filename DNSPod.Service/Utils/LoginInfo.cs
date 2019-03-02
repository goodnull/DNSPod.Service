using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSPod.Service
{
    public class LoginInfo
    {
        public string id { get; set; }
        public string token { get; set; }
        public string login_token
        {
            get
            {
                return $"{id},{token}";
            }
        }
    }
}
