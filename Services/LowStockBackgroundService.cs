using IMS.Data;
using IMS.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IMS.Services
{
    public class LowStockBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        // IServiceScopeFactory because this service is singleton
        // DbContext is scoped — can't inject directly into singleton
        // must create a scope manually each time we need the DB
        public LowStockBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckLowStockAsync();
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task CheckLowStockAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // get all products where current stock is below reorder level
            var lowStockProducts = context.Products
                .Where(p => p.Quantity < p.ReorderLevel)
                .ToList();

            foreach (var product in lowStockProducts)
            {
                // check if alert already exists for today
                // avoids duplicate alerts for same product on same day
                var alreadyAlerted = context.LowStockAlerts.Any(a =>
                    a.ProductId == product.Id &&
                    a.AlertDate.Date == DateTime.UtcNow.Date);

                if (!alreadyAlerted)
                {
                    context.LowStockAlerts.Add(new LowStockAlert
                    {
                        ProductId = product.Id,
                        CurrentStock = product.Quantity,
                        ReorderLevel = product.ReorderLevel,
                        AlertDate = DateTime.UtcNow
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}