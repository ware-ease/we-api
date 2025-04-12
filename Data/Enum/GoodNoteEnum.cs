using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Enum
{
    public enum GoodNoteEnum
    {
        Receive,
        Issue,
        //Transfer, // ko can
        //Return, // ko can
    }
    public enum GoodNoteStatusEnum
    {
        Pending,      // 🟡 Chờ xử lý
        Completed,    // ✅ Hoàn thành
        Canceled,     // ❌ Đã hủy
        Failed        // ⚠️ Thất bại (nếu có lỗi trong quá trình thực hiện)
    }
}
