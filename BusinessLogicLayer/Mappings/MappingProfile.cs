﻿using BusinessLogicLayer.Models.Category;
using BusinessLogicLayer.Models.Cell;
using BusinessLogicLayer.Models.Floor;
using BusinessLogicLayer.Models.Product;
using BusinessLogicLayer.Models.ReceivingNote;
using BusinessLogicLayer.Models.Shelf;
using BusinessLogicLayer.Models.StockCard;
using BusinessLogicLayer.Models.StockCardDetail;
using BusinessLogicLayer.Models.Supplier;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Request.Account;
using Data.Model.Request.Area;
using Data.Model.Request.Brand;
using Data.Model.Request.Category;
using Data.Model.Request.Customer;
using Data.Model.Request.Product;
using Data.Model.Request.Suppiler;
using Data.Model.Request.Supplier;
using Data.Model.Request.Unit;
using Data.Model.Request.Warehouse;
using static Data.Model.Request.Warehouse.WarehouseFullInfoDTO;
using Profile = Data.Entity.Profile;

namespace BusinessLogicLayer.Mappings
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, Data.Model.DTO.CustomerDTO>().ReverseMap();
            CreateMap<CustomerCreateDTO, Customer>().ReverseMap();
            CreateMap<CustomerUpdateDTO, Customer>().ReverseMap();

            // Mapping classes
            #region Account
            CreateMap<Account, AccountDTO>()
                .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => src.Profile))
                .ForMember(dest => dest.Groups, opt => opt.MapFrom(src => src.AccountGroups.Select(ag => ag.Group)))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.AccountPermissions.Select(ag => ag.Permission)))
                .ForMember(dest => dest.Warehouses, opt => opt.MapFrom(src => src.AccountWarehouses.Select(aw => aw.Warehouse)))
                .ReverseMap();
            CreateMap<Account, AccountUpdateDTO>().ReverseMap();
            CreateMap<Account, AccountCreateDTO>().ReverseMap();
            CreateMap<Profile, AccountDTOProfile>().ReverseMap();
            CreateMap<Group, AccountDTOGroup>().ReverseMap();
            CreateMap<Warehouse, AccountDTOWarehouse>().ReverseMap();
            #endregion

            #region Token
            #endregion

            #region Profile
            CreateMap<Profile, ProfileCreateDTO>().ReverseMap();
            //CreateMap<Profile, ProfileUpdateDTO>().ReverseMap();
            #endregion

            #region Category
            CreateMap<Category, CategoryDTO>().ReverseMap();

            CreateMap<Category, CategoryCreateDTO>().ReverseMap();

            CreateMap<Category, CategoryUpdateDTO>().ReverseMap();


            CreateMap<Category, DeleteCategoryDTO>();
            #endregion

            #region Supplier
            CreateMap<Supplier, SupplierDTO>().ReverseMap();

            CreateMap<Supplier, SupplierCreateDTO>().ReverseMap();

            CreateMap<Supplier, SupplierUpdateDTO>().ReverseMap();
            #endregion

            #region ReceivingNote
            CreateMap<ReceivingNote, CreateReceivingNoteDTO>();
            CreateMap<CreateReceivingNoteDTO, ReceivingNote>();
            #endregion

            #region Product
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit)).ReverseMap();

            CreateMap<ProductCreateDTO, Product>().ReverseMap();
            CreateMap<ProductUpdateDTO, Product>().ReverseMap();
            #endregion

            #region Brand
            CreateMap<Brand, BrandDTO>().ReverseMap();

            CreateMap<BrandCreateDTO, Brand>().ReverseMap();
            CreateMap<BrandUpdateDTO, Brand>().ReverseMap();
            #endregion

            #region Unit
            CreateMap<Unit, UnitDTO>().ReverseMap();

            CreateMap<UnitCreateDTO, Unit>().ReverseMap();
            CreateMap<UnitUpdateDTO, Unit>().ReverseMap();
            #endregion

            #region Shelf
            CreateMap<Shelf, CreateShelfDTO>();
            CreateMap<CreateShelfDTO, Shelf>();
            #endregion

            #region Floor
            CreateMap<Floor, CreateFloorDTO>();
            CreateMap<CreateFloorDTO, Floor>();
            #endregion

            #region Cell
            CreateMap<Cell, CreateCellDTO>();
            CreateMap<CreateCellDTO, Cell>();
            #endregion

            #region StockCard
            CreateMap<CellBatch, CreateStockCardDTO>();
            CreateMap<CreateStockCardDTO, CellBatch>();
            #endregion

            #region StockCardDetail
            CreateMap<InOutDetail, CreateStockCardDetailDTO>();
            CreateMap<CreateStockCardDetailDTO, InOutDetail>();
            #endregion

            #region Group
            CreateMap<Group, GroupDTO>()
                .ForMember(dest => dest.Accounts, opt => opt.MapFrom(src => src.AccountGroups.Select(ag => ag.Account)))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.GroupPermissions.Select(ga => ga.Permission)))
                .ReverseMap();
            //CreateMap<Group, CreateGroupDTO>().ReverseMap();
            CreateMap<Account, GroupDTOAccount>().ReverseMap();
            #endregion

            #region Permission
            CreateMap<Permission, Data.Model.DTO.PermissionDTO>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Route.Url))
                .ForMember(dest => dest.RouteCode, opt => opt.MapFrom(src => src.Route.Code))
                .ReverseMap();
            #endregion

            #region Route
            CreateMap<Route, RouteDTO>().ReverseMap();
            #endregion

            #region Warehouse
            CreateMap<Warehouse, WarehouseDTO>().ReverseMap();
            CreateMap<Warehouse, CreateWarehouseDTO>().ReverseMap();
            CreateMap<Warehouse, UpdateWarehouseDTO>().ReverseMap();
            CreateMap<Warehouse, WarehouseFullInfoDTO>()
           .ForMember(dest => dest.Areas, opt => opt.MapFrom(src => src.Areas));

            CreateMap<Area, AreaDto>()
                .ForMember(dest => dest.Shelves, opt => opt.MapFrom(src => src.Shelves));

            CreateMap<Shelf, ShelfDto>()
                .ForMember(dest => dest.Floors, opt => opt.MapFrom(src => src.Floors));

            CreateMap<Floor, FloorDto>()
                .ForMember(dest => dest.Cells, opt => opt.MapFrom(src => src.Cells));

            CreateMap<Cell, CellDto>();
            #endregion
            //Chỉ map riêng cho area
            CreateMap<Area, AreaDTO>().ReverseMap();
            #region Area

            #endregion
        }
    }
}