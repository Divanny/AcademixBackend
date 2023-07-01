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
    public class AreasRepo : Repository<Area,AreasModel>
    {
        public AreasRepo(DbContext dbContext = null) : base
          (
              dbContext ?? new AcadmixEntities(),
              new ObjectsMapper<AreasModel, Area>(u => new Area()
              {
                  idArea = u.idArea,
                  nombre = u.nombre,
                  descripcion = u.descripcion,
                  idEncargado = u.idEncargado,
                  esActivo = u.esActivo

              }),
              (DB, filter) => (from a in DB.Set<Area>().Where(filter)
                               join m in DB.Set<Maestro>() on a.idArea equals m.idArea
                               join u in DB.Set<Usuarios>() on m.idUsuario equals u.idUsuario
                               select new AreasModel()
                               {
                                   idArea = a.idArea,
                                   nombre = a.nombre,
                                   descripcion = a.descripcion,
                                   idEncargado = a.idEncargado,
                                   Encargado = u.Nombres,
                                   CantCarreras = DB.Set<Carrera>().Count(c => c.idArea == a.idArea),
                                   esActivo = a.esActivo
                               })
          )
        { }
        public AreasModel GetByUsername(string nombreArea)
        {
            return this.Get(x => x.nombre == nombreArea).FirstOrDefault();
        }

    }
}
