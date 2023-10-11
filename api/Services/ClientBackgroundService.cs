using System;
using System.Net.Mail;
using System.Reflection.Metadata;
using api.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace api.Services
{
    public class ClientBackgroundService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly IEmailRepository _emailRepository;
        private readonly IDocumentRepository _documentRepository;

        public ClientBackgroundService(IServiceProvider provider, IEmailRepository emailRepository, IDocumentRepository documentRepository)
        {
            _provider = provider;
            _emailRepository = emailRepository;
            _documentRepository = documentRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Implement your background tasks here
                await SendEmailAsync();
                await UpdateDocumentsAsync();

                // Adjust the delay as needed; this example waits for 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task SendEmailAsync()
        {
            await _emailRepository.Send("", "");
        }

        private async Task UpdateDocumentsAsync()
        {
            await _documentRepository.SyncDocumentsFromExternalSource("");
        }

        public async Task TriggerBackgroundTasksAsync()
        {
            // Manually trigger the background tasks when needed
            await SendEmailAsync();
            await UpdateDocumentsAsync();
        }
    }
}

