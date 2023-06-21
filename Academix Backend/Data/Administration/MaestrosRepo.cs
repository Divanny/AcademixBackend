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
    public class MaestrosRepo : Repository<Maestro,MaestroModel>
    {
        public MaestrosRepo(DbContext dbContext = null) : base
         (
             dbContext ?? new AcadmixEntities(),
             new ObjectsMapper<MaestroModel, Maestro>(u => new Maestro()
             {
                 idMaestro = u.idMaestro,
                 idUsuario = u.idUsuario,
                 credencial = u.credencial,
                 idArea = u.idArea
                

             }),
             (DB, filter) => (from m in DB.Set<Maestro>().Where(filter)
                              join a in DB.Set<Area>() on m.idArea equals a.idArea
                              select new MaestroModel()
                              {
                                  idMaestro = m.idMaestro,
                                  idUsuario = m.idUsuario,
                                  credencial = m.credencial,
                                  idArea = m.idArea,
                                  Area = a.nombre
                              })
         )
        { }
    }
}
