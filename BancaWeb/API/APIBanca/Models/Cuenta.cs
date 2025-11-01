using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace APIBanca.Models
{
    public class Cuenta
    {
        public Guid id { get; set; }
        public Guid usuario_id { get; set; }
        public string iban { get; set; } = "";
        public string alias { get; set; } = "";
        public Guid tipoCuenta { get; set; }
        public Guid moneda { get; set; }
        public decimal saldo { get; set; }
        public Guid estado { get; set; }
        public DateTime? fecha_creacion { get; set; }
        public DateTime? fecha_actualizacion { get; set; }
    }

    // Modelo para crear una cuenta en request
    public class CreateCuenta
    {
        [Required] public Guid usuario_id { get; set; }
        [Required, StringLength(34, MinimumLength = 8)] public string iban { get; set; } = "";
        [Required, StringLength(100)] public string alias { get; set; } = "";
        [Required] public int tipo { get; set; }
        [Required] public int moneda { get; set; }
        [Required, Range(0, double.MaxValue)] public decimal saldo_inicial { get; set; }
        [Required] public int estado { get; set; }
    }

    // Modelo para actualizar el estado de una cuenta
    public class UpdateEstadoCuenta
    {
        [Required] public int nuevo_estado { get; set; }
    }

    // Modelo para paginación
    public class PagedResult<T>
    {
        public IReadOnlyList<T> items { get; init; } = Array.Empty<T>();
        public int total { get; init; }
        public int page { get; init; }
        public int page_size { get; init; }
    }

    public class MovimientoPage : PagedResult<JsonElement> { }
}