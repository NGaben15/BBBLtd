﻿using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBBKliensAlkalmazas.DataModels
{
    public class Order
    {
        public int Id { get; set; }
        public string Bvin { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string StatusCode { get; set; }
        public int Price { get; set; }
        
        public DateTime Date { get;  set; }
        public string Address { get; set; }
        public string Email { get; set; }
    }
}
