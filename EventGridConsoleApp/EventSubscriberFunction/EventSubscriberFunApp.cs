using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Collections.Generic;
using EventGridConsoleApp;
using Newtonsoft.Json.Linq;
using System.IO;

namespace EventSubscriberFunction
{
    public static class EventSubscriberFunApp
    {
        [FunctionName("EventSubscriberFunApp")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log
            )
        {


            var payloadFromEventGrid = JToken.ReadFrom(new JsonTextReader(new StreamReader(await req.Content.ReadAsStreamAsync())));
            dynamic eventGridSoleItem = (payloadFromEventGrid as JArray)?.SingleOrDefault();
            if (eventGridSoleItem == null)
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Expecting only one item in the Event Grid message");
            }

            if (eventGridSoleItem.eventType == @"Microsoft.EventGrid.SubscriptionValidationEvent")
            {
                log.Verbose(@"Event Grid Validation event received.");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        validationResponse = ((dynamic)payloadFromEventGrid)[0].data.validationCode
                    }))
                };
            }
            log.Info("C# HTTP trigger function processed a request.");

            

                // Get request body
                dynamic data = await req.Content.ReadAsStringAsync();
            var theevents = JsonConvert.DeserializeObject<List<CustomEvent<Account>>>(data);
            System.Console.WriteLine("Event is  " +theevents);
            var resmsg = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { status = "good", code = "200" }))



            };
            log.Info("response is"  +resmsg.ToString());

            return resmsg;

        }
    }
}
