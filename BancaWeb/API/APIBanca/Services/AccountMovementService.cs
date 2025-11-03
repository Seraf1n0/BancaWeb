using APIBanca.Models;
using APIBanca.Repositories;

namespace APIBanca.Services
{
    public class AccountMovementService
    {
        private readonly AccountMovementRepository _repository;

        public AccountMovementService(AccountMovementRepository repository)
        {
            _repository = repository;
        }

        public Task<TransferInternalResponse> TransferInternal(TransferInternalRequest req, Guid userId)
            => _repository.TransferInternalAsync(req.from_account_id, req.to_account_id, req.amount, req.currency, req.descripcion, userId);
    }
}