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
    public class AsignaturasRepo : Repository<Asignatura, AsignaturasModel>
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
                esActivo = u.esActivo,
                Icon = u.Icon,
                Color = u.Color
            }),
            (DB, filter) => (from u in DB.Set<Asignatura>().Where(filter)
                             join a in DB.Set<Area>() on u.idArea equals a.idArea into area
                             from a in area.DefaultIfEmpty()
                             join c in DB.Set<Carrera>() on u.idCarrera equals c.idCarrera into carrera
                             from c in carrera.DefaultIfEmpty()
                             select new AsignaturasModel()
                             {
                                 idAsignatura = u.idAsignatura,
                                 NombreAsignatura = u.nombreAsignatura,
                                 CodigoAsignatura = u.codigoAsignatura,
                                 idArea = a != null ? a.idArea : (int?)null,
                                 Area = a != null ? a.nombre : null,
                                 Creditos = u.creditos,
                                 idCarrera = c != null ? c.idCarrera : (int?)null,
                                 Carrera = c != null ? c.nombre : null,
                                 esActivo = u.esActivo,
                                 Icon = u.Icon,
                                 Color = u.Color
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

        public override Asignatura Add(AsignaturasModel model)
        {
            using (var trx = dbContext.Database.BeginTransaction())
            {
                try
                {


                    Asignatura created = base.Add(model);

                    var detalleSet = dbContext.Set<Dependencia>();
                    if (model.Dependencias != null && model.Dependencias.Count() > 0)
                    {
                        var newDetalleHorario = model.Dependencias.Where(v => v.idAsignatura == model.idAsignatura);
                        if (newDetalleHorario != null)
                        {
                            detalleSet.AddRange(newDetalleHorario.Select(p => new Dependencia()
                            {
                                idDependencia = p.idAsignatura,
                                idAsignatura = model.idAsignatura,
                                idPrerrequisito = p.idPrerrequisito,
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

        public override void Edit(AsignaturasModel model)
        {
            using (var trx = dbContext.Database.BeginTransaction())
            {
                try
                {

                    base.Edit(model, model.idAsignatura);

                    var detalleSet = dbContext.Set<Dependencia>();
                    if (model.Dependencias != null && model.Dependencias.Count() > 0)
                    {
                        detalleSet.RemoveRange(detalleSet.Where(p => p.idAsignatura == model.idAsignatura));

                        var newDetalleHorario = model.Dependencias.Where(v => v.idAsignatura == model.idAsignatura);
                        if (newDetalleHorario != null)
                        {
                            detalleSet.AddRange(newDetalleHorario.Select(p => new Dependencia()
                            {
                                idDependencia = p.idAsignatura,
                                idAsignatura = p.idAsignatura,
                                idPrerrequisito = p.idPrerrequisito,
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
