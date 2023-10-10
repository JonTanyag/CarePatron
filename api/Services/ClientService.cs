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
            if (!ValidateEmail(client.Email))
                throw new Exception("Email address provided is not valid.");

            await _clientRepository.Create(client);
        }

        public async Task<Client[]> Search(string searchParam)
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

        public async Task<Client[]> Get()
        {
            var clients = await _clientRepository.Get();

            return clients;
        }

        public async Task Update(Client client)
        {
            if (!ValidateEmail(client.Email))
                throw new Exception("Email address provided is not valid.");

            await _clientRepository.Update(client);
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
    }
}

