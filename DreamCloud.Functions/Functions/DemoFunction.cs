using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DreamCloud.Functions.Functions
{
    public class DemoFunction
    {
        private IConfiguration Configuration { get; }

        public DemoFunction(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [FunctionName("Function1")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var kvSecret = Configuration["demokey"];
            log.LogInformation($"The secret 'demokey': '{kvSecret}'");
        }
    }
}
