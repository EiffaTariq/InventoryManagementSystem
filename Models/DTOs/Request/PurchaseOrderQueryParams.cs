namespace IMS.Models.DTOs.Request
{
    public class PurchaseOrderQueryParams
    {
        private int _page = 1;
        private int _pageSize = 10;

        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 10 : (value > 100 ? 100 : value);
        }

        public int? CategoryId { get; set; } // filters POs containing a product in this category
        public int? SupplierId { get; set; }
    }
}