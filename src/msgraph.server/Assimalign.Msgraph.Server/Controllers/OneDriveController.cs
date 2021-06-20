using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Assimalign.Msgraph.Server.Controllers
{
    using Assimalign.Msgraph.Server.Options;
    using Assimalign.Msgraph.Server.Abstraction;
    using Assimalign.Msgraph.Server.Attributes;
    using Assimalign.Msgraph.Server.Extensions;
    using Assimalign.AspNetCore.Mvc.JsonApi;
    using Assimalign.AspNetCore.Mvc.JsonApi.Generics;

    using Microsoft.AspNetCore.Http.Features;
    using System.Threading;

    [ApiController, Route("api/[controller]")]
    public class OneDriveController : ControllerBaseShared
    { 
        private readonly DriveStructure Folders;
        private readonly IGraphServiceClient Client;

        public OneDriveController(
            IGraphServiceClient client, 
            IOptions<DriveStructure> folders)
        {
            Client = client;
            Folders = folders.Value;
        }



        /// <summary>
        /// Builds the managed directory of resources will be placed in for the 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("{user}/drive/build")]
        public async Task<IActionResult> PostManagedFoldersStructureAsync(
           [FromRoute] string user)
        {
            try
            {
                var content = new BatchRequestContent();
                var successes = new List<DriveItem>();
                var errors = new List<Error>();

                foreach (var folder in Folders.GetFolderCollection())
                {
                    HttpRequestMessage request;
                    var dependencies = new List<string>();
                    var item = GetSerializedDriveItem(folder.Key);

                    if (folder.Key == folder.Value)
                        request = Client.Users[user].Drive.Root.Children.Request().GetHttpRequestMessage();
                    else
                    {
                        request = Client.Users[user].Drive.Root.ItemWithPath(folder.Value).Children.Request().GetHttpRequestMessage();
                        dependencies.Add(folder.Value.Split('/').Last());
                    }

                    // Set the Request Method and Content
                    request.Method = HttpMethod.Post;
                    request.Content = new StringContent(item, Encoding.UTF8, "application/json");

                    // Add the Batch step to the request
                    content.AddBatchRequestStep(new BatchRequestStep(folder.Key, request, dependencies.Count > 0 ? dependencies : null));
                }

                // Make Response
                var responses = await Client.Batch.Request().PostAsync(content);

                foreach(var response in await responses.GetResponsesAsync())
                {
                    var jsonResponse = await response.Value.Content.ReadAsStringAsync();

                    if (response.Value.IsSuccessStatusCode)
                    {
                        var success = JsonConvert.DeserializeObject<DriveItem>(jsonResponse);
                        successes.Add(success);
                    }
                    else
                    {
                        var error = JsonConvert.DeserializeObject<Error>(jsonResponse);
                        errors.Add(error);
                    }
                }

                return OkCreatedResponse.Create<DriveItem>(successes, id => id.Id)
                    .WithMeta("successCount", successes.Count)
                    .WithMeta("errorCount", errors.Count)
                    .WithMeta("errors", errors)
                    .UseSerializer<NewtonsoftJsonApi>();

            }
            catch(ClientException exception)
            {
                return BadRequestResponse.Create(exception, code => code.HResult.ToString());
            }
            catch(Exception exception)
            {
                return BadRequestResponse
                    .Create(exception, code => code.HResult.ToString(), HttpStatusCode.InternalServerError);
            }
        }



        // 10 GB
        private const long MaxFileSize = 10L * 1024L * 1024L * 1024L;

        [HttpPost("{user}/drive/item")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> PostItemAsync(
            [FromRoute] string user)
        {
            try
            {
                var sliceSize = 320 * 1024;
                var mediaHeader = MediaTypeHeaderValue.Parse(Request.ContentType);
                var boundary = MultipartRequestHelper.GetBoundary(mediaHeader, sliceSize);


                var reader = new MultipartReader(boundary, HttpContext.Request.Body);
                var section = await reader.ReadNextSectionAsync();

                
                var session = await Client.Users[user].Drive.Root
                    .ItemWithPath($".assimalign.apps/test")
                    .CreateUploadSession()
                    .Request()
                    .PostAsync();

                while (section != null)
                {
                    var stream = section.Body;
                    var sessionTask = new LargeFileUploadTask<DriveItem>(session, stream, sliceSize);
                    
                    IProgress<long> progress = new Progress<long>(status =>
                    {

                    });

                    var results = await sessionTask.UploadAsync(progress);

                    section = await reader.ReadNextSectionAsync();

                }

                return new OkResult();

            } 
            catch(Exception exception)
            {
                return new BadRequestResult();
            }
        }

        [HttpGet("{user}/drive/items")]
        public async Task<IActionResult> GetVideoItemsAsync(
            [FromRoute] string user )
        {
            try
            {

                //Client.Users[user].Drive.Root.ItemWithPath("").Children
                var items = await Client.Users[user].Drive.Root.ItemWithPath(".assimalign.apps/.videos").Children.Request().GetAsync();

                return OkObjectResponse.Create<DriveItem>(items.CurrentPage, id=>id.Id);
            }
            catch(Exception exception)
            {
                return BadRequestResponse.Create(exception);
            }
        }
       
    }
}
