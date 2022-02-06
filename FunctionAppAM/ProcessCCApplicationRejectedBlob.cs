using System;
using System.IO;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionAppAM
{
    public class ProcessCCApplicationRejectedBlob
    {
        [FunctionName("ProcessCCApplicationRejectedBlob")]
        public void Run
        (
            [BlobTrigger("rejected-application/{name}", Connection = "AzureWebJobsStorage")]string ccApplicationJson, 
            [Table("AMCCAplicationTable")] CloudTable cloudTable,
            string name, 
            ILogger log
        )
        {
            var data = JsonConvert.DeserializeObject<CCApplication>(ccApplicationJson);
            log.LogInformation($"C# Queue trigger function processed: {data.Name}");

            data.Name = $"(rejectedCCApplication:{data.Name})";
            
            data.PartitionKey = "amccmessage";
            data.RowKey = Guid.NewGuid().ToString();

            var tabOperation = TableOperation.Insert(data);
            cloudTable.ExecuteAsync(tabOperation);

            log.LogInformation($"C# Queue trigger function processed: {data.Name}");
        }
    }
}
