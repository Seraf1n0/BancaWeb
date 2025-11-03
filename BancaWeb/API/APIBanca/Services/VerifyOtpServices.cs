using APIBanca.Models;


namespace APIBanca.Services
{
    public class VerifyOtpService
    {
        private readonly VerifyOtpRepository _repository;

        public VerifyOtpService(VerifyOtpRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> VerifyOtp(OtpConsumeM otp) {
            return await _repository.VerifyOtp(otp);
        }

    }
}