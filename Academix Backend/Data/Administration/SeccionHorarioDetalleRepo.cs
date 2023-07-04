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
    public class SeccionHorarioDetalleRepo : Repository<SeccionHorarioDetalle,SeccionHorarioDetalleModel>
    {
        public SeccionHorarioDetalleRepo(DbContext dbContext = null) : base
        (
            dbContext ?? new AcadmixEntities(),
            new ObjectsMapper<SeccionHorarioDetalleModel, SeccionHorarioDetalle>(u => new SeccionHorarioDetalle()
            {
                idSecciom = u.idSecciom,
                idAula = u.idAula,
                idDia = u.idDia,
                horaDesde = u.horaDesde,
                horaHasta = u.horaHasta,
            }),
            (DB, filter) => (from u in DB.Set<SeccionHorarioDetalle>().Where(filter)
                             join a in DB.Set<Aula>() on u.idAula equals a.idAula
                             join d in DB.Set<Dia_Semana>() on u.idDia equals d.idDia
                             select new SeccionHorarioDetalleModel()
                             {
                                 idSecciom = u.idSecciom,
                                 idAula = u.idAula ?? 0,
                                 Aula = a.nombre,
                                 idDia = u.idDia,
                                 Dia = d.dia,
                                 horaDesde = u.horaDesde,
                                 horaHasta = u.horaHasta,
                                 
                             })
        )
        { }
    }
}
