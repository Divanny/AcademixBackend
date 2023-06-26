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
    public class AsignaturasRepo : Repository<Asignatura,AsignaturasModel>
    {
        
            public AsignaturasRepo(DbContext dbContext = null) : base
            (
                dbContext ?? new AcadmixEntities(),
                new ObjectsMapper<AsignaturasModel, Asignatura>(u => new Asignatura()
                {
                    idAsignatura = u.idAsignatura,
                    nombreAsignatura = u.NombreAsignatura,
                    codigoAsignatura = u.CodigoAsignatura,
                    idArea = u.idArea,
                    creditos = u.Creditos,
                    idCarrera = u.idCarrera
                   
                }),
                (DB, filter) => (from u in DB.Set<Asignatura>().Where(filter)
                                 join a in DB.Set<Area>() on u.idArea equals a.idArea
                                 select new AsignaturasModel()
                                 {
                                     idAsignatura = u.idAsignatura,
                                     NombreAsignatura = u.nombreAsignatura,
                                     CodigoAsignatura = u.codigoAsignatura,
                                     idArea = u.idArea ?? 0,
                                     Area = a.nombre,
                                     Creditos = u.creditos,
                                     idCarrera = u.idCarrera ?? 0
                                 })
            )
            { }
        public AsignaturasModel GetByUsername(string nombreAsignatura)
        {
            return this.Get(x => x.nombreAsignatura == nombreAsignatura).FirstOrDefault();
        }

        public AsignaturasModel GetByCode(string codigoAsignatura)
        {
            return this.Get(x => x.codigoAsignatura == codigoAsignatura).FirstOrDefault();
        }
    }
}
