using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Response
{
    public static class ControllerResponse
    {
        public static IActionResult Response(ServiceResponse serviceResponse)
        {
            ObjectResult result = new ObjectResult(serviceResponse);

            if (serviceResponse.Status == Enum.SRStatus.Success)
            {
                result.StatusCode = 200;
            }

            if (serviceResponse.Status == Enum.SRStatus.NotFound)
            {
                result.StatusCode = 404;
            }

            if (serviceResponse.Status == Enum.SRStatus.Error)
            {
                result.StatusCode = 500;
            }

            if (serviceResponse.Status == Enum.SRStatus.Duplicated)
            {
                result.StatusCode = 400;
            }

            return result;
        }
    }
}
