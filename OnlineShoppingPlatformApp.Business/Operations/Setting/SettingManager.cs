using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingPlatformApp.Data.Entities;
using OnlineShoppingPlatformApp.Data.Repositories;
using OnlineShoppingPlatformApp.Data.UnitOfWork;

namespace OnlineShoppingPlatformApp.Business.Operations.Setting
{
    public class SettingManager : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<SettingEntity> _settingRepository;
        public SettingManager(IUnitOfWork unitOfWork, IRepository<SettingEntity> settingRepository)
        {
            _unitOfWork = unitOfWork;
            _settingRepository = settingRepository;
        }

        public bool GetMaintenanceState()
        {
            var maintenanceMode = _settingRepository.GetById(1).MaintenanceMode;
            return maintenanceMode;
        }

        public async Task ToggleMaintenance()
        {
            var setting = _settingRepository.GetById(1);
            setting.MaintenanceMode = !setting.MaintenanceMode;
            _settingRepository.Update(setting);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Bakım durumu değiştirilirken bir hata oluştu.");
            }
        }
    }
}