﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.DTO
{
    public class TokenDTO
    {
        public string? accessToken { get; set; }
        public string? refreshToken { get; set; }
    }
}
