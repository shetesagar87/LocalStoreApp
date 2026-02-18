using CleanMvcApp.Models.Entities;

namespace CleanMvcApp.Services
{
    public interface IStoreService
    {
        Task<Store> CreateStoreAsync(Store store);
        Task<Store?> GetStoreByIdAsync(int storeId);
        Task<IEnumerable<Store>> GetStoresByOwnerAsync(string ownerId);
        Task<IEnumerable<Store>> GetNearbyStoresAsync(decimal latitude, decimal longitude, decimal radiusKm);
        Task<IEnumerable<Store>> GetAllStoresAsync();
        Task<IEnumerable<Store>> GetPendingStoresAsync();
        Task UpdateStoreAsync(Store store);
        Task<bool> ApproveStoreAsync(int storeId);
        Task<bool> RejectStoreAsync(int storeId, string reason);
        Task<bool> ToggleStoreStatusAsync(int storeId);
        Task<bool> DeleteStoreAsync(int storeId);
    }
}
