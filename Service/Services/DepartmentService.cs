using DataAccess.Entities;
using DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class DepartmentService:IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<DepartmentService> _logger;    

        public DepartmentService(IDepartmentRepository departmentRepository, ILogger<DepartmentService> logger)
        {
            _departmentRepository = departmentRepository;   
            _logger = logger;
        }
        public async Task<List<Department>> GetListAsync()
        {
            try
            {
                var department = await _departmentRepository.GetListAsync();

                if (department == null)
                {
                    throw new NullReferenceException("no department found");
                }

                return department;
            }
            catch(NullReferenceException) {
                throw;
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
