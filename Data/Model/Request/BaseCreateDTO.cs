﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request
{
    public class BaseCreateDTO
    {
        [JsonIgnore]
        public string? CreatedBy { get; set; }
    }
}
