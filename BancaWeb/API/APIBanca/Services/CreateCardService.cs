using APIBanca.Models;


namespace APIBanca.Services
{
    public class CreateCardService
    {
        private readonly CreateCardRepository _repository;
        public CreateCardService(CreateCardRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> CrearTarjetaAsync(CreateCard card)
        {
            Console.WriteLine("Encriptando " + card.cvv_encriptado + " y " + card.pin_encriptado);
            card.cvv_encriptado = HashProtect.HashSecretData(card.cvv_encriptado);
            card.pin_encriptado = HashProtect.HashSecretData(card.pin_encriptado);
            Console.WriteLine("Encriptados " + card.cvv_encriptado + " y " + card.pin_encriptado);

           return await _repository.CreateCard(card);
        }
    }
}