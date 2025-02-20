using AutoMapper;
using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.AccountGroup;
using BusinessLogicLayer.Models.AccountPermission;
using BusinessLogicLayer.Models.AppAction;
using BusinessLogicLayer.Models.Authentication;
using BusinessLogicLayer.Models.Category;
using BusinessLogicLayer.Models.Cell;
using BusinessLogicLayer.Models.Floor;
using BusinessLogicLayer.Models.Group;
using BusinessLogicLayer.Models.Permission;
using BusinessLogicLayer.Models.Product;
using BusinessLogicLayer.Models.ProductType;
using BusinessLogicLayer.Models.ProductTypeTypeDetail;
using BusinessLogicLayer.Models.Profile;
using BusinessLogicLayer.Models.PurchaseDetail;
using BusinessLogicLayer.Models.PurchaseReceipt;
using BusinessLogicLayer.Models.ReceivingDetail;
using BusinessLogicLayer.Models.ReceivingNote;
using BusinessLogicLayer.Models.Shelf;
using BusinessLogicLayer.Models.Supplier;
using BusinessLogicLayer.Models.TypeDetail;
using BusinessLogicLayer.Models.Warehouse;
using Data.Entity;
using Profile = Data.Entity.Profile;

namespace BusinessLogicLayer.Mappings
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            // Mapping classes
            #region Account
            CreateMap<Account, AccountDTO>()
                .ForMember(dest => dest.GroupIds, opt => opt.MapFrom(src => src.AccountGroups.Select(ag => ag.GroupId).ToList()))
                .ForMember(dest => dest.PermissionIds, opt => opt.MapFrom(src => src.AccountPermissions.Select(ap => ap.PermissionId).ToList()))
                .ForMember(dest => dest.WarehouseIds, opt => opt.MapFrom(src => src.AccountWarehouses.Select(aw => aw.WarehouseId).ToList()));
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

            #region PurchaseReceipt
            CreateMap<PurchaseReceipt, CreatePurchaseReceiptDTO>();
            CreateMap<CreatePurchaseReceiptDTO, PurchaseReceipt>();
            #endregion

            #region ReceivingNote
            CreateMap<ReceivingNote, CreateReceivingNoteDTO>();
            CreateMap<CreateReceivingNoteDTO, ReceivingNote>();
            #endregion

            #region Product
            CreateMap<Product, CreateProductDTO>();
            CreateMap<CreateProductDTO, Product>();
            #endregion

            #region ProductType
            CreateMap<ProductType, CreateProductTypeDTO>();
            CreateMap<CreateProductTypeDTO, ProductType>();
            #endregion

            #region PurchaseDetail
            CreateMap<PurchaseDetail, CreatePurchaseDetailDTO>();
            CreateMap<CreatePurchaseDetailDTO, PurchaseDetail>();
            #endregion

            #region ReceivingDetail
            CreateMap<ReceivingDetail, CreateReceivingDetailDTO>();
            CreateMap<CreateReceivingDetailDTO, ReceivingDetail>();
            #endregion

            #region TypeDetail
            CreateMap<TypeDetail, CreateTypeDetailDTO>();
            CreateMap<CreateTypeDetailDTO, TypeDetail>();
            #endregion

            #region ProductTypeTypeDetail
            CreateMap<ProductTypeTypeDetail, CreateProductTypeTypeDetailDTO>();
            CreateMap<CreateProductTypeTypeDetailDTO, ProductTypeTypeDetail>();
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

            #region AccountPermission
            CreateMap<AccountPermission, AccountPermissionDTO>();
            CreateMap<CreateAccountPermissionDTO, AccountPermission>();
            CreateMap<UpdateAccountPermissionDTO, AccountPermission>();
            #endregion
        }
    }
}
