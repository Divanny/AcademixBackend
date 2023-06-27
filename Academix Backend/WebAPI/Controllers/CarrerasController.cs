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
    [RoutePrefix("api/Carreras")]
    public class CarrerasController : ApiBaseController
    {
        CarrerasRepo carrerasRepo = new CarrerasRepo();

        /// <summary>
        /// Obtiene un listado de las carreras registradas.
        /// </summary>
        /// <returns></returns>
        // GET api/Carreras
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public List<CarrerasModel> Get()
        {
            return carrerasRepo.Get().ToList();
        }

        /// <summary>
        /// Obtiene una carrera en específico.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/Carreras/5
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public CarrerasModel Get(int id)
        {
            return carrerasRepo.Get(x => x.idCarrera == id).FirstOrDefault();
        }

        /// <summary>
        /// Agrega una nueva carrera (se necesita permiso de administrador).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Carreras
        [HttpPost]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Post([FromBody] CarrerasModel model)
        {
            if (ValidateModel(model))
            {
                try
                {

                CarrerasModel asignatura = carrerasRepo.GetByCarreraName(model.nombre);
                if (asignatura != null)
                {
                    return new OperationResult(false, "Esta carrera ya está registrada");
                }
               
                var created = carrerasRepo.Add(model);
                carrerasRepo.SaveChanges();
                return new OperationResult(true, "Se ha creado esta carrera satisfactoriamente", created);
                }
                catch (Exception ex)
                {

                    carrerasRepo.LogError(ex);

                    return new OperationResult(false, "Error en la inserción de datos");
                }

            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        /// <summary>
        /// Actualiza la información de una carrera.
        /// </summary>
        /// <param name="idCarrera"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/Carreras/5
        [HttpPut]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Put(int idCarrera, [FromBody] CarrerasModel model)
        {
            if (ValidateModel(model))
            {
                CarrerasModel carreras = carrerasRepo.Get(x => x.idCarrera == idCarrera).FirstOrDefault();

                if (carreras == null)
                {
                    return new OperationResult(false, "Esta carrera no existe.");
                }

                var carreraExists = carrerasRepo.Get(x => x.nombre == model.nombre).FirstOrDefault();

                if (carreraExists != null)
                {
                    if (carreraExists.idCarrera != idCarrera)
                    {
                        return new OperationResult(false, "Esta carrera ya esta registrada");
                    }
                }



                carrerasRepo.Edit(model, idCarrera);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente");
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }
    }
}
