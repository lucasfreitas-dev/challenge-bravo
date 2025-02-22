using System;
using api_challenge_bravo.Model;
using System.Threading;
using System.Threading.Tasks;
using api_challenge_bravo.Services.Util.ExternalCurrencyAPI;

namespace api_challenge_bravo.Services
{
    public static class ExchangeRateUpdateService
    {
        private const int TIME_TO_LIVE_EXCHANGE_RATE_SECONDS = 30;
        private const int MAX_CONCURRENT_THREADS = 2;

        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1,MAX_CONCURRENT_THREADS);
        
        public static async Task CheckTTLForNewUpdate(string symbol)
        {
            // Only two threads by time can check if the currency need an update, making this a thread-safe logic:
            // avoiding flooding the external API when we got a lot of simultaneous requests
            await semaphore.WaitAsync();
            try
            {
                var currency = Currency.GetCached(symbol);

                if (!currency.AutoUpdateExchangeRate)
                    return;

                var isCurrencyTTLExpired = currency.LastTimeUpdatedExchangeRateUTC <=
                                           DateTime.UtcNow.AddSeconds(-TIME_TO_LIVE_EXCHANGE_RATE_SECONDS);
                
                if (isCurrencyTTLExpired)
                    await UpdateExchangeRate(currency);
            }
            finally
            {
                semaphore.Release();
            }
        }
        private static async Task UpdateExchangeRate(Currency currency)
        {
            decimal newExchangeRate;
            DateTime dateTimeUpdate;

            try
            {
                (newExchangeRate, dateTimeUpdate) = await ExternalCurrencyAPI.GetExchangeRate(currency.Symbol);
            }
            catch (Exception exception)
            {
                throw new Exception("External API error:", exception);
            }

            currency.UpdateExchangeRate(newExchangeRate, dateTimeUpdate);
        }
    }
}