using Data.Administration;
using Data.Entities;
using Models.Administration;
using Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Infraestructure;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/Aula")]
    public class AulaController : ApiBaseController
    {
        AulaRepo aulaRepo = new AulaRepo();
        TipoAulaRepo tipoAulaRepo = new TipoAulaRepo();

        /// <summary>
        /// Obtiene un listado de las aulas registradas.
        /// </summary>
        /// <returns></returns>
        // GET api/Aula
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public List<AulaModel> Get()
        {
            return aulaRepo.Get().ToList();
        }

        /// <summary>
        /// Obtiene un aula en específico.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/Aula/5
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public AulaModel Get(int id)
        {
            return aulaRepo.Get(x => x.idAula == id).FirstOrDefault();
        }

        /// <summary>
        /// Agrega una nueva aula (se necesita permiso de administrador).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Aula
        [HttpPost]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Post([FromBody] AulaModel model)
        {
            if (ValidateModel(model))
            {
                try
                {
                AulaModel aula = aulaRepo.GetByName(model.nombre);
                if (aula != null)
                {
                    return new OperationResult(false, "Este nombre para un aula ya está registrado");
                }

                var created = aulaRepo.Add(model);
                aulaRepo.SaveChanges();
                return new OperationResult(true, "Se ha creado esta aula satisfactoriamente", created);

                }
                catch (Exception ex)
                {

                    aulaRepo.LogError(ex);

                    return new OperationResult(false, "Error en la inserción de datos");
                }

            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        /// <summary>
        /// Actualiza la información de una aula.
        /// </summary>
        /// <param name="idAula"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/Aula/5
        [HttpPut]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Put(int idAula, [FromBody] AulaModel model)
        {
            if (ValidateModel(model))
            {
                try
                {

                AulaModel aula = aulaRepo.Get(x => x.idAula == idAula).FirstOrDefault();

                if (aula == null)
                {
                    return new OperationResult(false, "Esta aula no existe.");
                }
                    AulaModel aulaName = aulaRepo.GetByName(model.nombre);
                if(aulaName != null) 
                {
                        return new OperationResult(false, "Ya existe un aula con este nombre");
                }

                aulaRepo.Edit(model, idAula);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente");
                }
                catch (Exception ex)
                {

                    aulaRepo.LogError(ex);

                    return new OperationResult(false, "Error en la inserción de datos");
                }

            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        [Route("GetTipoAula")]
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public List<TipoAulaModel> GetTipoAula()
        {
            return tipoAulaRepo.Get().ToList();
        }
    }
}
