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
    public class DependenciasRepo:Repository<Dependencia,DependenciasModel>
    {
        public DependenciasRepo(DbContext dbContext = null) : base
      (
          dbContext ?? new AcadmixEntities(),
          new ObjectsMapper<DependenciasModel, Dependencia>(u => new Dependencia()
          {
              idDependencia = u.idDependencia,
              idAsignatura = u.idAsignatura,
              idPrerrequisito = u.idPrerrequisito,
          }),
          (DB, filter) => (from u in DB.Set<Dependencia>().Where(filter)
                           join a in DB.Set<Asignatura>() on u.idAsignatura equals a.idAsignatura
                           select new DependenciasModel()
                           {
                               idDependencia = u.idDependencia,
                               idAsignatura = u.idAsignatura,
                               Asignatura = a.nombreAsignatura,
                               idPrerrequisito = u.idPrerrequisito,
                               Prerrequisito = a.nombreAsignatura

                           })
      )
        { }
    }
}
