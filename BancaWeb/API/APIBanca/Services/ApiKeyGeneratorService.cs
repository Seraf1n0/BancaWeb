using System.Text;

namespace APIBanca.Services
{
    public class ApiKeyGeneratorService
    {
        public string GenerarApiKey(int longitud = 20) // Largo de la contraseña, lo podemos cambiar también
        {

            // Aqui podemos agregar más 
            const string letras = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            const string numeros = "0123456789";
            const string caracteresEspeciales = "!@#$%^&-*/()=?¡¿.,;:,<>";

            var password = new StringBuilder();
            var random = Random.Shared;

            for (int i = 0; i < longitud; i++)
            {
                var tipo = random.Next(1, 4); // Si cae  en 1: letra, 2: número, 3: carácter especial
                
                switch (tipo)
                {
                    case 1:
                        password.Append(letras[random.Next(letras.Length)]);
                        break;
                    case 2:
                        password.Append(numeros[random.Next(numeros.Length)]);
                        break;
                    case 3:
                        password.Append(caracteresEspeciales[random.Next(caracteresEspeciales.Length)]);
                        break;
                }
            }

            return password.ToString();
        }
    }
}