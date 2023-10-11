using System;
using System.Text.RegularExpressions;
using api.Models;
using api.Repositories;

namespace api.Services
{
	public interface IClientService
	{
        Task<Client[]> Search(string searchParam);
        Task<Client[]> Get();
        Task<Client?> GetById(string id);
        Task Create(Client client);
        Task Update(Client client);
    }

	public class ClientService : IClientService
	{
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
		{
            _clientRepository = clientRepository;
		}

        public async Task Create(Client client)
        {
            try
            {
                if (!ValidateEmail(client.Email))
                    throw new Exception("Email address provided is not valid.");

                await _clientRepository.Create(client);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<Client[]> Search(string searchParam)
        {
            try
            {
                var clients = await _clientRepository.Get();

                if (clients != null)
                {
                    var results = clients.Where(x => x.FirstName.Contains(searchParam, StringComparison.OrdinalIgnoreCase)
                        || x.LastName.Contains(searchParam, StringComparison.OrdinalIgnoreCase)).ToArray();
                    return results;
                }

                return new Client[0];
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public async Task<Client[]> Get()
        {
            var clients = await _clientRepository.Get();

            return clients;
        }

        public async Task<Client?> GetById(string id)
        {
            var client = await _clientRepository.GetById(id);

            if (client == null)
                return null;

            return client;
        }

        public async Task Update(Client client)
        {
            try
            {
                if (!ValidateEmail(client.Email))
                    throw new Exception("Email address provided is not valid.");

                await _clientRepository.Update(client);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private bool ValidateEmail(string email)
        {
            // Define a regular expression pattern for email validation
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            // Create a Regex object and match the email against the pattern
            Regex regex = new Regex(pattern);
            Match match = regex.Match(email);

            // Check if the email matches the pattern
            return match.Success;
        }

        //
        //private async Task ExecuteEmailAndDocumentRepository(Client client)
        //{
        //    await _emailRepository.Send(client.Email, "Hi there - welcome to my Carepatron portal.");
        //    await _documentRepository.SyncDocumentsFromExternalSource(client.Email);
        //}
    }
}

