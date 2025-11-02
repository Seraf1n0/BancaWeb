using APIBanca.Models;


namespace APIBanca.Services
{
    public class OtpCreateService
    {
        private readonly OtpCreateRepository _repository;

        public OtpCreateService(OtpCreateRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> OtpCreate(OtpCreateM otp) {
            return await _repository.OtpCreate(otp);
        }

    }
}