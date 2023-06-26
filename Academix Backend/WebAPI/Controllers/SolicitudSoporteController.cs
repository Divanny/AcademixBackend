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

        [HttpGet]
        [Route("GetBandejaSolicitudes")]
        public List<SolicitudesSoporteModel> GetBandejaSolicitudes()
        {
            List<SolicitudesSoporteModel> solicitudesSoportes = solicitudesSoporteRepo.Get(x => x.idEstatus == (int)EstatusSolicitudSoporteEnum.Enrevisión).ToList();
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
                model.idEstatus = (int)EstatusSolicitudSoporteEnum.PendienteaEnviar;
                model.idUsuario = OnlineUser.GetUserId();
                model.FechaSolicitud = DateTime.Now;

                var created = solicitudesSoporteRepo.Add(model);
                solicitudesSoporteRepo.SaveChanges();
                return new OperationResult(true, "Se ha creado esta solicitud satisfactoriamente", created);
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
                SolicitudesSoporteModel solicitud = solicitudesSoporteRepo.Get(x => x.idSolicitud == idSolicitud).FirstOrDefault();

                if (solicitud == null)
                {
                    return new OperationResult(false, "Esta solicitud no existe.");
                }

                solicitudesSoporteRepo.Edit(model, idSolicitud);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente");
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
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
            return new OperationResult(true, "Se ha actualizado satisfactoriamente");
        }
    }
}