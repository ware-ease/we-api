using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models.AccountWarehouse
{
    public class AccountWarehouseDTO : BaseEntity
    {
        public DateTime? JoinedDate { get; set; }
        public DateTime? LeftDate { get; set; }
        public bool Status { get; set; }
        public string AccountId { get; set; }
        public string WarehouseId { get; set; }
    }

    public class CreateAccountWarehouseDTO
    {
        public string AccountId { get; set; }
        public string WarehouseId { get; set; }
        public string? CreatedBy { get; set; }
    }
    public class UpdateAccountWarehouseDTO
    {
        public DateTime? JoinedDate { get; set; }
        public DateTime? LeftDate { get; set; }
        public bool Status { get; set; }
        public string? LastUpdatedBy { get; set; }

    }
    public class DeleteAccountWarehouseDTO
    {
        public string AccountId { get; set; }
        public string WarehouseId { get; set; }
        public string? DeletedBy { get; set; }

    }
}
