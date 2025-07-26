using CsvHelper.Configuration;
using DataAccess.ViewModel.admin;
using System.Globalization;

namespace Meeting_Scheduling_System.Helper
{
    public sealed class UserViewMap:ClassMap<UserView>
    {
        public UserViewMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.IsDuplicate).Ignore();
        }
    }
}
