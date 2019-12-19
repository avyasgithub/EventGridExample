using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace EventGridConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            {
                var eventNew = MakeRequestEvent();
                eventNew.Wait();
                Console.WriteLine(eventNew.Result.Content.ReadAsStringAsync().Result.Length);
            }
            Console.ReadKey();
        }
        private static async Task<HttpResponseMessage> MakeRequestEvent()
        {
            // Get  event grid topic end point, will get it from portal

            string endpoint = "https://mycustomevengridtopic.westus2-1.eventgrid.azure.net/api/events";

            // create data to send to event
            var customeEvent = new CustomEvent<Account>();
            customeEvent.EventType = "Test Event Type";
            customeEvent.Subject = "Test Event Subject";

            var accdetails = new Account()
            {
                Name = "Ajay",
                Gender ="Male"
            };
            customeEvent.Data = accdetails;
            List<CustomEvent<Account>> events = new List<CustomEvent<Account>>();

            events.Add(customeEvent);
            // convert it to json

            string jsonContent = JsonConvert.SerializeObject(events);

            // create object of http client
            var httpClient = new HttpClient();

            //add header
            httpClient.DefaultRequestHeaders.Add("aeg-sas-key", "bBSyVlkGlKNpP37VbEkOIbRf33Wcxgx+KizClIjxOH8=");

            // convert content into utf8 
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            //use post async 

            return await httpClient.PostAsync(endpoint, content);
        }
    }
}
