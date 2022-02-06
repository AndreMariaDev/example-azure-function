using System;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionAppAM
{
    public class ProcessCCApplicationQueue
    {
        [FunctionName("ProcessCCApplicationQueue")]
        public void Run
        (
            [QueueTrigger("amqueueapplication", Connection = "AzureWebJobsStorage")] string ccApplication,
            [Table("AMCCAplicationTable")] CloudTable cloudTable,
            [Blob("rejected-application/{rand-guid}")] out string rejectedCCApplication,
            ILogger log
        )
        {
            var data = JsonConvert.DeserializeObject<CCApplication>(ccApplication);
            log.LogInformation($"C# Queue trigger function processed: {data.Name}");
            bool isApplicationAccepted = data.YearlyIncome > 10000 && data.Age > 23;

            if (isApplicationAccepted)  
            {  

                data.PartitionKey = "amccmessage";
                data.RowKey = Guid.NewGuid().ToString();

                var tabOperation = TableOperation.Insert(data);
                cloudTable.ExecuteAsync(tabOperation);
                rejectedCCApplication = null;  
            }  
            else  
            {  
                rejectedCCApplication = JsonConvert.SerializeObject(data); 
            } 

            log.LogInformation($"C# Queue trigger function processed: {data.Name}");
        }
    }
}
