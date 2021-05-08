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

namespace Assimalign.OneDrive.Tooler.Controllers
{
    using Assimalign.OneDrive.Tooler.Options;
    using Assimalign.OneDrive.Tooler.Abstraction;
    using Assimalign.OneDrive.Tooler.Attributes;
    using Assimalign.OneDrive.Tooler.Extensions;
    using Assimalign.AspNetCore.Mvc.JsonApi;
    using Assimalign.AspNetCore.Mvc.JsonApi.Generics;

    [ApiController, Route("api/onedrive/videos")]
    public class OneDriveVideosController : Controller
    {

        private readonly DriveStructure Folders;
        private readonly IGraphServiceClient Client;

        public OneDriveVideosController(
            IGraphServiceClient client,
            IOptions<DriveStructure> folders)
        {
            Client = client;
            Folders = folders.Value;
        }



        [HttpGet]
        public async Task<Stream> GetVideoAsync(
            [FromQuery] Guid streamId)
        {
            try
            {
                return await Client.Me.Drive.Items["01A5XOGD6MP6QOY5VI6BGLU4GU4374USMT"].Content
                    .Request()
                    .GetAsync();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
