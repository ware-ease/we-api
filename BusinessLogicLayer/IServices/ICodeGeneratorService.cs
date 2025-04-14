﻿using Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface ICodeGeneratorService
    {
        Task<string> GenerateCodeAsync(CodeType codeType);
    }
}
