using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parbad.Builder;
using Parbad.Gateway.ZarinPal;
using Parbad.Gateway.Zibal;

namespace WebFramework.Configuration
{
    public static class PaymentGatewayConfigurationExtensions
    {
        public static IServiceCollection AddZibalPaymentGateway(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddParbad()
            .ConfigureHttpContext(builder => builder.UseDefaultAspNetCore())
            .ConfigureStorage(builder => builder.UseMemoryCache())
            .ConfigureGateways(gateways =>
            {

                gateways.AddZibal()
                .WithAccounts(c =>
                {
                    c.AddInMemory(acc =>
                    {
                        acc.Merchant = configuration["Zibal:Merchant"];
                        acc.IsSandbox = false;
                        acc.Name = configuration["Zibal:Name"];
                    });
                });
            })
            .ConfigureAutoRandomTrackingNumber(options =>
            {
                options.MinimumValue = 1000;
            });

            return services;
        }
        public static IServiceCollection AddZarinPalPaymentGateway(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddParbad()
                .ConfigureHttpContext(builder => builder.UseDefaultAspNetCore())
                .ConfigureStorage(builder => builder.UseMemoryCache())
                .ConfigureGateways(gateways =>
                {
                    gateways.AddZarinPal()
                        .WithAccounts(accounts =>
                        {
                            accounts.AddInMemory(account =>
                            {
                                account.MerchantId = configuration["ZarinPal:MerchantId"];
                                account.IsSandbox = configuration.GetValue<bool>("ZarinPal:IsSandbox");
                                account.Name = configuration["ZarinPal:Name"] ?? "ZarinPal Gateway";
                            });
                        });
                })
                .ConfigureAutoRandomTrackingNumber(options =>
                {
                    options.MinimumValue = 1000;
                });

            return services;
        }
    }
}
