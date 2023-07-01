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
    [RoutePrefix("api/Areas")]
    public class AreasController : ApiBaseController
    {
        AreasRepo areasRepo = new AreasRepo();

        /// <summary>
        /// Obtiene un listado de las Areas registradas.
        /// </summary>
        /// <returns></returns>
        // GET api/Areas
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public List<AreasModel> Get()
        {
            return areasRepo.Get().ToList();
        }

        /// <summary>
        /// Obtiene un area en específico.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/Areas/5
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public AreasModel Get(int id)
        {
            return areasRepo.Get(x => x.idArea == id).FirstOrDefault();
        }

        /// <summary>
        /// Agrega una nueva area (se necesita permiso de administrador).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Areas
        [HttpPost]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Post([FromBody] AreasModel model)
        {
            if (ValidateModel(model))
            {
                try
                {
                    AreasModel area = areasRepo.GetByUsername(model.nombre);
                    if (area != null)
                    {
                        return new OperationResult(false, "Esta area ya está registrada");
                    }

                    var created = areasRepo.Add(model);
                    areasRepo.SaveChanges();
                    return new OperationResult(true, "Se ha creado esta area satisfactoriamente", created);
                }
                catch (Exception ex)
                {
                    areasRepo.LogError(ex);

                    return new OperationResult(false, "Error en la inserción de datos");
                }

                
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        /// <summary>
        /// Actualiza la información de una area.
        /// </summary>
        /// <param name="idArea"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/Areas/5
        [HttpPut]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Put(int idArea, [FromBody] AreasModel model)
        {
            if (ValidateModel(model))
            {
                try
                {
                AreasModel areas = areasRepo.Get(x => x.idArea == idArea).FirstOrDefault();

                if (areas == null)
                {
                    return new OperationResult(false, "Esta area no existe.");
                }

                var areaExists = areasRepo.Get(x => x.nombre == model.nombre).FirstOrDefault();

                if (areaExists != null)
                {
                    if (areaExists.idArea != idArea)
                    {
                        return new OperationResult(false, "Esta area ya esta registrada");
                    }
                }



                    areasRepo.Edit(model, idArea);
                    return new OperationResult(true, "Se ha actualizado satisfactoriamente");

                }
                catch (Exception ex) 
                { 

                    //areasRepo.LogError(ex);

                    return new OperationResult(false, "Error en la inserción de datos");
                }

            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

    }
}
