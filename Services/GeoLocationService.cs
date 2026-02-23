namespace CleanMvcApp.Services
{
    public class GeoLocationService : IGeoLocationService
    {
        private readonly ILogger<GeoLocationService> _logger;

        public GeoLocationService(ILogger<GeoLocationService> logger)
        {
            _logger = logger;
        }

        public double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
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

        public async Task<(decimal Latitude, decimal Longitude)?> GeocodeAddressAsync(string address)
        {
            // Simple implementation - in production, integrate with Google Maps API or similar
            // For now, return null to indicate geocoding is not available
            _logger.LogWarning("Geocoding not implemented. Address: {Address}", address);
            return await Task.FromResult<(decimal, decimal)?>(null);
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
