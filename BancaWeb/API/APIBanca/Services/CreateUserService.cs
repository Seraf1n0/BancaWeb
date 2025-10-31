using APIBanca.Models;


namespace APIBanca.Services
{  
    public class CreateUserService
    {
        private readonly UserRepository _repository;
        public CreateUserService(UserRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> CrearUsuarioAsync(User usuario)
        {

           return await _repository.CrearUsuarioAsync(usuario);
        }


    }
}
