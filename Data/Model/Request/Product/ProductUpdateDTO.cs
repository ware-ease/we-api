﻿using Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Model.Request.Product
{
    public class ProductUpdateDTO
    {
        [JsonIgnore]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Sku { get; set; }
        public bool? IsBatchManaged { get; set; }
        public string? imageUrl { get; set; }
        //[Required(ErrorMessage = "CategoryId không được để trống")]
        public string? ProductTypeId { get; set; }
        //[Required(ErrorMessage = "BrandId không được để trống")]
        public string? BrandId { get; set; }
        //[Required(ErrorMessage = "UnitId không được để trống")]
        public string? UnitId { get; set; }
    }
}
