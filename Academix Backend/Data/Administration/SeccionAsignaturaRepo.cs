using Data.Common;
using Data.Entities;
using Models.Administration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Data.Administration
{
    public class SeccionAsignaturaRepo : Repository<Seccion_Asignatura,SeccionAsignaturaModel>
    {
        public SeccionAsignaturaRepo(DbContext dbContext = null) : base
       (
           dbContext ?? new AcadmixEntities(),
           new ObjectsMapper<SeccionAsignaturaModel, Seccion_Asignatura>(u => new Seccion_Asignatura()
           {
               idSeccion = u.idSeccion,
               codigoSeccion = u.codigoSeccion,
               descripcion = u.descripcion,
               idModalidad = u.idModalidad,
               capacidadMax = u.capacidadMax,   
               idAsignatura = u.idAsignatura,
               idMaestro = u.idMaestro,
               esActivo = u.esActivo,
           }),
           (DB, filter) => (from u in DB.Set<Seccion_Asignatura>().Where(filter)
                            join m in DB.Set<Modalidad>() on u.idModalidad equals m.idModalidad
                            join a in DB.Set<Asignatura>() on u.idAsignatura equals a.idAsignatura
                            join p in DB.Set<Maestro>() on u.idMaestro equals p.idMaestro
                            join s in DB.Set<Usuarios>() on p.idUsuario equals s.idUsuario 
                            select new SeccionAsignaturaModel()
                            {
                                idSeccion = u.idSeccion,
                                codigoSeccion = u.codigoSeccion,
                                descripcion = u.descripcion,
                                idModalidad = u.idModalidad,
                                Modalidad = m.nombre,
                                capacidadMax = u.capacidadMax,
                                idAsignatura = u.idAsignatura,
                                Asignatura = a.nombreAsignatura,
                                idMaestro = u.idMaestro,
                                Maestro = s.Nombres + " " + s.Apellidos,
                                esActivo = u.esActivo,

                            })
       )
        { }
        public IEnumerable<SeccionHorarioDetalleModel> GetHorariosDetalles(int idSeccion)
        {
            return from u in dbContext.Set<SeccionHorarioDetalle>().Where(u => u.idSecciom == idSeccion)
                   join a in dbContext.Set<Aula>() on u.idAula equals a.idAula
                   join d in dbContext.Set<Dia_Semana>() on u.idDia equals d.idDia
                   select new SeccionHorarioDetalleModel()
                   {
                      idSecciom = u.idSecciom,
                      idSeccionHorario = u.idSeccionHorario,
                      idAula = u.idAula,
                      Aula = a.nombre,
                      idDia = u.idDia,
                      Dia = d.dia,
                      horaDesde = u.horaDesde,
                      horaHasta = u.horaHasta,
                   };
        }
        

        //public async Task<bool> ValidarChoquesdeHora(int IdProfesor, int diaId, TimeSpan horaDesde, TimeSpan horaHasta,int idSeccion)
        //{


        //    var HorariosProfesor = await _context.Profesor_Disponibilidad_Horarios.AsNoTracking()
        //        .Where(x => x.ProfesorId == IdProfesor && x.DiaId == diaId
        //        && x.idSeccion != idSeccion)

        //        .ToListAsync();

        //    bool Disponible = !HorariosProfesor
        //    .Where(x =>
        //            horaDesde == x.horaDesde
        //            || horaHasta == x.horaHasta
        //            || (horaDesde  >= x.horaDesde && horaDesde < x.horaHasta)
        //            || (horaHasta > x.horaDesde && horaHasta <= x.horaHasta)
        //            || (horaHasta >x.horaHasta && horaDesde < x.horaDesde)

        //           ).Any();

        //    return Disponible;




        //}

        //public async Task<int> CrearDisponibilidadHorario(ProfesorDisponibilidadHorarioViewModel entidad)
        //{
        //    int id = 0;
        //    if (entidad is null) return 0;


        //    if (TimeSpan.Parse(ConvertTime.convertTo24hFormat(entidad.PrHoHasta)) <= TimeSpan.Parse(ConvertTime.convertTo24hFormat(entidad.PrHoDesde)))
        //        return 0;

        //    if (entidad.ProfesorDisponibilidadHorarioId == 0)
        //    {

                

        //        var estaDisponible = await ValidarChoquesdeHora(entidad.ProfesorId, entidad.SedeId, entidad.DiaId, entidad.PrHoDesde, entidad.PrHoHasta, entidad.ProfesorDisponibilidadHorarioId).ConfigureAwait(false);


        //        if (estaDisponible == true)
        //        {
        //            var profDisponibilidadHorario = new Profesor_Disponibilidad_Horario
        //            {
        //                CompanyId = entidad.CompanyId,
        //                SedeId = entidad.SedeId,
        //                ProfesorId = entidad.ProfesorId,
        //                DiaId = entidad.DiaId,
        //                PrHoDesde = entidad.PrHoDesde,
        //                PrHoHasta = entidad.PrHoHasta,
        //                PrHoFechaCreacion = DateTime.Now,

        //            };

        //            await _context.Profesor_Disponibilidad_Horarios.AddAsync(profDisponibilidadHorario).ConfigureAwait(false);
        //            await _context.SaveChangesAsync().ConfigureAwait(false);
        //            id = (int)profDisponibilidadHorario.ProfesorDisponibilidadHorarioId;


        //        }


        //    }
        //    return id;
        //}

        public List<SeccionHorarioDetalleModel> GetHorarioByAsignatura(int idAsignatura)
        {
            var secciones = this.Get(x => x.idAsignatura == idAsignatura).ToList();
            List<SeccionHorarioDetalleModel> detalles = new List<SeccionHorarioDetalleModel>();

            foreach (var seccion in secciones)
            {
                var seccionDetalles = GetHorariosDetalles(seccion.idSeccion);
                detalles.AddRange(seccionDetalles);
            }


            return detalles;
        }
        public List<SeccionHorarioDetalleModel> GetHorarioByMaesto(int idMaestro)
        {
            var secciones = this.Get(x => x.idMaestro == idMaestro).ToList();
            List<SeccionHorarioDetalleModel> detalles = new List<SeccionHorarioDetalleModel>();

            foreach (var seccion in secciones)
            {
                var seccionDetalles = GetHorariosDetalles(seccion.idSeccion);
                detalles.AddRange(seccionDetalles);
            }


            return detalles;
        }





        public override Seccion_Asignatura Add(SeccionAsignaturaModel model)
        {
            using (var trx = dbContext.Database.BeginTransaction())
            {
                try
                {
                   
                    
                    Seccion_Asignatura created = base.Add(model);

                    var detalleSet = dbContext.Set<SeccionHorarioDetalle>();
                    if (model.detalleSeccion != null && model.detalleSeccion.Count() > 0)
                    {
                        var newDetalleHorario = model.detalleSeccion.Where(v => v.idSecciom == model.idSeccion);
                        if (newDetalleHorario != null)
                        {
                            detalleSet.AddRange(newDetalleHorario.Select(p => new SeccionHorarioDetalle()
                            {
                                idSecciom = created.idSeccion,
                                idAula = p.idAula,
                                idDia = p.idDia,
                                horaDesde = p.horaDesde,
                                horaHasta = p.horaHasta
                            }));
                            SaveChanges();
                        }
                    }

                    

                    trx.Commit();
                    return created;
                }
                catch (Exception E)
                {
                    trx.Rollback();
                    throw E;
                }
            }
        }

        public override void Edit(SeccionAsignaturaModel model)
        {
            using (var trx = dbContext.Database.BeginTransaction())
            {
                try
                {
                   
                    base.Edit(model, model.idSeccion);

                    var detalleSet = dbContext.Set<SeccionHorarioDetalle>();
                    if (model.detalleSeccion != null && model.detalleSeccion.Count() > 0)
                    {
                        detalleSet.RemoveRange(detalleSet.Where(p => p.idSecciom == model.idSeccion));

                        var newDetalleHorario = model.detalleSeccion.Where(v => v.idSecciom == model.idSeccion);
                        if (newDetalleHorario != null)
                        {
                            detalleSet.AddRange(newDetalleHorario.Select(p => new SeccionHorarioDetalle()
                            {
                                idSecciom = model.idSeccion,
                                idAula = p.idAula,
                                idDia = p.idDia,
                                horaDesde = p.horaDesde,
                                horaHasta = p.horaHasta
                            }));
                            SaveChanges();
                        }
                    }

                    //if (model.Usuarios != null && model.Usuarios.Count() > 0)
                    //{
                    //    var usuariosIds = model.Usuarios.Select(u => u.idUsuario);
                    //    var usuariosDarPerfil = dbContext.Set<Usuarios>().Where(u => usuariosIds.Contains(u.idUsuario)).ToList();
                    //    usuariosDarPerfil.ForEach(u => u.idPerfil = model.idPerfil);
                    //    SaveChanges();
                    //}

                    trx.Commit();
                }
                catch (Exception E)
                {
                    trx.Rollback();
                    throw E;
                }
            }
        }
    }
}
