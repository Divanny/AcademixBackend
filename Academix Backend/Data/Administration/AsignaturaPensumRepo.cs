//using Data.Common;
//using Data.Entities;
//using Models.Administration;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Data.Administration
//{
//    public class AsignaturaPensumRepo:Repository<Asignatura_Pensum,AsignaturaPensumModel>
//    {
//        public AsignaturaPensumRepo(DbContext dbContext = null) : base
//      (
//          dbContext ?? new AcadmixEntities(),
//          new ObjectsMapper<AsignaturaPensumModel, Asignatura_Pensum>(u => new Asignatura_Pensum()
//          {
//              idAsignaturaPensum = u.idAsignaturaPensum,
//              idAsignatura = u.idAsignatura,
//              idPensum = u.idPensum,
//              idTrimestre = u.idTrimestre,
//          }),
//          (DB, filter) => (from u in DB.Set<Asignatura_Pensum>().Where(filter)
//                           join a in DB.Set<Asignatura>() on u.idAsignatura equals a.idAsignatura
//                           join p in DB.Set<Pensum>() on u.idPensum equals p.idPensum
//                           select new AsignaturaPensumModel()
//                           {
//                               idAsignaturaPensum = u.idAsignaturaPensum,
//                               idAsignatura = u.idAsignatura,
//                               Asignatura = a.nombreAsignatura,
//                               idPensum = u.idPensum,
//                               Pensum = p.nombrePensum,
//                               idTrimestre = u.idTrimestre,

//                           })
//      )
//        { }
//    }
//}
