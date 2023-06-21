using Data.Administration;
using Models.Administration;
using Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using WebAPI.Infraestructure;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/Maestros")]
    public class MaestrosController : ApiBaseController
    {
        MaestrosRepo maestrosRepo = new MaestrosRepo();

        /// <summary>
        /// Obtiene un listado de los Maestros registrados.
        /// </summary>
        /// <returns></returns>
        // GET api/Maestros
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public List<MaestroModel> Get()
        {
            return maestrosRepo.Get().ToList();
        }

        /// <summary>
        /// Obtiene un maestro en específico.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/Maestros/5
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public MaestroModel Get(int id)
        {
            return maestrosRepo.Get(x => x.idMaestro == id).FirstOrDefault();
        }

        /// <summary>
        /// Agrega un nuevo maestro (se necesita permiso de administrador).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Maestros
        [HttpPost]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Post([FromBody] MaestroModel model)
        {
            if (ValidateModel(model))
            {
                

                var created = maestrosRepo.Add(model);
                maestrosRepo.SaveChanges();
                return new OperationResult(true, "Se ha creado este maestro satisfactoriamente", created);
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        /// <summary>
        /// Actualiza la información de un maestro.
        /// </summary>
        /// <param name="idMaestro"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/Maestros/5
        [HttpPut]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Put(int idMaestro, [FromBody] MaestroModel model)
        {
            if (ValidateModel(model))
            {
                MaestroModel maestros = maestrosRepo.Get(x => x.idMaestro == idMaestro).FirstOrDefault();

                if (maestros == null)
                {
                    return new OperationResult(false, "Este maestro no existe.");
                }

                



                maestrosRepo.Edit(model, idMaestro);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente");
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }
    }
}
