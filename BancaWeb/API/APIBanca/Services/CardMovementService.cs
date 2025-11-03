using APIBanca.Models;

namespace APIBanca.Services
{
    public class CardMovementService
    {
        private readonly CardMovementRepository _repository;

        public CardMovementService(CardMovementRepository repository)
        {
            _repository = repository;
        }

        public async Task<(Guid, decimal)> CardMovement(CardMovement card)
        {
            return await _repository.CardMovement(card);
        }
    }
}