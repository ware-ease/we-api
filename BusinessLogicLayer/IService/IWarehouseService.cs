﻿using BusinessLogicLayer.Models.Pagination;
using BusinessLogicLayer.Models.Warehouse;

namespace BusinessLogicLayer.IService
{
    public interface IWarehouseService
    {
        Task<PageEntity<WarehouseDTO>> GetAllAsync(int pageIndex, int pageSize);
        Task<WarehouseDTO?> GetByIdAsync(string id);
        Task<WarehouseDTO> CreateAsync(CreateWarehouseDTO model);
        Task<WarehouseDTO?> UpdateAsync(string id, UpdateWarehouseDTO model);
        Task<bool> DeleteAsync(string id, string deletedBy);
        Task<PageEntity<WarehouseDTO>> SearchAsync(string? searchKey, int? pageIndex, int? pageSize);
    }
}
