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

        public async Task<bool> ResetPassword(ResetPasswordM resetPassword) {
            return await _repository.ResetPassword(resetPassword);
        }

    }
}