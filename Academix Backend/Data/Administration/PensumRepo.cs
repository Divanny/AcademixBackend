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
    public class PensumRepo:Repository<Pensum,PensumModel>
    {
        public PensumRepo(DbContext dbContext = null) : base
         (
             dbContext ?? new AcadmixEntities(),
             new ObjectsMapper<PensumModel, Pensum>(u => new Pensum()
             {
                 idPensum = u.idPensum,
                 nombrePensum = u.nombrePensum,
                 anioPensum = u.anioPensum,
                 descripcion = u.descripcion,
                 idCarrera = u.idCarrera,
                 limiteCreditoTrimestral = u.limiteCreditoTrimestral,
                 esActivo = u.esActivo


             }),
             (DB, filter) => (from u in DB.Set<Pensum>().Where(filter)
                              join c in DB.Set<Carrera>() on u.idCarrera equals c.idCarrera
                              select new PensumModel()
                              {
                                  idPensum = u.idPensum,
                                  nombrePensum = u.nombrePensum,
                                  anioPensum = u.anioPensum,
                                  descripcion = u.descripcion,
                                  idCarrera = u.idCarrera,
                                  Carrera = c.nombre,
                                  limiteCreditoTrimestral = u.limiteCreditoTrimestral,
                                  esActivo = u.esActivo
                              })
         )
        { }

        public PensumModel GetByName(string nombrePensum)
        {
            return this.Get(x => x.nombrePensum == nombrePensum).FirstOrDefault();
        }

    }
}
