using AutoMapper;
using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.Authentication;
using BusinessLogicLayer.Models.Category;
using BusinessLogicLayer.Models.Profile;
using BusinessLogicLayer.Models.PurchaseReceipt;
using BusinessLogicLayer.Models.Supplier;
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

            CreateMap<Token, TokenDTO>().ReverseMap();

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
        }
    }
}
