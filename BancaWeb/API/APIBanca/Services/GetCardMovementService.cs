using APIBanca.Models;


namespace APIBanca.Services
{
    public class GetCardMovementService
    {
        private readonly GetCardMovementRepository _repository;

        public GetCardMovementService(GetCardMovementRepository repository){
            _repository = repository;
        }

        public async Task<GetListCardMovement> CardMovement(string? card_id, DateTime? start_date, DateTime? to_date, string type,
                string? q, int page, int pageSize)
        {
            return await _repository.CardMovement(card_id, start_date, to_date, type, q, page, pageSize);
        }
    }
}