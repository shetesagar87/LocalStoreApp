namespace CleanMvcApp.Services;

public class ExampleService : IExampleService
{
    private readonly ILogger<ExampleService> _logger;

    public ExampleService(ILogger<ExampleService> logger)
    {
        _logger = logger;
    }

    public async Task<string> GetExampleDataAsync()
    {
        _logger.LogInformation("Getting example data");
        await Task.Delay(100); // Simulate async operation
        return "Example data from service";
    }
}
