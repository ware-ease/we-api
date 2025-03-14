﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entity.Base;

namespace Data.Entity
{
    [Table("AccountWarehouse")]
    public class AccountWarehouse : BaseEntity
    {
        public DateTime? JoinedDate { get; set; }
        public DateTime? LeftDate { get; set; }
        public bool Status { get; set; }

        [ForeignKey("Account")]
        public string AccountId { get; set; }
        public Account Account { get; set; }

        [ForeignKey("Warehouse")]
        public string WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
