using Data.Common;
using Data.Entities;
using Models.Administration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Data.Administration
{
    public class ListadoEstudiantesRepo : Repository<Listado_Estudiantes, ListadoEstudiantesModel>
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
             (DB, filter) =>
             {
                 UsuariosRepo usuariosRepo = new UsuariosRepo(dbContext);


                 return (

                              from u in DB.Set<Listado_Estudiantes>().Where(filter)
                              join m in DB.Set<Seccion_Asignatura>() on u.idSeccion equals m.idSeccion
                              join a in DB.Set<Asignatura>() on m.idAsignatura equals a.idAsignatura
                              join e in DB.Set<Estudiante>() on u.idEstudiante equals e.idEstudiante
                              join s in DB.Set<Usuarios>() on e.idUsuario equals s.idUsuario
                              join p in DB.Set<Periodo>() on u.idPeriodo equals p.idPeriodo
                              join n in DB.Set<Publicacion>() on u.idListadoEstudiante equals n.idListadoEstudiante into publicacion
                              from n in publicacion.DefaultIfEmpty()
                              let infoUsuario = usuariosRepo.Get(x => x.idUsuario == e.idUsuario).FirstOrDefault()
                              select new ListadoEstudiantesModel()
                              {
                                  idListadoEstudiante = u.idListadoEstudiante,
                                  idSeccion = u.idSeccion,
                                  codigoSeccion = m.codigoSeccion,
                                  nombreAsignatura = a.nombreAsignatura,
                                  idEstudiante = u.idEstudiante,
                                  infoUsuario = infoUsuario,
                                  idPeriodo = u.idPeriodo,
                                  nombrePeriodo = p.nombre,
                                  anioPeriodo = u.anioPeriodo,
                                  calificacion = n != null ? n.idCalificacion : 0
                              });
             }
         )
        { }

        public List<Listado_Estudiantes> getListadoByEstudiante()
        {
            AcadmixEntities academixEntities = new AcadmixEntities();
            Utilities utilities = new Utilities();

            //int idUsuario = OnlineUser.GetUserId();
            int Periodo = utilities.ObtenerTrimestreActual();


            var idEstudiante = academixEntities.Estudiante
                           .Where(z => z.idUsuario == 9)
                           .Select(x => x.idEstudiante)
                           .FirstOrDefault();

            List<Listado_Estudiantes> ids = academixEntities.Listado_Estudiantes
                                                .Where(z => z.idEstudiante == idEstudiante && z.idPeriodo == Periodo && z.anioPeriodo == DateTime.Now.Year).ToList();

            return ids;
        }

        public List<Listado_Estudiantes> getListadoByEstudianteGeneral()
        {
            AcadmixEntities academixEntities = new AcadmixEntities();
            Utilities utilities = new Utilities();

            int idUsuario = OnlineUser.GetUserId();



            var idEstudiante = academixEntities.Estudiante
                           .Where(z => z.idUsuario == idUsuario)
                           .Select(x => x.idEstudiante)
                           .FirstOrDefault();

            List<Listado_Estudiantes> AsignaturasCursadas = academixEntities.Listado_Estudiantes
                                                .Where(z => z.idEstudiante == idEstudiante).ToList();

            return AsignaturasCursadas;
        }

        public ListadoEstudiantesModel GetByIdSeccion(int idSeccion, int idEstudiante, int anioPeriodo, int idPeriodo)
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

        public int CupoActual(int idSeccion, int anioPeriodo, int idPeriodo)
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


        public int verificarSeccionRepetida(int idEstudiante, int anioPeriodo, int idPeriodo, int idSeccion)
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

        public int verificarAprobacionAsignatura(int idEstudiante, int idSeccion)
        {
            AcadmixEntities academixEntities = new AcadmixEntities();

            int AsignaturaComparable = ObtenerIdAsignaturaPorIdSeccion(idSeccion);

            var seccionesActuales = academixEntities.Listado_Estudiantes
                                        .Where(x => x.idEstudiante == idEstudiante)
                                        .ToList();

            foreach (var seccionEstudiante in seccionesActuales)
            {
                var asignaturaSeccion = academixEntities.Seccion_Asignatura
                                            .FirstOrDefault(x => x.idSeccion == seccionEstudiante.idSeccion && x.idAsignatura == AsignaturaComparable);

                if (asignaturaSeccion != null)
                {
                    var listadoCursado = academixEntities.Listado_Estudiantes
                                            .Where(x => x.idSeccion == asignaturaSeccion.idSeccion).FirstOrDefault();

                    var PublicacionCursada = academixEntities.Publicacion
                                                .Where(x => x.idListadoEstudiante == listadoCursado.idListadoEstudiante).FirstOrDefault();


                    if (PublicacionCursada != null)
                    {
                        if (PublicacionCursada.idCalificacion >= 70)
                        {
                            return PublicacionCursada.idCalificacion;
                        }
                        else
                        {
                            return 0;
                        }

                    }
                    else
                    {
                        return 0;
                    }

                    // Devuelve el ID del registro en la tabla academixEntities.Listado_Estudiantes

                }
            }

            return 0;

        }

        public bool validarChoqueEstudiante(int idEstudiante, int anioPeriodo, int idPeriodo, int diaId, TimeSpan horaDesde, TimeSpan horaHasta)
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

        public double GetMiIndiceGeneral()
        {
            AcadmixEntities acadmixEntities = new AcadmixEntities();
            Utilities utilities = new Utilities();

            List<Listado_Estudiantes> ids = this.getListadoByEstudianteGeneral();
            double indiceTrimestal;
            int sumaCreditos = 0;
            double sumaPuntos = 0;
            int creditos;


            foreach (var item in ids)
            {
                int calificacion = acadmixEntities.Publicacion
                                .Where(x => x.idListadoEstudiante == item.idListadoEstudiante)
                                .Select(x => x.idCalificacion).FirstOrDefault();



                if (calificacion != null)
                {
                    string literal = utilities.getCalificacionLiteral(calificacion);

                    double valorLiteral = utilities.ValorDelLiteral(literal);

                    int idAsignatura = acadmixEntities.Seccion_Asignatura
                                        .Where(x => x.idSeccion == item.idSeccion)
                                        .Select(x => x.idAsignatura).FirstOrDefault();

                    creditos = acadmixEntities.Asignatura
                                       .Where(x => x.idAsignatura == idAsignatura)
                                       .Select(x => x.creditos).FirstOrDefault();

                    double puntos = creditos * valorLiteral;

                    sumaCreditos += creditos;
                    sumaPuntos += puntos;



                }





            }
            indiceTrimestal = sumaPuntos / sumaCreditos;
            return indiceTrimestal;
        }

        public int verificarAprobacionPrerrequisitos(int idEstudiante, int idSeccion)
        {
            AcadmixEntities academixEntities = new AcadmixEntities();
            SeccionAsignaturaRepo seccionAsignaturaRepo = new SeccionAsignaturaRepo();

            int AsignaturaComparable = ObtenerIdAsignaturaPorIdSeccion(idSeccion);

            var prerrequisito = academixEntities.Dependencia
                                    .Where(x => x.idAsignatura == AsignaturaComparable)
                                    .Select(x => x.idPrerrequisito).ToList();

            var conteoPrerrequisito = academixEntities.Dependencia
                                    .Where(x => x.idAsignatura == AsignaturaComparable)
                                    .Count();

            int conteoSeccionesActuales = 0;
            var seccionesActuales = new List<int>();

            if (conteoPrerrequisito > 0)
            {
                foreach (var item in prerrequisito)
                {
                    var seccionesPrerrequisito = academixEntities.Seccion_Asignatura
                                                .Where(x => x.idAsignatura == item).ToList();

                    foreach (var x in seccionesPrerrequisito)
                    {
                        seccionesActuales = academixEntities.Listado_Estudiantes
                                          .Where(y => y.idEstudiante == idEstudiante && y.idSeccion == x.idSeccion)
                                          .Select(z => z.idListadoEstudiante)
                                          .ToList();

                        conteoSeccionesActuales = seccionesActuales.Count();

                    }


                }
                if (conteoSeccionesActuales < conteoPrerrequisito)
                {
                    return 1;
                }
                else
                {
                    bool tieneCalificacionMenor70 = false;

                    foreach (var item in seccionesActuales)
                    {
                        var PublicacionCursada = academixEntities.Publicacion
                                                    .Where(x => x.idListadoEstudiante == item)
                                                    .FirstOrDefault();

                        if (PublicacionCursada.idCalificacion < 70)
                        {
                            tieneCalificacionMenor70 = true;
                            break; // Salir del bucle si se encuentra una calificación menor a 70
                        }
                    }

                    if (tieneCalificacionMenor70)
                    {
                        return 1;
                    }


                }

            }
            else
            {
                return 0;
            }

            return 0;

        }

    }
}
