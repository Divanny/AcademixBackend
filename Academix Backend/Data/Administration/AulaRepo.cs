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
    public class AulaRepo:Repository<Aula,AulaModel>
    {
        public AulaRepo(DbContext dbContext = null) : base
         (
             dbContext ?? new AcadmixEntities(),
             new ObjectsMapper<AulaModel, Aula>(u => new Aula()
             {
                 idAula = u.idAula,
                 nombre = u.nombre,
                 descripcion = u.descripcion,
                 idTipoAula = u.idTipoAula,


             }),
             (DB, filter) => (from u in DB.Set<Aula>().Where(filter)
                              join c in DB.Set<Tipo_Aula>() on u.idTipoAula equals c.idTipoAula
                              select new AulaModel()
                              {
                                  idAula = u.idAula,
                                  nombre = u.nombre,
                                  descripcion = u.descripcion,
                                  idTipoAula = u.idTipoAula,
                                  TipoAula = c.tipo
                              })
         )
        { }
    }
}
