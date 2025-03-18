using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Request.Batch;
using Data.Model.Request.Product;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class BatchService : GenericService<Batch>, IBatchService
    {
        private readonly IGenericRepository<Batch> _batchRepository;
        private readonly IGenericRepository<Partner> _partnerRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        public readonly IUnitOfWork _unitOfWork;
        public BatchService(IGenericRepository<Batch> genericService,
            IGenericRepository<Partner> partnerRepository,
            IGenericRepository<Product> productRepository,
            IMapper mapper, 
            IUnitOfWork unitOfWork) : base(genericService, mapper, unitOfWork) 
        {
            _batchRepository = genericService;
            _partnerRepository = partnerRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BatchDTO> AddBatch(BatchCreateDTO request)
        {
            /*var supplier = await _partnerRepository.Get(request.SupplierId);
            if (supplier == null)
                throw new Exception("Supplier không tồn tại");

            if (supplier.PartnerType != Data.Enum.PartnerEnum.Supplier)
                throw new Exception("Partner này không phải là Supplier");*/

            var product = await _productRepository.Get(request.ProductId);
            if (product == null)
                throw new Exception("Product không tồn tại");

            if (request.MfgDate >= request.ExpDate)
                throw new Exception("MfgDate phải nhỏ hơn ExpDate");


            var batch = _mapper.Map<Batch>(request);

            await _genericRepository.Insert(batch);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<BatchDTO>(batch);
        }

        public async Task<BatchDTO> UpdateBatch(BatchUpdateDTO request)
        {
            var existingBatch = await _batchRepository.Get(request.Id);
            if (existingBatch == null)
                throw new Exception("Batch not found");

            if (!string.IsNullOrEmpty(request.Code))
            {
                existingBatch.Code = request.Code;
            }
            if (!string.IsNullOrEmpty(request.Name))
            {
                existingBatch.Name = request.Name;
            }

            if (!string.IsNullOrEmpty(request.ProductId))
            {
                var product = await _productRepository.Get(request.ProductId);
                if (product == null)
                    throw new Exception("Product not found");
                existingBatch.ProductId = request.ProductId;
            }

            _batchRepository.Update(existingBatch);
            await _unitOfWork.SaveAsync();

            var updatedBatch = await _batchRepository.Get(existingBatch.Id);
            if (updatedBatch == null)
                throw new Exception("Update failed, batch not found after update");

            return _mapper.Map<BatchDTO>(updatedBatch);
        }
    }
}
