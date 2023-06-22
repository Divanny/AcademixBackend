using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class UsuariosModel
    {
        public int idUsuario { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un nombre de usuario válido")]
        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 carácteres")]
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un correo electrónico")]
        [MaxLength(100, ErrorMessage = "El correo electrónico no puede exceder los 100 carácteres")]
        public string CorreoElectronico { get; set; }
        [Required(ErrorMessage = "Debe especificar una contraseña válida")]
        public string Password { get; set; }
        public string PasswordEncrypted { get; set; }
        public int idPerfil { get; set; }
        public string Perfil { get; set; }
        public int idEstado { get; set; }
        public string Estado { get; set; }
        public System.DateTime FechaRegistro { get; set; }
        public System.DateTime UltimoIngreso { get; set; }
        [Required(ErrorMessage = "Debe especificar el nombre")]
        public string Nombres { get; set; }
        [Required(ErrorMessage = "Debe especificar el apellido")]
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public EstudiantesModel InfoEstudiante { get ; set; }
        public MaestroModel InfoMaestro { get ; set; }
    }
}
