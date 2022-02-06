
using Microsoft.Azure.Cosmos.Table;

namespace FunctionAppAM
{
    public class CCApplication  : TableEntity
    {  
        public string Name { get; set; }  
        public int Age { get; set; }  
        public string Occupation { get; set; }  
        public float YearlyIncome { get; set; }  
    } 
}