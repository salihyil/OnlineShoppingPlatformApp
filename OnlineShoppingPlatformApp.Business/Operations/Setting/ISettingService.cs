namespace OnlineShoppingPlatformApp.Business.Operations.Setting
{
    public interface ISettingService
    {
        Task ToggleMaintenance();
        bool GetMaintenanceState();
    }
}