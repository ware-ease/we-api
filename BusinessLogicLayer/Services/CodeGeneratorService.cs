using BusinessLogicLayer.IServices;
using Data.Enum;
using DataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CodeGeneratorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<string> GenerateCodeAsync(CodeType codeType)
        {
            string prefix = codeType.ToString();
            string? maxCode = null;

            switch (codeType)
            {
                case CodeType.YCN:
                case CodeType.YCX:
                case CodeType.YCC:
                case CodeType.YCT:
                    var goodRequests = await _unitOfWork.GoodRequestRepository.Search(
                        x => x.Code.StartsWith(prefix),
                        q => q.OrderByDescending(x => x.Code),
                        "",
                        1, 1
                    );
                    maxCode = goodRequests.FirstOrDefault()?.Code;
                    break;

                case CodeType.PN:
                case CodeType.PX:
                case CodeType.PNNB:
                case CodeType.PXNB:
                    var goodNotes = await _unitOfWork.GoodNoteRepository.Search(
                        x => x.Code.StartsWith(prefix),
                        q => q.OrderByDescending(x => x.Code),
                        "",
                        1, 1
                    );
                    maxCode = goodNotes.FirstOrDefault()?.Code;
                    break;

                case CodeType.LO:
                    var batches = await _unitOfWork.BatchRepository.Search(
                        x => x.Code.StartsWith(prefix),
                        q => q.OrderByDescending(x => x.Code),
                        "",
                        1, 1
                    );
                    maxCode = batches.FirstOrDefault()?.Code;
                    break;
            }

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(maxCode))
            {
                var numberPart = maxCode.Substring(prefix.Length);
                if (int.TryParse(numberPart, out int currentNumber))
                    nextNumber = currentNumber + 1;
            }

            return $"{prefix}{nextNumber:D5}";
        }
    }
}
