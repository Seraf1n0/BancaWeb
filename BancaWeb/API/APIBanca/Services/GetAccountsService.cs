using APIBanca.Models;
using APIBanca.Repositories;

namespace APIBanca.Services
{
    public class GetAccountService
    {
        private readonly GetAccountRepository _repository;

        public GetAccountService(GetAccountRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<Cuenta>> GetAccounts(string? userId, string? accountId)
        {
            return await _repository.GetAccountsAsync(userId, accountId);
        }
    }
}