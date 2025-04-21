using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Enum
{
    public enum GoodRequestEnum
    {
        Receive,
        Issue,
        Transfer,
        Return,
    }
    public enum GoodRequestStatusEnum
    {
        Pending, // chờ xử lí, good request mới tạo
        Approved, // cứ tạo good note thì thành approved
        Rejected, // từ chối yêu cầu
        Completed, // thủ kho xác nhận
        Issued, // đã xuất kho cho yêu cầu điều chuyển
        //Canceled, // bỏ
    }
}
