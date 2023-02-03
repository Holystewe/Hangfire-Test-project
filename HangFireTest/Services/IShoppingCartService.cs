using Hangfire;

namespace HangFireTest.Services
{
    public interface IShoppingCartService
    {
        [AutomaticRetry(Attempts = 0)] // disabilito il retry, e va inserito nell'interfaccia, dato che con HF creo questo metodo attraverso l'interfaccia
        Task ChechoutAsync();
        Task CleanupAsync();
    }
}