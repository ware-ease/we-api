namespace BusinessLogicLayer.Utils
{
    public class PaginHelper
    {
        public static int PageCount(int totalCount, int pageSize)
        {
            var pageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
            return pageCount;
        }
    }
}
