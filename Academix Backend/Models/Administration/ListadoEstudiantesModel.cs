﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class ListadoEstudiantesModel
    {
        public int idListadoEstudiante { get; set; }
        public int idSeccion { get; set; }
        public int codigoSeccion { get; set; }
        public string nombreAsignatura { get; set; }
        public int idEstudiante { get; set; }
        public UsuariosModel infoUsuario { get; set; }
        public int idPeriodo { get; set; }
        public string nombrePeriodo { get; set; }
        public int anioPeriodo { get; set; }
        public int calificacion { get; set; }
        public DateTime? fechaPublicacion { get; set; }
        public int idAsignatura { get; set; }
    }
}
