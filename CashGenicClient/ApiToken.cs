using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashGenicClient
{
    public class ApiToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string userName { get; set; }

        public static implicit operator Task<object>(ApiToken v)
        {
            throw new NotImplementedException();
        }
    }
}
