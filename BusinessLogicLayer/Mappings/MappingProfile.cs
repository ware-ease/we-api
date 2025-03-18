using BusinessLogicLayer.Models.Category;
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
using Data.Model.Request.Batch;
using Data.Model.Request.Brand;
using Data.Model.Request.Category;
using Data.Model.Request.Customer;
using Data.Model.Request.GoodRequest;
using Data.Model.Request.Product;
using Data.Model.Request.ProductType;
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
            CreateMap<Partner, Data.Model.DTO.CustomerDTO>().ReverseMap();
            CreateMap<CustomerCreateDTO, Partner>().ReverseMap();
            CreateMap<CustomerUpdateDTO, Partner>().ReverseMap();

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

            #region ProductType
            CreateMap<ProductType, ProductTypeDTO>().ReverseMap();
            CreateMap<ProductType, ProductTypeCreateDTO>().ReverseMap();
            #endregion ProductType

            #region Category
            CreateMap<Category, CategoryDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.Name} {src.Note}".Trim()))
                .ReverseMap();

            CreateMap<Category, CategoryCreateDTO>().ReverseMap();

            CreateMap<Category, CategoryUpdateDTO>().ReverseMap();


            CreateMap<Category, DeleteCategoryDTO>();
            #endregion

            #region Supplier
            CreateMap<Partner, SupplierDTO>().ReverseMap();

            CreateMap<Partner, SupplierCreateDTO>().ReverseMap();

            CreateMap<Partner, SupplierUpdateDTO>().ReverseMap();
            #endregion

            #region Customer
            CreateMap<Partner, CustomerDTO>().ReverseMap();

            CreateMap<Partner, CustomerCreateDTO>().ReverseMap();

            CreateMap<Partner, CustomerUpdateDTO>().ReverseMap();
            #endregion

            #region ReceivingNote
            CreateMap<ReceivingNote, CreateReceivingNoteDTO>();
            CreateMap<CreateReceivingNoteDTO, ReceivingNote>();
            #endregion

            #region Product
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => $"{src.ProductType.Name} {src.ProductType.Note}".Trim()))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => $"{src.ProductType.Category.Name} {src.ProductType.Category.Note}".Trim()))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => $"{src.Brand.Name}".Trim()))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => $"{src.Unit.Name} {src.Unit.Note}".Trim()))
                .ReverseMap();

            CreateMap<ProductCreateDTO, Product>().ReverseMap();
            CreateMap<ProductUpdateDTO, Product>().ReverseMap();
            #endregion

            #region Batch
            CreateMap<Batch, BatchDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ReverseMap();
            CreateMap<Batch, BatchCreateDTO>().ReverseMap();
            CreateMap<Batch, BatchUpdateDTO>().ReverseMap();
            #endregion

            #region Brand
            CreateMap<Brand, BrandDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.Name}".Trim()))
                .ReverseMap();

            CreateMap<BrandCreateDTO, Brand>().ReverseMap();
            CreateMap<BrandUpdateDTO, Brand>().ReverseMap();
            #endregion

            #region Unit
            CreateMap<Unit, UnitDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.Name} {src.Note}".Trim()))
                .ReverseMap();

            CreateMap<UnitCreateDTO, Unit>().ReverseMap();
            CreateMap<UnitUpdateDTO, Unit>().ReverseMap();
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
            CreateMap<Warehouse, WarehouseFullInfoDTO>().ReverseMap();
            CreateMap<Location, LocationDto>().ReverseMap();
            CreateMap<Location, LocationCreateDto>().ReverseMap();
            #endregion

            #region GoodRequest
            CreateMap<GoodRequest, GoodRequestDTO>()
                           .ForMember(dest => dest.GoodRequestDetails, opt => opt.MapFrom(src => src.GoodRequestDetails)).ReverseMap();
            CreateMap<GoodRequestCreateDTO, GoodRequest>().ReverseMap();
            CreateMap<GoodRequestUpdateDTO, GoodRequest>().ReverseMap();
            CreateMap<GoodRequestDetail, GoodRequestDetailDTO>().ReverseMap();
            #endregion
        }
    }
}