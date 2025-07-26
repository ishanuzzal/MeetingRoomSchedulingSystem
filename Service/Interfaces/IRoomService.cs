using DataAccess.ViewModel.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IRoomService
    {
        public Task AddAsync(AddRoomView addRoomView,string hostAddress);
        public Task<List<ShowRoomView>> GetListAsync();
        public Task<ShowRoomView> GetAsync(int roomId); 
        public Task<ShowRoomView> GetByQrIdentifierAsync(string qrIdentifier);  
        public Task UpdateAsync(UpdateRoomView updateRoomView, string hostAddress);
        public Task DeleteAsync(int roomId);
    }
}
