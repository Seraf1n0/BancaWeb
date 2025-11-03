using APIBanca.Models;


namespace APIBanca.Services
{
    public class ResetPasswordService
    {
        private readonly ResetPasswordRepository _repository;

        public ResetPasswordService(ResetPasswordRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> ResetPassword(OtpConsumeM otp) {
            return await _repository.ResetPassword(otp);
        }

    }
}