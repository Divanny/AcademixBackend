using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class PerfilesModel
    {
        public int idPerfil { get; set; }
        [Required(ErrorMessage = "Debe especificar un nombre del perfil válido")]
        [MaxLength(50, ErrorMessage = "El nombre del perfil no puede exceder los 50 carácteres")]
        public string Nombre { get; set; }
        [MaxLength(250, ErrorMessage = "La descripción del perfil no puede exceder los 250 carácteres")]
        public string Descripcion { get; set; }
        public int CantPermisos { get; set; }
        public bool PorDefecto { get; set; }
        public IEnumerable<VistasModel> Vistas { get; set; }
        public IEnumerable<UsuariosModel> Usuarios { get; set; }
    }
}
