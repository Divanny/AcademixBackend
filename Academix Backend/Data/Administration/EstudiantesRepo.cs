using Data.Common;
using Data.Entities;
using Models.Administration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Data.Administration
{
    public class EstudiantesRepo:Repository<Estudiante,EstudiantesModel>
    {
        public EstudiantesRepo(DbContext dbContext = null) : base
         (
             dbContext ?? new AcadmixEntities(),
             new ObjectsMapper<EstudiantesModel, Estudiante>(u => new Estudiante()
             {
                 idEstudiante = u.idEstudiante,
                 idUsuario = u.idUsuario,
                 idCarrera = u.idCarrera,
                 trimestresCursados = u.trimestresCursados,
                 asignaturasAprobadas = u.asignaturasAprobadas,
                 idPensum = u.idPensum
             }),
             (DB, filter) => (from e in DB.Set<Estudiante>().Where(filter)
                              join c in DB.Set<Carrera>() on e.idCarrera equals c.idCarrera
                              join a in DB.Set<Area>() on c.idArea equals a.idArea
                              join p in DB.Set<Pensum>() on e.idPensum equals p.idPensum
                              select new EstudiantesModel()
                              {
                                  idEstudiante = e.idEstudiante,
                                  idUsuario = e.idUsuario,
                                  idCarrera = e.idCarrera,
                                  Carrera = c.nombre,
                                  idArea = a.idArea,
                                  Area = a.nombre,
                                  trimestresCursados = e.trimestresCursados,
                                  asignaturasAprobadas = e.asignaturasAprobadas,
                                  idPensum = e.idPensum,
                                  Pensum = p.nombrePensum,
                                  IndiceGeneral = GetIndiceGeneral(e.idUsuario)
                              })
         )
        { }

        private static double GetIndiceGeneral(int idUsuario)
        {
            using (AcadmixEntities dbc = new AcadmixEntities())
            {
                ListadoEstudiantesRepo listadoEstudiantesRepo = new ListadoEstudiantesRepo(dbc);
                Utilities utilities = new Utilities();

                var idEstudiante = dbc.Estudiante
                               .Where(z => z.idUsuario == idUsuario)
                               .Select(x => x.idEstudiante)
                               .FirstOrDefault();

                List<Listado_Estudiantes> AsignaturasCursadas = dbc.Listado_Estudiantes
                                                    .Where(z => z.idEstudiante == idEstudiante).ToList();

                double indiceTrimestal, sumaPuntos = 0;
                int sumaCreditos = 0, creditos;

                foreach (var item in AsignaturasCursadas)
                {
                    int calificacion = dbc.Publicacion
                                    .Where(x => x.idListadoEstudiante == item.idListadoEstudiante)
                                    .Select(x => x.idCalificacion).FirstOrDefault();

                    if (calificacion != 0)
                    {
                        string literal = utilities.getCalificacionLiteral(calificacion);

                        double valorLiteral = utilities.ValorDelLiteral(literal);

                        int idAsignatura = dbc.Seccion_Asignatura
                                            .Where(x => x.idSeccion == item.idSeccion)
                                            .Select(x => x.idAsignatura).FirstOrDefault();

                        creditos = dbc.Asignatura
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
        }
    }
}
