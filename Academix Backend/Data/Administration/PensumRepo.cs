using Data.Common;
using Data.Entities;
using Models.Administration;
using Models.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Data.Administration
{
    public class PensumRepo : Repository<Pensum, PensumModel>
    {
        public AsignaturasRepo asignaturasRepo = new AsignaturasRepo();
        public TrimestreRepo trimestreRepo = new TrimestreRepo();
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
                                  CantAsignaturas = DB.Set<Asignatura_Pensum>().Count(x => x.idPensum == u.idPensum),
                                  limiteCreditoTrimestral = u.limiteCreditoTrimestral,
                                  esActivo = u.esActivo
                              })
         )
        { }

        public PensumModel GetByName(string nombrePensum)
        {
            return this.Get(x => x.nombrePensum == nombrePensum).FirstOrDefault();
        }

        public List<AsignaturaPensumModel> GetAsignaturasPensum()
        {
            List<Asignatura_Pensum> asignaturasSet = dbContext.Set<Asignatura_Pensum>().ToList();

            var asignaturasPensumModelList = asignaturasSet
                .GroupBy(ap => new { ap.idPensum, ap.idTrimestre })
                .Select(g => new AsignaturaPensumModel
                {
                    idPensum = g.Key.idPensum,
                    idTrimestre = g.Key.idTrimestre,
                    Trimestre = (trimestreRepo.Get(x => x.idTrimestre == g.Key.idTrimestre).FirstOrDefault()).descripcion, // Puedes personalizar cómo se muestra el trimestre
                    Asignaturas = g.Select(ap => asignaturasRepo.Get(x => x.idAsignatura == ap.idAsignatura).FirstOrDefault()).ToList()
                })
                .ToList();

            return asignaturasPensumModelList;
        }

        public OperationResult PostAsignaturaPensum(int idPensum, int idTrimestre, List<int> idAsignaturas)
        {
            var asignaturasSet = dbContext.Set<Asignatura_Pensum>();
            if (idAsignaturas != null && idAsignaturas.Count() > 0)
            {
                var pensum = this.Get(x => x.idPensum == idPensum).FirstOrDefault();

                var asignaturas = asignaturasRepo.Get(x => idAsignaturas.Contains(x.idAsignatura)).ToList();
                int sumaCreditos = 0;


                foreach (var asignatura in asignaturas)
                {
                    sumaCreditos += asignatura.Creditos;
                }

                if (sumaCreditos >= pensum.limiteCreditoTrimestral)
                {
                    return new OperationResult(false, "Las asignaturas han excedido la cantidad de límite de créditos trimestral");
                }

                asignaturasSet.RemoveRange(asignaturasSet.Where(p => p.idPensum == idPensum && p.idTrimestre == idTrimestre));

                asignaturasSet.AddRange(idAsignaturas.Select(p => new Asignatura_Pensum()
                {
                    idPensum = idPensum,
                    idTrimestre = idTrimestre,
                    idAsignatura = p
                }));

                SaveChanges();
                return new OperationResult(true, "Se han guardado las asignaturas satisfactoriamente");
            }
            else
            {
                return new OperationResult(false, "No se han enviado las asignaturas");
            }
        }
    }
}
