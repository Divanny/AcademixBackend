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
                              join p in DB.Set<Pensum>() on e.idPensum equals p.idPensum
                              select new EstudiantesModel()
                              {
                                  idEstudiante = e.idEstudiante,
                                  idUsuario = e.idUsuario,
                                  idCarrera = e.idCarrera,
                                  Carrera = c.nombre,
                                  trimestresCursados = e.trimestresCursados,
                                  asignaturasAprobadas = e.asignaturasAprobadas,
                                  idPensum = e.idPensum,
                                  Pensum = p.nombrePensum
                              })
         )
        { }
    }
}
