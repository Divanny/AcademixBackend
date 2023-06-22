using Data.Administration;
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


                var created = aulaRepo.Add(model);
                aulaRepo.SaveChanges();
                return new OperationResult(true, "Se ha creado esta aula satisfactoriamente", created);
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
                AulaModel aula = aulaRepo.Get(x => x.idAula == idAula).FirstOrDefault();

                if (aula == null)
                {
                    return new OperationResult(false, "Esta aula no existe.");
                }





                aulaRepo.Edit(model, idAula);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente");
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }
    }
}
