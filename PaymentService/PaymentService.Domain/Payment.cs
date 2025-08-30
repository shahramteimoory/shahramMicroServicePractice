﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Domain
{
    public class Payment
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public bool IsPay { get; set; }
        public DateTime? DatePay { get; set; }
        public string Authority { get; set; }
        public long RefId { get; set; } = 0;
        public Order Order { get; set; }
        public Guid OrderId { get; set; }
    }
}
