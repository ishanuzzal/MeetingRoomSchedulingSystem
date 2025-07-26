using DataAccess.Entities;
using DataAccess.Interfaces;
using DataAccess.Repository;
using Microsoft.Extensions.Logging;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class DesignationService : IDesignationService
    {
        private readonly IDesignationRepository _designationRepository; 
        private readonly ILogger<DesignationService> _logger;   

        public DesignationService(IDesignationRepository designationRepository, ILogger<DesignationService> logger)
        {
            _designationRepository = designationRepository;
            _logger = logger;
        }
        public async Task<List<Designation>> GetListAsync()
        {
            try
            {
                var department = await _designationRepository.GetListAsync();

                if (department == null)
                {
                    throw new NullReferenceException("no department found");
                }

                return department;
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
