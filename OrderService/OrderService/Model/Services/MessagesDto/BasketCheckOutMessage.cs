﻿using System.Collections.Generic;
using System;
using OrderService.MessagingBus;

namespace OrderService.Model.Services.MessagesDto
{
    public class BasketCheckOutMessage: BaseMessage
    {
        public Guid BasketId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string UserId { get; set; }
        public int TotalPrice { get; set; }
        public List<BasketItemMessage> basketItemMessages { get; set; } = new List<BasketItemMessage>();
    }
}
