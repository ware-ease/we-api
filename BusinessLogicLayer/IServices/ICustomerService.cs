﻿using BusinessLogicLayer.Generic;
using Data.Entity;
using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface ICustomerService : IGenericService
    {
        void Test();
    }
}
