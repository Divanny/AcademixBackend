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
    public class SolicitudesSoporteRepo:Repository<SolicitudesSoporte,SolicitudesSoporteModel>
    {
        public SolicitudesSoporteRepo(DbContext dbContext = null) : base
         (
             dbContext ?? new AcadmixEntities(),
             new ObjectsMapper<SolicitudesSoporteModel, SolicitudesSoporte>(s => new SolicitudesSoporte()
             {
                 idSolicitud = s.idSolicitud,
                 idUsuario = s.idUsuario,
                 idAsignadoA = s.idAsignadoA,
                 idEstatus = s.idEstatus,
                 Mensaje = s.Mensaje,
                 Respuesta = s.Respuesta,
                 FechaSolicitud = s.FechaSolicitud,
                 FechaUltimoEstatus = s.FechaUltimoEstatus,
             }),
             (DB, filter) => (from s in DB.Set<SolicitudesSoporte>().Where(filter)
                              join u in DB.Set<Usuarios>() on s.idUsuario equals u.idUsuario
                              join e in DB.Set<EstatusSolicitudesSoporte>() on s.idEstatus equals e.idEstatus
                              join pc in DB.Set<Usuarios>() on s.idAsignadoA equals pc.idUsuario into usuarioAsignado
                              from pc in usuarioAsignado.DefaultIfEmpty()
                              select new SolicitudesSoporteModel()
                              {
                                  idSolicitud = s.idSolicitud,
                                  idUsuario = s.idUsuario,
                                  Usuario = u.NombreUsuario,
                                  idAsignadoA = s.idAsignadoA,
                                  AsignadoA = (pc != null) ? pc.NombreUsuario : "",
                                  idEstatus = s.idEstatus,
                                  Estatus = e.Nombre,
                                  Mensaje = s.Mensaje,
                                  Respuesta = s.Respuesta,
                                  FechaSolicitud = s.FechaSolicitud,
                                  FechaUltimoEstatus = s.FechaUltimoEstatus,
                              })
         )
        { }
    }
}
