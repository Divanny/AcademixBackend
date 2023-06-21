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
    public class CarrerasRepo : Repository<Carrera,CarrerasModel>
    {
        public CarrerasRepo(DbContext dbContext = null) : base
           (
               dbContext ?? new AcadmixEntities(),
               new ObjectsMapper<CarrerasModel, Carrera>(u => new Carrera()
               {
                   idCarrera = u.idCarrera,
                   nombre = u.nombre,
                   descripcion = u.descripcion,
                   duracionTrimestres = u.duracionTrimestres,
                   creditos = u.creditos,
                   idArea = u.idArea,
                   esActivo = u.esActivo,

               }),
               (DB, filter) => (from u in DB.Set<Carrera>().Where(filter)
                                join e in DB.Set<Estudiante>() on u.idCarrera equals e.idCarrera
                                join a in DB.Set<Area>() on u.idArea equals a.idArea
                                select new CarrerasModel()
                                {
                                    idCarrera = u.idCarrera,
                                    nombre = u.nombre,
                                    descripcion = u.descripcion,
                                    duracionTrimestres = u.duracionTrimestres,
                                    creditos = u.creditos,
                                    idArea = u.idArea,
                                    Area = a.nombre,
                                    CantEstudiantes = DB.Set<Estudiante>().Count(x => x.idCarrera == u.idCarrera),
                                    esActivo = u.esActivo
                                })
           )
        { }
        public CarrerasModel GetByUsername(string nombreCarrera)
        {
            return this.Get(x => x.nombre == nombreCarrera).FirstOrDefault();
        }

        
    }
}
