using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IServices;
using Data.Entity;
using Data.Model.DTO;
using Data.Model.Request.ErrorTicket;
using Data.Model.Response;
using DataAccessLayer.Generic;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ErrorTicketService : GenericService<ErrorTicket>, IErrorTicketService
    {
        private readonly IGenericRepository<InventoryCountDetail> _inventoryCountDetailRepository;
        public ErrorTicketService(IGenericRepository<ErrorTicket> genericRepository,
            IGenericRepository<InventoryCountDetail> inventoryCountDetailRepository,
            IMapper mapper, IUnitOfWork unitOfWork) : base(genericRepository, mapper, unitOfWork)
        {
            _inventoryCountDetailRepository = inventoryCountDetailRepository;
        }

        public override async Task<ServiceResponse> Get<TResult>()
        {
            var products = await _genericRepository.GetAllNoPaging(
                includeProperties: "InventoryCountDetail"
            );

            List<TResult> mappedResults = _mapper.Map<List<TResult>>(products);

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Get successfully!",
                Data = mappedResults
            };
        }

        public async Task<ErrorTicketDTO> AddErrorTicket(ErrorTicketCreateDTO request)
        {
            var inventoryCountDetail = await _inventoryCountDetailRepository.GetByCondition(
                x => x.Id == request.InventoryCountDetailId
            );
            if (inventoryCountDetail == null)
            {
                throw new Exception("InventoryCountDetail không tồn tại");
            }

            if (inventoryCountDetail.CountedQuantity == inventoryCountDetail.ExpectedQuantity)
            {
                throw new Exception("InventoryCountDetail này không có vấn đề để tạo Ticket");
            }

            var errorTicket = _mapper.Map<ErrorTicket>(request);
            errorTicket.InventoryCountDetailId = request.InventoryCountDetailId;

            await _genericRepository.Add(errorTicket);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<ErrorTicketDTO>(errorTicket);
        }

        public async Task<ErrorTicketDTO> UpdateErrorTicket(ErrorTicketUpdateDTO request)
        {
            var existedErrorTicket = await _genericRepository.GetByCondition(x => x.Id == request.Id);
            if (existedErrorTicket == null)
            {
                throw new Exception("ErrorTicket không tồn tại");
            }
            if (!string.IsNullOrEmpty(request.Reason))
            {
                existedErrorTicket.Reason = request.Reason;
            }
            if (!string.IsNullOrEmpty(request.Code))
            {
                existedErrorTicket.Code = request.Code;
            }
            if (!string.IsNullOrEmpty(request.Note))
            {
                existedErrorTicket.Note = request.Note;
            }
            if (!string.IsNullOrEmpty(request.HandleBy))
            {
                existedErrorTicket.HandleBy = request.HandleBy;
            }
            if (!string.IsNullOrEmpty(request.InventoryCountDetailId))
            {
                var inventoryCountDetail = await _inventoryCountDetailRepository.GetByCondition(
                                x => x.Id == request.InventoryCountDetailId
                            );
                if (inventoryCountDetail == null)
                {
                    throw new Exception("InventoryCountDetail không tồn tại");
                }
                if (inventoryCountDetail.CountedQuantity == inventoryCountDetail.ExpectedQuantity)
                {
                    throw new Exception("InventoryCountDetail này không có vấn đề để tạo Ticket");
                }
            }


            _genericRepository.Update(existedErrorTicket);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<ErrorTicketDTO>(existedErrorTicket);
        }

        public async Task<ServiceResponse> Search<TResult>(int? pageIndex = null, int? pageSize = null,
                                                                   string? keyword = null)
        {

            Expression<Func<ErrorTicket, bool>> filter = p =>
                (string.IsNullOrEmpty(keyword) || p.Reason.Contains(keyword)
                || p.Code.Contains(keyword)
                || p.Note.Contains(keyword)
                || p.HandleBy.Contains(keyword))
                || p.InventoryCountDetail.Note.Contains(keyword);

            var totalRecords = await _genericRepository.Count(filter);

            var results = await _genericRepository.Search(
                filter: filter, pageIndex: pageIndex, pageSize: pageSize,
                includeProperties: "InventoryCountDetail");

            var mappedResults = _mapper.Map<IEnumerable<TResult>>(results);

            int totalPages = (int)Math.Ceiling((double)totalRecords / (pageSize ?? totalRecords));

            return new ServiceResponse
            {
                Status = Data.Enum.SRStatus.Success,
                Message = "Search successful!",
                Data = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageIndex = pageIndex ?? 1,
                    PageSize = pageSize ?? totalRecords,
                    Records = mappedResults
                }
            };
        }
    }
}
