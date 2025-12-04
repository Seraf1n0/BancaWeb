using APIBanca.Models;
using APIBanca.Repositories;

namespace APIBanca.Services
{
    public class InterbankTransferService
    {
        private readonly InterbankTransferRepository _repository;
        private readonly ILogger<InterbankTransferService> _logger;

        public InterbankTransferService(
            InterbankTransferRepository repository,
            ILogger<InterbankTransferService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<InterbankTransferResponse> TransferInterbank(
            InterbankTransferRequest req, 
            Guid userId)
        {
            _logger.LogInformation($"🚀 Iniciando transferencia interbancaria de {req.from} a {req.to}");

            // Por ahora solo llamamos al repositorio
            // Más adelante agregaremos la lógica de WebSocket aquí
            return await _repository.TransferInterbankAsync(
                req.from, 
                req.to, 
                req.amount, 
                req.currency, 
                req.description, 
                userId);
        }
    }
}
