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
    public class TrimestreRepo:Repository<Trimestre,TrimestreModel>
    {
        public TrimestreRepo(DbContext dbContext = null) : base
       (
           dbContext ?? new AcadmixEntities(),
           new ObjectsMapper<TrimestreModel, Trimestre>(u => new Trimestre()
           {
               idTrimestre = u.idTrimestre,
               numeroTrimestre = u.numeroTrimestre,
               descripcion = u.descripcion,
           }),
           (DB, filter) => (from u in DB.Set<Trimestre>().Where(filter)
                            select new TrimestreModel()
                            {
                                idTrimestre = u.idTrimestre,
                                numeroTrimestre = u.numeroTrimestre,
                                descripcion = u.descripcion,

                            })
       )
        { }
    }
}
