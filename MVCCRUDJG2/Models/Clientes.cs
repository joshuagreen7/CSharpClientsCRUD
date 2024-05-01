using System.ComponentModel.DataAnnotations;

namespace MVCCRUDJG2.Models
{
    public class Clientes
    {
        public int Idcliente { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string dni { get; set; }

        [Display(Name = "Seleccione Tipo de Cliente")]
        public int tipoCliente { get; set; }

    }
}
