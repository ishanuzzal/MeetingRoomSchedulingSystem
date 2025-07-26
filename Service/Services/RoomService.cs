using AutoMapper;
using DataAccess.Entities;
using DataAccess.Interfaces;
using DataAccess.Repository;
using DataAccess.CustomException;
using Service.Interfaces;
using DataAccess.ViewModel.Room;
using Microsoft.Extensions.Logging;
using QRCoder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing.Constraints;
using DataAccess.Attributes;
using Microsoft.Extensions.Caching.Distributed;
using DataAccess.Extension;

namespace Service.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository; 
        private readonly IBookingRepository _bookingRepository; 
        private readonly IMapper _mapper;
        private readonly ILogger<RoomService> _logger;  
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDistributedCache _cache;
        public RoomService(IRoomRepository roomRepository, IBookingRepository bookingRepository, IMapper mapper,
            ILogger<RoomService> logger, IWebHostEnvironment webHostEnvironment, IDistributedCache cache
            )
        {
            _roomRepository = roomRepository;
            _bookingRepository = bookingRepository; 
            _mapper = mapper;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _cache = cache;
        }
        public async Task AddAsync(AddRoomView addRoomView,string hostAddress)
        {
            try
            {
                var checkroom = await _roomRepository.GetAsync(e => e.Name == addRoomView.RoomName);

                if (checkroom != null)
                {
                    throw new EntityAlreadyExistException("Room Already Exist");
                }

                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                string imagefileName =  $"{Guid.NewGuid().ToString()}_"+addRoomView.Image.FileName;
                string imageFilePath = Path.Combine(uploadFolder, imagefileName);

                using (var fileStream = new FileStream(imageFilePath, FileMode.Create))
                {
                    await addRoomView.Image.CopyToAsync(fileStream);
                }

                var qrGenerator = new QRCodeGenerator();
                var qrCodeIdentifier = Guid.NewGuid().ToString();   
                var qrCodeDataObject = qrGenerator.CreateQrCode(new Url($"https://{hostAddress}/Room/ShowRoom/{qrCodeIdentifier}"), QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeDataObject);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);
                string qrCodeFileName = $"{addRoomView.RoomName}.png";
                string qrCodeFilePath = Path.Combine(uploadFolder, qrCodeFileName);
                File.WriteAllBytes(qrCodeFilePath, qrCodeBytes);

                var model = _mapper.Map<Room>(addRoomView);
                model.Name = addRoomView.RoomName.Trim();
                model.Facilities = addRoomView.Facilities.Trim();
                model.ImageName = imagefileName;
                model.QRCode = qrCodeFileName;
                model.QrCodeIdentifier = qrCodeIdentifier;  
                _roomRepository.AddAsync(model);
                await _roomRepository.Commit();

                await _cache.RemoveAsync("rooms:all");
            }
            catch (EntityAlreadyExistException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task DeleteAsync(int roomId)
        {
            try
            {
                var HaveEntity = await _roomRepository.GetAsync(e => e.Id == roomId);

                if (HaveEntity == null)
                {
                    throw new NullReferenceException("No Room found");
                }
                
                var HaveBookingInThisRoom = await _bookingRepository.CheckHaveAny(e => e.RoomId == roomId);

                if (HaveBookingInThisRoom == true)
                {
                    throw new DeleteingRoomHaveActiveMeetingException("Room have bookings");
                }

                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                if (!string.IsNullOrEmpty(HaveEntity.ImageName))
                {
                    string imageFilePath = Path.Combine(uploadFolder, HaveEntity.ImageName);
                    if (File.Exists(imageFilePath))
                    {
                        File.Delete(imageFilePath);
                    }
                }
                else
                {
                    throw new NullReferenceException("Resource not found");
                }

                if (!string.IsNullOrEmpty(HaveEntity.QRCode))
                {
                    string qrCodeFilePath = Path.Combine(uploadFolder, HaveEntity.QRCode);
                    if (File.Exists(qrCodeFilePath))
                    {
                        File.Delete(qrCodeFilePath);
                    }
                }
                else
                {
                    throw new NullReferenceException("Resource not found");
                }

                _roomRepository.DeleteAsync(HaveEntity);
                await _cache.RemoveAsync("rooms:all");
                await _roomRepository.Commit();

            }
            catch (NullReferenceException)
            {
                throw;
            }
            catch (DeleteingRoomHaveActiveMeetingException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<ShowRoomView> GetAsync(int roomId)
        {
            try
            {
                var model = await _roomRepository.GetAsync(u => u.Id == roomId);

                if (model == null)
                {
                    throw new NullReferenceException("Room Not found");
                }

                var showRoomView = _mapper.Map<ShowRoomView>(model);

                return showRoomView;
            }
            catch(NullReferenceException)
            {
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            } 
        }

        public async Task<ShowRoomView> GetByQrIdentifierAsync(string qrIdentifier)
        {
            try
            {
                var model = await _roomRepository.GetAsync(u => u.QrCodeIdentifier==qrIdentifier);

                if (model == null)
                {
                    throw new NullReferenceException("Room Not found");
                }

                var showRoomView = _mapper.Map<ShowRoomView>(model);

                return showRoomView;
            }
            catch (NullReferenceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<List<ShowRoomView>> GetListAsync()
         {
            try
            {
                var cacheKey = "rooms:all";
                var allRooms = await _cache.GetOrSetAsync(cacheKey, () => _roomRepository.GetListAsync());

                if (allRooms==null || allRooms.Count==1)
                {
                    return new List<ShowRoomView>();
                }

                var ShowAllRoom = new List<ShowRoomView>();

                foreach (var room in allRooms)
                {
                    var showRoom = _mapper.Map<ShowRoomView>(room);
                    ShowAllRoom.Add(showRoom);
                }

                return ShowAllRoom; 
            }
            catch(Exception ex) {
                _logger.LogError(ex.Message);   
                throw;
            }
        }
        public async Task UpdateAsync(UpdateRoomView updateRoomView, string hostAddress)
        {
            try
            {
                var entity = await _roomRepository.GetAsync(e => e.Id == updateRoomView.Id);

                if (entity == null)
                {
                    throw new NullReferenceException("No Room found");
                }

                var checkroom = await _roomRepository.CheckHaveAny(e => e.Id!=updateRoomView.Id && e.Name == updateRoomView.RoomName);

                if (checkroom == true)
                {
                    throw new EntityAlreadyExistException("Room Already Exist with that name");
                }

                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                _mapper.Map(updateRoomView, entity);

                if (updateRoomView.Photo!=null && updateRoomView.Photo.FileName != entity.ImageName)
                {
                    string fileName = $"{Guid.NewGuid()}_" + updateRoomView.Photo.FileName; 
                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await updateRoomView.Photo.CopyToAsync(fileStream);
                    }

                    entity.ImageName = fileName;
                }

                _roomRepository.UpdateAsync(entity);
                await _roomRepository.Commit();
                await _cache.RemoveAsync("rooms:all");
            }
            catch(EntityAlreadyExistException)
            {
                throw;
            }
            catch (NullReferenceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
