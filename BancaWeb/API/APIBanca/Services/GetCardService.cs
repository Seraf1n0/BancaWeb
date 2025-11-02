using APIBanca.Models;

namespace APIBanca.Services
{
    public class GetCardService
    {
        private readonly GetCardRepository _repository;

        public GetCardService(GetCardRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<GetCard>> GetCard(string? userId, string? cardId)
        {
            return await _repository.GetCard(userId, cardId);
        }
    }
}