using Data.Common;
using Data.Entities;
using Models.Administration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
           (DB, filter) => 
           {
               SeccionHorarioDetalleRepo seccionHorarioDetalleRepo = new SeccionHorarioDetalleRepo(dbContext);


              return( from u in DB.Set<Seccion_Asignatura>().Where(filter)
               join m in DB.Set<Modalidad>() on u.idModalidad equals m.idModalidad
               join a in DB.Set<Asignatura>() on u.idAsignatura equals a.idAsignatura
               join p in DB.Set<Maestro>() on u.idMaestro equals p.idMaestro
               join s in DB.Set<Usuarios>() on p.idUsuario equals s.idUsuario
               let detalleHorario = seccionHorarioDetalleRepo.Get(x => x.idSecciom == u.idSeccion)
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
                   detalleSeccion = detalleHorario


               });
           }   
       )
        { }

        public SeccionAsignaturaModel GetByCode(int codigoSeccion, int idAsignatura)
        {
            return this.Get(x => x.codigoSeccion == codigoSeccion && x.idAsignatura == idAsignatura).FirstOrDefault();
        }
        public IEnumerable<SeccionHorarioDetalleModel> GetHorariosDetalles(int idSeccion)
        {
            return from u in dbContext.Set<SeccionHorarioDetalle>().Where(u => u.idSecciom == idSeccion )
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
        public bool ValidarChoquesdeHoraAula(int idAula, int diaId, TimeSpan horaDesde, TimeSpan horaHasta, int idSeccion)
        {
            SeccionHorarioDetalleRepo seccionHorarioDetalleRepo = new SeccionHorarioDetalleRepo();


            var HorariosProfesor = this.Get()
                .Where(x => x.idSeccion != idSeccion)
                .ToList();

            foreach (var detallesItem in HorariosProfesor)
            {
                detallesItem.detalleSeccion = seccionHorarioDetalleRepo.Get()
                .Where(x => x.idSecciom == detallesItem.idSeccion && x.idDia == diaId && x.idAula == idAula).ToList();

            }


            bool Disponible = true;

            foreach (var item in HorariosProfesor)
            {
                Disponible = !item.detalleSeccion
                   .Where(x =>
                   horaDesde == x.horaDesde
                   || horaHasta == x.horaHasta
                   || (horaDesde >= x.horaDesde && horaDesde < x.horaHasta)
                   || (horaHasta > x.horaDesde && horaHasta <= x.horaHasta)
                   || (horaHasta > x.horaHasta && horaDesde < x.horaDesde)

                  ).Any();
                if (Disponible == false)
                {
                    return Disponible;
                }

            }

            return Disponible;




        }


        public  bool ValidarChoquesdeHora(int idMaestro, int diaId, TimeSpan horaDesde, TimeSpan horaHasta, int idSeccion)
        {
            SeccionHorarioDetalleRepo seccionHorarioDetalleRepo = new SeccionHorarioDetalleRepo();


            var HorariosProfesor = this.Get()
                .Where(x => x.idMaestro == idMaestro && x.idSeccion != idSeccion)
                .ToList();

            foreach(var detallesItem in HorariosProfesor)
            {
                detallesItem.detalleSeccion = seccionHorarioDetalleRepo.Get()
                .Where(x => x.idSecciom == detallesItem.idSeccion && x.idDia == diaId ).ToList();

            }


            bool Disponible = true;

            foreach (var item in HorariosProfesor)
            {
                 Disponible = !item.detalleSeccion
                    .Where(x =>
                    horaDesde == x.horaDesde
                    || horaHasta == x.horaHasta
                    || (horaDesde >= x.horaDesde && horaDesde < x.horaHasta)
                    || (horaHasta > x.horaDesde && horaHasta <= x.horaHasta)
                    || (horaHasta > x.horaHasta && horaDesde < x.horaDesde)

                   ).Any();
                if (Disponible == false) 
                {
                    return Disponible;
                }

            }

            return Disponible;




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

        public List<SeccionAsignaturaModel> GetMaestroSecciones()
        {
            AcadmixEntities academixEntities = new AcadmixEntities();

            //int idUsuario = OnlineUser.GetUserId();

            var idMaestro = academixEntities.Maestro
                           .Where(z => z.idUsuario == 14)
                           .Select(x => x.idMaestro)
                           .FirstOrDefault();

            List<SeccionAsignaturaModel> seccionesMaestro = this.Get(x => x.idMaestro == idMaestro).ToList();
            return seccionesMaestro;
        
        }

        public List<ListadoEstudiantesModel> GetListadoEstudiantes()
        {
            AcadmixEntities academixEntities = new AcadmixEntities();
            ListadoEstudiantesRepo listadoEstudiantesRepo = new ListadoEstudiantesRepo();
            Utilities utilities = new Utilities();

           
            int Periodo = utilities.ObtenerTrimestreActual();


            List<SeccionAsignaturaModel> seccionesMaestro = this.GetMaestroSecciones();



            List<ListadoEstudiantesModel> listadoEstudiantes = new List<ListadoEstudiantesModel>();
            foreach (var item in seccionesMaestro)
            {
                var estudiantes = listadoEstudiantesRepo.Get(x => x.idSeccion == item.idSeccion && x.idPeriodo == Periodo && x.anioPeriodo == DateTime.Now.Year).ToList();
                listadoEstudiantes.AddRange(estudiantes);
            }

                 var listadoEstudiantesModel = listadoEstudiantes
                    .GroupBy(x => x.idSeccion)
                    .SelectMany(g => g.Select(estudiante => new ListadoEstudiantesModel
                    {
                        idListadoEstudiante = estudiante.idListadoEstudiante,
                        idSeccion = estudiante.idSeccion,
                        codigoSeccion = estudiante.codigoSeccion,
                        nombreAsignatura = estudiante.nombreAsignatura,
                        idEstudiante = estudiante.idEstudiante,
                        idPeriodo = estudiante.idPeriodo,
                        anioPeriodo = estudiante.anioPeriodo,
                    }))
                    .ToList();



            return listadoEstudiantesModel;
        }

    }
}
