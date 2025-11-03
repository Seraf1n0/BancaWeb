using APIBanca.Models;

namespace APIBanca.Services
{
    public class OtpConsumeService
    {
        private readonly OtpConsumeRepository _repository;

        public OtpConsumeService(OtpConsumeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Boolean> OtpConsume(OtpConsumeM otp)
        {
            return await _repository.OtpConsume(otp);
        }
    }
}