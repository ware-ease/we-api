using AutoMapper;
using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.AccountAction;
using BusinessLogicLayer.Models.AccountGroup;
using BusinessLogicLayer.Models.AccountPermission;
using BusinessLogicLayer.Models.AccountWarehouse;
using BusinessLogicLayer.Models.AppAction;
using BusinessLogicLayer.Models.Authentication;
using BusinessLogicLayer.Models.Category;
using BusinessLogicLayer.Models.Cell;
using BusinessLogicLayer.Models.Floor;
using BusinessLogicLayer.Models.Group;
using BusinessLogicLayer.Models.GroupPermission;
using BusinessLogicLayer.Models.Permission;
using BusinessLogicLayer.Models.PermissionAction;
using BusinessLogicLayer.Models.Product;
using BusinessLogicLayer.Models.ProductType;
using BusinessLogicLayer.Models.ProductTypeTypeDetail;
using BusinessLogicLayer.Models.Profile;
using BusinessLogicLayer.Models.PurchaseDetail;
using BusinessLogicLayer.Models.PurchaseReceipt;
using BusinessLogicLayer.Models.ReceivingDetail;
using BusinessLogicLayer.Models.ReceivingNote;
using BusinessLogicLayer.Models.Shelf;
using BusinessLogicLayer.Models.StockCard;
using BusinessLogicLayer.Models.StockCardDetail;
using BusinessLogicLayer.Models.Supplier;
using BusinessLogicLayer.Models.TypeDetail;
using BusinessLogicLayer.Models.Warehouse;
using Data.Entity;
using Data.Model.Request.Customer;
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
                .ForMember(dest => dest.Groups, opt => opt.MapFrom(src => src.AccountGroups.Select(ag => ag.Group)))
                .ForMember(dest => dest.Actions, opt => opt.MapFrom(src => src.AccountActions.Select(ag => ag.Action)))
                .ForMember(dest => dest.Warehouses, opt => opt.MapFrom(src => src.AccountWarehouses.Select(aw => aw.Warehouse)))
                .ReverseMap();

            CreateMap<Account, AccountUpdateDTO>().ReverseMap();

            CreateMap<Account, AccountCreateDTO>().ReverseMap();
            #endregion

            #region Token
            CreateMap<Token, TokenDTO>().ReverseMap();
            #endregion

            #region Profile
            CreateMap<Profile, ProfileDTO>().ReverseMap();
            CreateMap<Profile, ProfileCreateDTO>().ReverseMap();
            CreateMap<Profile, ProfileUpdateDTO>().ReverseMap();
            #endregion

            #region Category
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();

            CreateMap<Category, UpdateCategoryDTO>();
            CreateMap<UpdateCategoryDTO, Category>();

            CreateMap<Category, DeleteCategoryDTO>();
            CreateMap<DeleteCategoryDTO, Category>();
            #endregion

            #region Supplier
            CreateMap<Supplier, CreateSupplierDTO>();
            CreateMap<CreateSupplierDTO, Supplier>();
            #endregion

            #region ReceivingNote
            CreateMap<ReceivingNote, CreateReceivingNoteDTO>();
            CreateMap<CreateReceivingNoteDTO, ReceivingNote>();
            #endregion

            #region Product
            CreateMap<Product, CreateProductDTO>();
            CreateMap<CreateProductDTO, Product>();
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
            CreateMap<Group, GroupDTO>().ReverseMap();
            CreateMap<Group, CreateGroupDTO>().ReverseMap();
            #endregion

            #region AppAction
            CreateMap<AppAction, AppActionDTO>().ReverseMap();
            CreateMap<AppAction, CreateAppActionDTO>().ReverseMap();
            #endregion

            #region Permission
            CreateMap<Permission, PermissionDTO>().ReverseMap();
            CreateMap<Permission, CreatePermissionDTO>().ReverseMap();
            #endregion

            #region Warehouse
            CreateMap<Warehouse, WarehouseDTO>().ReverseMap();
            CreateMap<Warehouse, CreateWarehouseDTO>().ReverseMap();
            #endregion

            #region AccountGroup
            CreateMap<AccountGroup, AccountGroupDTO>().ReverseMap();
            CreateMap<AccountGroup, CreateAccountGroupDTO>().ReverseMap();
            CreateMap<AccountGroup, UpdateAccountGroupDTO>().ReverseMap();
            #endregion

            #region AccountAction
            CreateMap<AccountAction, AccountActionDTO>().ReverseMap();
            CreateMap<AccountAction, CreateAccountActionDTO>().ReverseMap();
            CreateMap<AccountAction, UpdateAccountActionDTO>().ReverseMap();
            #endregion

            #region AccountPermission
            CreateMap<AccountAction, AccountPermissionDTO>().ReverseMap();
            CreateMap<CreateAccountPermissionDTO, AccountAction>().ReverseMap();
            CreateMap<UpdateAccountPermissionDTO, AccountAction>().ReverseMap();
            #endregion

            #region PermissionAction
            //CreateMap<PermissionAction, PermissionActionDTO>().ReverseMap();
            //CreateMap<CreatePermissionActionDTO, PermissionAction>().ReverseMap();
            //CreateMap<UpdatePermissionActionDTO, PermissionAction>().ReverseMap();
            #endregion

            #region GroupPermission
            CreateMap<GroupAction, GroupPermissionDTO>().ReverseMap();
            CreateMap<CreateGroupPermissionDTO, GroupAction>().ReverseMap();
            CreateMap<UpdateGroupPermissionDTO, GroupAction>().ReverseMap();
            #endregion

            #region AccountWarehouse
            CreateMap<AccountWarehouse, AccountWarehouseDTO>().ReverseMap();
            CreateMap<CreateAccountWarehouseDTO, AccountWarehouse>().ReverseMap();
            CreateMap<UpdateAccountWarehouseDTO, AccountWarehouse>().ReverseMap();
            CreateMap<DeleteAccountWarehouseDTO, AccountWarehouse>().ReverseMap();
            #endregion
        }
    }
}