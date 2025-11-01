using APIBanca.Models;


namespace APIBanca.Services
{
    public class CreateCardService
    {
        private readonly CreateCardRepository _repository;
        private readonly EncryptionProtect _encryptionProtect;

        public CreateCardService(CreateCardRepository repository, EncryptionProtect encryptionProtect)
        {
            _repository = repository;
            _encryptionProtect = encryptionProtect;
        }

        public async Task<Guid> CrearTarjetaAsync(CreateCard card)
        {
            Console.WriteLine("Encriptando " + card.cvv_encriptado + " y " + card.pin_encriptado);
            card.cvv_encriptado = _encryptionProtect.Encrypt(card.cvv_encriptado);
            card.pin_encriptado = _encryptionProtect.Encrypt(card.pin_encriptado);
            Console.WriteLine("Encriptados " + card.cvv_encriptado + " y " + card.pin_encriptado);

           return await _repository.CreateCard(card);
        }
    }
}