using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace CashGenicClient
{
    public class SessionPayment
    {

        public int PaymentRequest { get; set; }
        public int PaymentMade { get; set; }

        public int RefundRequest { get; set; }

        public int RefundMade { get; set; }

        public int ChangeMade { get; set; }





    }
}
