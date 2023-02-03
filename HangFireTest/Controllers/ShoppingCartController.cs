using Hangfire;
using HangFireTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace HangFireTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;

        public ShoppingCartController(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }

        [HttpPost("checkout")]
        public IActionResult Checkout()
        {
            //var jobId = _backgroundJobClient.Enqueue<IShoppingCartService>((service) => service.ChechoutAsync());
            //var job = _backgroundJobClient.Enqueue(() => Console.WriteLine("Executed checkout operations...")); // non deve essere asincrono, dato che lo gestisce automaticamente Hanfire
            var jobId = _backgroundJobClient.Schedule<IShoppingCartService>((service) => service.ChechoutAsync(), DateTimeOffset.UtcNow.AddHours(1));
            // Con lo schedule, posso decidere quando far avviare il job desiderato, e lo mette nella coda in modo che parta in quel momento
            Console.WriteLine($"Created Job: {jobId}");

            return Accepted();
        }

        [HttpPost("schedule")]
        public IActionResult ChangeSchedule(string cronExpression)
        {
            // Aggiungo o elimino il Job
            _recurringJobManager.AddOrUpdate<IShoppingCartService>("cleanup", (service) => service.CleanupAsync(), cronExpression);
            return NoContent();
        }
    }
}