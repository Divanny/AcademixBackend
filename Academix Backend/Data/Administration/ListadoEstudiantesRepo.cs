using Data.Common;
using Data.Entities;
using Models.Administration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Administration
{
    public class ListadoEstudiantesRepo:Repository<Listado_Estudiantes,ListadoEstudiantesModel>
    {
        public ListadoEstudiantesRepo(DbContext dbContext = null) : base
         (
             dbContext ?? new AcadmixEntities(),
             new ObjectsMapper<ListadoEstudiantesModel, Listado_Estudiantes>(u => new Listado_Estudiantes()
             {
                 idListadoEstudiante = u.idListadoEstudiante,
                 idSeccion = u.idSeccion,
                 idEstudiante = u.idEstudiante,
                 idPeriodo = u.idPeriodo,
                 anioPeriodo = u.anioPeriodo

             }),
             (DB, filter) => (from u in DB.Set<Listado_Estudiantes>().Where(filter)
                              join m in DB.Set<Seccion_Asignatura>() on u.idSeccion equals m.idSeccion
                              join a in DB.Set<Asignatura>() on m.idAsignatura equals a.idAsignatura
                              join e in DB.Set<Estudiante>() on u.idEstudiante equals e.idEstudiante
                              join s in DB.Set<Usuarios>() on e.idUsuario equals s.idUsuario    
                              join p in DB.Set<Periodo>() on u.idPeriodo equals p.idPeriodo
                              select new ListadoEstudiantesModel()
                              {
                                  idListadoEstudiante = u.idListadoEstudiante,
                                  idSeccion = u.idSeccion,
                                  codigoSeccion = m.codigoSeccion,  
                                  nombreAsignatura = a.nombreAsignatura,
                                  idEstudiante = u.idEstudiante,
                                  nombreCompleto = s.Nombres + " " + s.Apellidos,
                                  idPeriodo = u.idPeriodo,
                                  nombrePeriodo = p.nombre,
                                  anioPeriodo = u.anioPeriodo
                              })
         )
        { }

        public ListadoEstudiantesModel GetByIdSeccion(int idSeccion,int idEstudiante, string anioPeriodo, int idPeriodo)
        {
            return this.Get(x => x.idSeccion == idSeccion && x.idEstudiante == idEstudiante && x.anioPeriodo == anioPeriodo && x.idPeriodo == idPeriodo).FirstOrDefault();
        }

        public int CapacidadMaxima(int idSeccion)
        {
            AcadmixEntities academixEntities = new AcadmixEntities();


            int capacidadMaxima = academixEntities.Seccion_Asignatura
                           .Where(seccion => seccion.idSeccion == idSeccion)
                           .Select(x => x.capacidadMax)
                           .FirstOrDefault();




            return capacidadMaxima;
        }

        public int CupoActual(int idSeccion, string anioPeriodo, int idPeriodo)
        {
            AcadmixEntities academixEntities = new AcadmixEntities();

            int cupoActual = academixEntities.Listado_Estudiantes
                                .Where(listado => listado.idPeriodo == idPeriodo && listado.anioPeriodo == anioPeriodo && listado.idSeccion == idSeccion)
                                .Count();

            return cupoActual;

        }
        private int ObtenerIdAsignaturaPorIdSeccion(int idSeccion)
        {
            AcadmixEntities academixEntities = new AcadmixEntities();

            var seccionAsignatura = academixEntities.Seccion_Asignatura
                                        .FirstOrDefault(seccion => seccion.idSeccion == idSeccion);


            return seccionAsignatura.idAsignatura;

        }
        public List<SeccionHorarioDetalle> ObtenerHorarioPorIdSeccion(int idSeccion)
        {
            AcadmixEntities academixEntities = new AcadmixEntities();

            var horarios = academixEntities.SeccionHorarioDetalle
                .Where(x => x.idSecciom == idSeccion)
                .ToList();

            return horarios;
        }


        public int verificarSeccionRepetida(int idEstudiante, string anioPeriodo, int idPeriodo, int idSeccion)
        {
            AcadmixEntities academixEntities = new AcadmixEntities();

            int AsignaturaComparable = ObtenerIdAsignaturaPorIdSeccion(idSeccion);

            var seccionesActuales = academixEntities.Listado_Estudiantes
                                        .Where(x => x.idEstudiante == idEstudiante && x.anioPeriodo == anioPeriodo && x.idPeriodo == idPeriodo)
                                        .ToList();

            foreach (var seccionEstudiante in seccionesActuales)
            {
                var asignaturaSeccion = academixEntities.Seccion_Asignatura
                                            .FirstOrDefault(x => x.idSeccion == seccionEstudiante.idSeccion && x.idAsignatura == AsignaturaComparable);

                if (asignaturaSeccion != null)
                {


                    // Devuelve el ID del registro en la tabla academixEntities.Listado_Estudiantes
                    return seccionEstudiante.idListadoEstudiante;
                }
            }

            return 0;

        }
        public bool validarChoqueEstudiante(int idEstudiante, string anioPeriodo, int idPeriodo, int diaId, TimeSpan horaDesde, TimeSpan horaHasta)
        {

            AcadmixEntities academixEntities = new AcadmixEntities();
            

            var seccionesActuales = academixEntities.Listado_Estudiantes
                                        .Where(x => x.idEstudiante == idEstudiante && x.anioPeriodo == anioPeriodo && x.idPeriodo == idPeriodo)
                                        .ToList();


            bool Disponible = true;
            //List<SeccionHorarioDetalle> detalle = ObtenerHorarioPorIdSeccion(idSeccion);
            foreach (var seccionEstudiante in seccionesActuales)
            {
                var horariosActuales = academixEntities.SeccionHorarioDetalle
                    .Where(x => x.idSecciom == seccionEstudiante.idSeccion && x.idDia == diaId)
                    .ToList();

                foreach (var item in horariosActuales)
                {
                    Disponible = !horariosActuales
                       .Where(x =>
                       horaDesde == x.horaDesde
                       || horaHasta == x.horaHasta
                       || (horaDesde >= x.horaDesde && horaDesde < x.horaHasta)
                       || (horaHasta > x.horaDesde && horaHasta <= x.horaHasta)
                       || (horaHasta > x.horaHasta && horaDesde < x.horaDesde)

                      ).Any();
                    if (Disponible == false)
                    {
                        return Disponible;
                    }

                }
            }

            

           

            return Disponible;




            
        }
    }
}
