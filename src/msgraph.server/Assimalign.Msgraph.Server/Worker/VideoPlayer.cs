

using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;


namespace Assimalign.Msgraph.Server.Worker
{
    public class VideoPlayer : BackgroundService
    {

        public VideoPlayer()
        {

        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var streamer = new VideoStreamer();

            
        }


        public partial class VideoStreamer
        {

        }
    }
}
