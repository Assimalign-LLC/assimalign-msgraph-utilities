using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace Assimalign.Msgraph.Server.Abstraction
{

    using Assimalign.AspNetCore.Mvc.JsonApi;


    public abstract class ControllerBaseShared : ControllerBase
    {

        public Func<string, string> GetSerializedDriveItem = (name) =>
        {
            return JsonConvert.SerializeObject(new DriveItem()
            {
                Name = name,
                Folder = new Folder(),
                AdditionalData = new Dictionary<string, object>()
                {
                    {"@microsoft.graph.conflictBehavior", "replace"}
                }
            });
        };


        public partial class NewtonsoftJsonApi : IResponseSerializer
        {
            public async Task SerializeAsync(Stream stream, object response)
            {
                var task = Task.Run(() =>
                {
                    var serializer = new JsonSerializer();
                    serializer.NullValueHandling = NullValueHandling.Ignore;

                    using (StreamWriter streamWriter = new StreamWriter(stream))
                    {
                        using (JsonWriter writer = new JsonTextWriter(streamWriter))
                        {
                            serializer.Serialize(writer, response);
                        }
                    }
                });

                await task;
            }
        }
    }
}
