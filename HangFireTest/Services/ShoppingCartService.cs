namespace HangFireTest.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ILogger<ShoppingCartService> _logger;
        public ShoppingCartService(ILogger<ShoppingCartService> logger)
        {
            _logger = logger;
        }

        public async Task ChechoutAsync()
        {
            _logger.LogInformation("Starting checkout...");

            await Task.Delay(1000);

            //throw new Exception("Incredible Exception!"); // Hangfire ritenta l'esecuzione di un Job in caso venisse lanciata una eccezione

            _logger.LogInformation("Checkout completed");
        }

        public async Task CleanupAsync()
        {
            _logger.LogInformation("Starting carts cleanup...");

            await Task.Delay(5000);

            _logger.LogInformation("Cleanup completed");
        }
    }
}
