namespace CleanMvcApp.Services
{
    public interface IGeoLocationService
    {
        double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2);
        Task<(decimal Latitude, decimal Longitude)?> GeocodeAddressAsync(string address);
    }
}
