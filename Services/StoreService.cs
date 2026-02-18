using CleanMvcApp.Models.Entities;
using CleanMvcApp.Models.Enums;
using CleanMvcApp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CleanMvcApp.Services
{
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StoreService> _logger;

        public StoreService(IUnitOfWork unitOfWork, ILogger<StoreService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Store> CreateStoreAsync(Store store)
        {
            try
            {
                // Set initial status to Pending
                store.Status = StoreStatus.Pending;
                store.IsActive = false;
                store.CreatedAt = DateTime.UtcNow;
                store.UpdatedAt = DateTime.UtcNow;

                var createdStore = await _unitOfWork.Stores.AddAsync(store);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Store {StoreName} created with ID {StoreId} by owner {OwnerId}", 
                    store.StoreName, createdStore.StoreId, store.OwnerId);

                return createdStore;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating store {StoreName}", store.StoreName);
                throw;
            }
        }

        public async Task<Store?> GetStoreByIdAsync(int storeId)
        {
            return await _unitOfWork.Stores.GetByIdAsync(storeId);
        }

        public async Task<IEnumerable<Store>> GetStoresByOwnerAsync(string ownerId)
        {
            return await _unitOfWork.Stores.FindAsync(s => s.OwnerId == ownerId);
        }

        public async Task<IEnumerable<Store>> GetNearbyStoresAsync(decimal latitude, decimal longitude, decimal radiusKm)
        {
            // Get all active and approved stores
            var stores = await _unitOfWork.Stores.FindAsync(s => 
                s.Status == StoreStatus.Approved && s.IsActive);

            // Filter by distance using Haversine formula
            var nearbyStores = stores.Where(s =>
            {
                if (!s.Latitude.HasValue || !s.Longitude.HasValue)
                    return false;
                    
                var distance = CalculateDistance(latitude, longitude, s.Latitude.Value, s.Longitude.Value);
                return distance <= (double)radiusKm;
            }).ToList();

            _logger.LogInformation("Found {Count} stores within {Radius}km of ({Lat}, {Lon})", 
                nearbyStores.Count, radiusKm, latitude, longitude);

            return nearbyStores;
        }

        public async Task<IEnumerable<Store>> GetAllStoresAsync()
        {
            return await _unitOfWork.Stores.GetAllAsync();
        }

        public async Task<IEnumerable<Store>> GetPendingStoresAsync()
        {
            return await _unitOfWork.Stores.FindAsync(s => s.Status == StoreStatus.Pending);
        }

        public async Task UpdateStoreAsync(Store store)
        {
            try
            {
                store.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Stores.UpdateAsync(store);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Store {StoreId} updated", store.StoreId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating store {StoreId}", store.StoreId);
                throw;
            }
        }

        public async Task<bool> ApproveStoreAsync(int storeId)
        {
            try
            {
                var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
                if (store == null)
                {
                    _logger.LogWarning("Store {StoreId} not found for approval", storeId);
                    return false;
                }

                if (store.Status != StoreStatus.Pending)
                {
                    _logger.LogWarning("Store {StoreId} is not in Pending status", storeId);
                    return false;
                }

                store.Status = StoreStatus.Approved;
                store.IsActive = true;
                store.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Stores.UpdateAsync(store);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Store {StoreId} approved", storeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving store {StoreId}", storeId);
                throw;
            }
        }

        public async Task<bool> RejectStoreAsync(int storeId, string reason)
        {
            try
            {
                var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
                if (store == null)
                {
                    _logger.LogWarning("Store {StoreId} not found for rejection", storeId);
                    return false;
                }

                if (store.Status != StoreStatus.Pending)
                {
                    _logger.LogWarning("Store {StoreId} is not in Pending status", storeId);
                    return false;
                }

                store.Status = StoreStatus.Rejected;
                store.IsActive = false;
                store.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Stores.UpdateAsync(store);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Store {StoreId} rejected. Reason: {Reason}", storeId, reason);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting store {StoreId}", storeId);
                throw;
            }
        }

        public async Task<bool> ToggleStoreStatusAsync(int storeId)
        {
            try
            {
                var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
                if (store == null)
                {
                    _logger.LogWarning("Store {StoreId} not found for status toggle", storeId);
                    return false;
                }

                if (store.Status != StoreStatus.Approved)
                {
                    _logger.LogWarning("Cannot toggle status for store {StoreId} - not approved", storeId);
                    return false;
                }

                store.IsActive = !store.IsActive;
                store.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Stores.UpdateAsync(store);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Store {StoreId} status toggled to {IsActive}", storeId, store.IsActive);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling store {StoreId} status", storeId);
                throw;
            }
        }

        public async Task<bool> DeleteStoreAsync(int storeId)
        {
            try
            {
                var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
                if (store == null)
                {
                    _logger.LogWarning("Store {StoreId} not found for deletion", storeId);
                    return false;
                }

                await _unitOfWork.Stores.DeleteAsync(store);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Store {StoreId} deleted", storeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting store {StoreId}", storeId);
                throw;
            }
        }

        // Haversine formula to calculate distance between two points on Earth
        private double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            const double R = 6371; // Earth's radius in kilometers

            var dLat = ToRadians((double)(lat2 - lat1));
            var dLon = ToRadians((double)(lon2 - lon1));

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c;

            return distance;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
