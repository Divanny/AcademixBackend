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
    public class TipoAulaRepo:Repository<Tipo_Aula,TipoAulaModel>
    {
        public TipoAulaRepo(DbContext dbContext = null) : base
      (
          dbContext ?? new AcadmixEntities(),
          new ObjectsMapper<TipoAulaModel, Tipo_Aula>(u => new Tipo_Aula()
          {
              idTipoAula = u.idTipoAula,
              tipo = u.tipo,
              descripcion = u.descripcion
          }),
          (DB, filter) => (from u in DB.Set<Tipo_Aula>().Where(filter)
                           select new TipoAulaModel()
                           {
                               idTipoAula = u.idTipoAula,
                               tipo = u.tipo,
                               descripcion = u.descripcion

                           })
      )
        { }
    }
}
