using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.AccountWarehouse;
using BusinessLogicLayer.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IService
{
    public interface IAccountWarehouseService
    {
        Task<IEnumerable<AccountWarehouseDTO>> GetAllAsync();
        Task<AccountWarehouseDTO> GetByIdAsync(string accountId, string warehouseId);
        Task<AccountWarehouseDTO> CreateAsync(CreateAccountWarehouseDTO model);
        Task<bool> DeleteAsync(DeleteAccountWarehouseDTO model);
        Task<IEnumerable<WarehouseDTO>> GetWarehousesByAccountIdAsync(string accountId);
        Task<IEnumerable<AccountDTO>> GetAccountsByWarehouseIdAsync(string warehouseId);
        Task<AccountWarehouseDTO> UpdateAsync(string accountId, string warehouseId, UpdateAccountWarehouseDTO model);
    }
}
