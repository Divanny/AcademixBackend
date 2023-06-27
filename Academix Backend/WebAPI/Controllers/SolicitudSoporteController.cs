using Data.Administration;
using Models.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Infraestructure;
using System.Web.Http;
using Data.Entities;
using System.Security.Cryptography.X509Certificates;
using Swashbuckle.Swagger;
using Models.Enums;
using Models.Common;

namespace WebAPI.Controllers
{
    /// <summary>
    /// API para manejar las solicitudes de soporte
    /// </summary>
    [RoutePrefix("api/SolicitudesSoporte")]
    public class SolicitudSoporteController : ApiBaseController
    {
        SolicitudesSoporteRepo solicitudesSoporteRepo = new SolicitudesSoporteRepo();

        /// <summary>
        /// Obtiene un listado de todos las solicitudes de soporte no atendidas.
        /// </summary>
        /// <returns></returns>
        //[Autorizar(VistasEnum.GestionarPerfiles)]
        [HttpGet]
        public List<SolicitudesSoporteModel> Get()
        {
            List<SolicitudesSoporteModel> solicitudesSoportes = solicitudesSoporteRepo.Get().ToList();
            return solicitudesSoportes;
        }

        /// <summary>
        /// Obtiene una solicitud en específico.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/SolicitudesSoporte/5
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public SolicitudesSoporteModel Get(int id)
        {
            return solicitudesSoporteRepo.Get(x => x.idSolicitud == id).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene todas las solicitudes realizadas por un usuario
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMisSolicitudes")]
        public List<SolicitudesSoporteModel> GetMisSolicitudes()
        {
            List<SolicitudesSoporteModel> solicitudesSoportes = solicitudesSoporteRepo.Get(x => x.idUsuario == OnlineUser.GetUserId()).ToList();
            return solicitudesSoportes;
        }

        /// <summary>
        /// Obtiene una bandeja de las solictudes de soporte sin responder
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBandejaSolicitudes")]
        public List<SolicitudesSoporteModel> GetBandejaSolicitudes()
        {
            List<SolicitudesSoporteModel> solicitudesSoportes = solicitudesSoporteRepo.Get(x => x.idEstatus == (int)EstatusSolicitudSoporteEnum.Enrevisión || x.idEstatus == (int)EstatusSolicitudSoporteEnum.PendienteaRevisión).ToList();
            return solicitudesSoportes;
        }

        /// <summary>
        /// Agrega una nuevo solicitud de soporte
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/SolicitudesSoporte
        [HttpPost]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Post([FromBody] SolicitudesSoporteModel model)
        {
            if (ValidateModel(model))
            {
                try
                {
                    model.idEstatus = (int)EstatusSolicitudSoporteEnum.PendienteaEnviar;
                    model.idUsuario = OnlineUser.GetUserId();
                    model.FechaSolicitud = DateTime.Now;
                    model.FechaUltimoEstatus = DateTime.Now;

                    var created = solicitudesSoporteRepo.Add(model);
                    solicitudesSoporteRepo.SaveChanges();
                    return new OperationResult(true, "Se ha creado esta solicitud satisfactoriamente", created);

                }
                catch (Exception ex)
                {

                    solicitudesSoporteRepo.LogError(ex);

                    return new OperationResult(false, "Error en la inserción de datos");
                }

            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        /// <summary>
        /// Actualiza la información de una solicitud de soporte.
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/SolicitudesSoporte/5
        [HttpPut]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Put(int idSolicitud, [FromBody] SolicitudesSoporteModel model)
        {
            if (ValidateModel(model))
            {

                try
                {

                    SolicitudesSoporteModel solicitud = solicitudesSoporteRepo.Get(x => x.idSolicitud == idSolicitud).FirstOrDefault();

                    if (solicitud == null)
                    {
                        return new OperationResult(false, "Esta solicitud no existe.");
                    }

                    solicitudesSoporteRepo.Edit(model, idSolicitud);
                    return new OperationResult(true, "Se ha actualizado satisfactoriamente");
                }
                catch (Exception ex)
                {

                    solicitudesSoporteRepo.LogError(ex);

                    return new OperationResult(false, "Error en la inserción de datos");
                }

            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        /// <summary>
        /// Enviar la solicitud de un usuario a bandeja
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Enviar")]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Enviar(int idSolicitud)
        {
            SolicitudesSoporteModel solicitud = solicitudesSoporteRepo.Get(x => x.idSolicitud == idSolicitud).FirstOrDefault();

            if (solicitud == null)
            {
                return new OperationResult(false, "Esta solicitud no existe.");
            }

            solicitud.idEstatus = (int)EstatusSolicitudSoporteEnum.PendienteaRevisión;
            solicitud.FechaUltimoEstatus = DateTime.Now;
            solicitudesSoporteRepo.Edit(solicitud, idSolicitud);
            return new OperationResult(true, "Se ha enviado satisfactoriamente");
        }

        [HttpPut]
        [Route("Asignar")]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Asignar(int idSolicitud)
        {
            SolicitudesSoporteModel solicitud = solicitudesSoporteRepo.Get(x => x.idSolicitud == idSolicitud).FirstOrDefault();

            if (solicitud == null)
            {
                return new OperationResult(false, "Esta solicitud no existe.");
            }

            if (solicitud.idAsignadoA != null && solicitud.idAsignadoA != 0)
            {
                return new OperationResult(false, $"Esta ya está asignada a {solicitud.AsignadoA}.");
            }

            solicitud.idEstatus = (int)EstatusSolicitudSoporteEnum.Enrevisión;
            solicitud.FechaUltimoEstatus = DateTime.Now;
            solicitud.idAsignadoA = OnlineUser.GetUserId();
            solicitudesSoporteRepo.Edit(solicitud, idSolicitud);

            return new OperationResult(true, "Se ha asignado satisfactoriamente");
        }


        /// <summary>
        /// Cambia el estatus de una solicitud
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <param name="idEstatus"></param>
        /// <param name="Mensaje"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("CambiarEstatus")]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult CambiarEstatus(int idSolicitud, int idEstatus, string Mensaje)
        {
            SolicitudesSoporteModel solicitud = solicitudesSoporteRepo.Get(x => x.idSolicitud == idSolicitud).FirstOrDefault();

            if (solicitud == null)
            {
                return new OperationResult(false, "Esta solicitud no existe.");
            }

            solicitud.idEstatus = idEstatus;
            solicitud.FechaUltimoEstatus = DateTime.Now;

            if (Mensaje != null || Mensaje != "")
            {
                solicitud.Respuesta = Mensaje;
            }

            solicitudesSoporteRepo.Edit(solicitud, idSolicitud);

            solicitud = solicitudesSoporteRepo.Get(x => x.idSolicitud == idSolicitud).FirstOrDefault();

            return new OperationResult(true, $"El estatus ha cambiado a {solicitud.Estatus} satisfactoriamente");
        }

        /// <summary>
        /// Obtiene todos los estatus que puede pasar una solicitud de soporte
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetEstatusSolicitud")]
        public List<EstatusSolicitudesSoporte> GetEstatusSolicitud()
        {
            List<EstatusSolicitudesSoporte> estatusSolicitud = solicitudesSoporteRepo.dbContext.Set<EstatusSolicitudesSoporte>().ToList();
            return estatusSolicitud;
        }
    }
}