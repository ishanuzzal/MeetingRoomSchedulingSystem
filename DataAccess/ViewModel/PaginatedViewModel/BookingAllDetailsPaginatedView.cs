using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.ViewModel.admin;

namespace DataAccess.ViewModel.PaginatedViewModel
{
    public class BookingAllDetailsPaginatedView
    {
        public IEnumerable<BookingAllDetailsView> bookingAllDetailsViews { get; set; }
        public int PageSize { get; set; } = 5;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int EmpId { get; set; } = 0;
        public string OrderBy { get; set; } = string.Empty;
       
    }
}
