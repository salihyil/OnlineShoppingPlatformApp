using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineShoppingPlatformApp.Data.Entities
{
    public class SettingEntity : BaseEntity
    {
        //proje bakımda değilse erişime izin vericez. apiden true verirsek proje bakım moduna girecek ve erişimi engelleyecek.
        public bool MaintenanceMode { get; set; }
    }
}