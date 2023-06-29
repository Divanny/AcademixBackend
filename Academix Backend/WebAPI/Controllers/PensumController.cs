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
    [RoutePrefix("api/Pensum")]
    public class PensumController : ApiBaseController
    {
        PensumRepo pensumRepo = new PensumRepo();
        //AsignaturaPensumRepo asignaturaPensumRepo = new AsignaturaPensumRepo();
        TrimestreRepo trimestreRepo = new TrimestreRepo();  

        /// <summary>
        /// Obtiene un listado de los pensum registrados.
        /// </summary>
        /// <returns></returns>
        // GET api/Pensum
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public List<PensumModel> Get()
        {
            return pensumRepo.Get().ToList();
        }

        /// <summary>
        /// Obtiene un pensum en específico.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/Pensum/5
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public PensumModel Get(int id)
        {
            return pensumRepo.Get(x => x.idPensum == id).FirstOrDefault();
        }

        /// <summary>
        /// Agrega un nuevo pensum (se necesita permiso de administrador).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Pensum
        [HttpPost]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Post([FromBody] PensumModel model)
        {
            if (ValidateModel(model))
            {
                try
                {

                PensumModel pensum = pensumRepo.GetByName(model.nombrePensum);
                if(pensum != null)
                {
                        return new OperationResult(false, "Ya existe un pensum con este nombre");
                }

                var created = pensumRepo.Add(model);
                pensumRepo.SaveChanges();
                return new OperationResult(true, "Se ha creado este pensum satisfactoriamente", created);
                }
                catch (Exception ex)
                {

                    pensumRepo.LogError(ex);

                    return new OperationResult(false, "Error en la inserción de datos");
                }

            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        /// <summary>
        /// Actualiza la información de un pensum.
        /// </summary>
        /// <param name="idPensum"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/Pensum/5
        [HttpPut]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Put(int idPensum, [FromBody] PensumModel model)
        {
            if (ValidateModel(model))
            {
                try
                {

                PensumModel pensum = pensumRepo.Get(x => x.idPensum == idPensum).FirstOrDefault();

                if (pensum == null)
                {
                    return new OperationResult(false, "Este pensum no existe.");
                }




                pensumRepo.Edit(model, idPensum);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente");
                }
                catch (Exception ex)
                {

                    pensumRepo.LogError(ex);

                    return new OperationResult(false, "Error en la inserción de datos");
                }

            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }


        [Route("PostAsignaturaPensum")]
        [HttpPost]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult PostAsignaturaPensum(int idPensum, int idTrimestre, List<int> idAsignaturas)
        {
            if (idAsignaturas != null)
            {
                try
                {
                    return pensumRepo.PostAsignaturaPensum(idPensum, idTrimestre, idAsignaturas);
                }
                catch (Exception ex)
                {
                    pensumRepo.LogError(ex);
                    return new OperationResult(false, "Error en la inserción de datos");
                }
            }
            else
            {
                return new OperationResult(false, "No se han enviado las asignaturas");
            }
        }

        /// <summary>
        /// Obtiene un listado de los Trimestres registrados.
        /// </summary>
        /// <returns></returns>
        // GET api/Pensum
        [Route("GetTrimestre")]
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public List<TrimestreModel> GetTrimestre()
        {
            return trimestreRepo.Get().ToList();
        }
    }
}
