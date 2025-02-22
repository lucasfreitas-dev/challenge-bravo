using api_challenge_bravo.Model.Util;
using api_challenge_bravo.Services.Util.ExternalCurrencyAPI;
using tests_challenge_bravo.UnitTests.Mocks;

namespace tests_challenge_bravo.UnitTests
{
    public class UnitTestBase
    {
        public UnitTestBase()
        {
            // Set BD to inMemory option
            AppDbContext.SetTestingEnvironment();
            ExternalCurrencyAPI.Registry(new MockAPI());
        }
    }
}