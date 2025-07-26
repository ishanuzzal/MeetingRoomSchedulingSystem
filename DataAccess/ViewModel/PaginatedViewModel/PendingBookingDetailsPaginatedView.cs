using DataAccess.ViewModel.admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel.PaginatedViewModel
{
    public class PendingBookingDetailsPaginatedView
    {
        public IEnumerable<BookingAllDetailsView> bookingAllDetailsViews { get; set; }
        public int PageSize { get; set; } = 5;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int EmpId { get; set; }
        public string OrderBy { get; set; }
    }
}
