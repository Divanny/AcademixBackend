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
                idCarrera = u.idCarrera,
                esActivo = u.esActivo
                   
                }),
                (DB, filter) => (from u in DB.Set<Asignatura>().Where(filter)
                                 join a in DB.Set<Area>() on u.idArea equals a.idArea
                                 join c in DB.Set<Carrera>() on u.idCarrera equals c.idCarrera
                                 select new AsignaturasModel()
                                 {
                                     idAsignatura = u.idAsignatura,
                                     NombreAsignatura = u.nombreAsignatura,
                                     CodigoAsignatura = u.codigoAsignatura,
                                     idArea = a.idArea,
                                     Area = a.nombre,
                                     Creditos = u.creditos,
                                     idCarrera = c.idCarrera,
                                     Carrera = c.nombre,
                                     esActivo = u.esActivo
                                 })
            )
            { }
        public AsignaturasModel GetByName(string nombreAsignatura)
        {
            return this.Get(x => x.nombreAsignatura == nombreAsignatura).FirstOrDefault();
        }

        public AsignaturasModel GetByCode(string codigoAsignatura)
        {
            return this.Get(x => x.codigoAsignatura == codigoAsignatura).FirstOrDefault();
        }
    }
}
