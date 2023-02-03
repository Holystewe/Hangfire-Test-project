using Hangfire;
using Hangfire.SqlServer;
using HangFireTest.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Aggiungo Hangfire ai servizi e imposto le GlobalConfiguration
builder.Services.AddHangfire(configuration =>

    configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170) // allineamento dei servizi alla stessa versione di Hf
    .UseSimpleAssemblyNameTypeSerializer() // impostazioni di default consigliate
    .UseRecommendedSerializerSettings()

    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions()
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }) // imposto il server da utilizzare
    //.WithJobExpirationTimeout(TimeSpan.FromMinutes(5)) quando scade un job
);

builder.Services.AddHangfireServer(); // aggiungo l'hosted service

builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>(); // aggiungo il servizio creato per testare

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// La dashboard, di default, e' accessibile soltanto dal LocalHost, ma posso renderla sicura e accessibile pubblicamente
// creando un filtro (che implementa DashboardAuthorizationFilter, fornito di default da Hf e accetta soltanto Localhost, sempre di default)
// effettuo il controllo, ovvero se il client sia autenticato, e successivamente posso accedere alla dashboard
app.UseHangfireDashboard("/jobs", new DashboardOptions()
{
    DashboardTitle = "Hangfire Dashboard Test",
    //TimeZoneResolver // serve a impostare la timezone con cui andare a risolvere i job che ci stanno dentro
}); // utilizzo semplicemente la dashboard di Hf

var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
recurringJobManager.AddOrUpdate<IShoppingCartService>("cleanup", (service) => service.CleanupAsync(), Cron.Minutely()); // posso inpostare anche ogni quanto far partire il Cron
// Si prende in ingresso il servizio, e aggiungiamo il servizio di recurring job al programma
// Anche se dovessi eliminare/commentare le due righe di codice scritte qua sopra, i recurring job rimarranno nel Database (il puntatore rimane sull'esecuzione del metodo);
// Per toglierlo, bisogna eliminarli quindi sempre con il recurringJobManager
//recurringJobManager.RemoveIfExists(); // questo permette di rimuovere il job in fase di caricamento


app.MapControllers();

app.Run();
