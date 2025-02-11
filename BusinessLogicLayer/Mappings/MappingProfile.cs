using AutoMapper;
using BusinessLogicLayer.Models.Account;
using BusinessLogicLayer.Models.Authentication;
using BusinessLogicLayer.Models.Category;
using BusinessLogicLayer.Models.PurchaseReceipt;
using BusinessLogicLayer.Models.Supplier;
using Data.Entity;

namespace BusinessLogicLayer.Mappings
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            // Mapping classes
            CreateMap<Account, AccountDTO>().ReverseMap();
            CreateMap<Token, TokenDTO>().ReverseMap();

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
