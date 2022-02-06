using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionAppAM
{
    public static class ApplyForCCApp
    {
        [FunctionName("ApplyForCCApp")]
        public static async Task<IActionResult> Run
        (
            [HttpTrigger(AuthorizationLevel.Function,"post", Route = null)] HttpRequest req,
            [Queue("amqueueapplication")] IAsyncCollector<CCApplication> applicationQueue, 
            ILogger log
        )
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CCApplication ccApplication = JsonConvert.DeserializeObject<CCApplication>(requestBody);

                ccApplication.PartitionKey = "amccmessage";
                ccApplication.RowKey = Guid.NewGuid().ToString();

            await applicationQueue.AddAsync(ccApplication); 

            string responseMessage = string.IsNullOrEmpty(ccApplication.Name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {ccApplication.Name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
