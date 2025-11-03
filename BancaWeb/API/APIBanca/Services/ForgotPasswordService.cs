using APIBanca.Models;

namespace APIBanca.Services
{
    public class ForgotPasswordService
    {
        private readonly ForgotPasswordRepository _forgotPasswordRepository;

        public ForgotPasswordService(ForgotPasswordRepository forgotPasswordRepository)
        {
            _forgotPasswordRepository = forgotPasswordRepository;
        }

        public async Task<Guid> ForgotPassword(ForgotPasswordM forgotPassword)
        {
            return await _forgotPasswordRepository.ForgotPassword(forgotPassword);
        }
    }
}