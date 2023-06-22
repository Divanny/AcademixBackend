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
    [RoutePrefix("api/Estudiantes")]
    public class EstudiantesController : ApiBaseController
    {
        
            EstudiantesRepo estudiantesRepo = new EstudiantesRepo();

            /// <summary>
            /// Obtiene un listado de los estudiantes registrados.
            /// </summary>
            /// <returns></returns>
            // GET api/Estudiantes
            [HttpGet]
            //[Autorizar(AllowAnyProfile = true)]
            public List<EstudiantesModel> Get()
            {
                return estudiantesRepo.Get().ToList();
            }

            /// <summary>
            /// Obtiene un estudiante en específico.
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            // GET api/Estudiantes/5
            [HttpGet]
            //[Autorizar(AllowAnyProfile = true)]
            public EstudiantesModel Get(int id)
            {
                return estudiantesRepo.Get(x => x.idEstudiante == id).FirstOrDefault();
            }

            /// <summary>
            /// Agrega un nuevo estudiante (se necesita permiso de administrador).
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            // POST api/Estudiantes
            [HttpPost]
            //[Autorizar(AllowAnyProfile = true)]
            public OperationResult Post([FromBody] EstudiantesModel model)
            {
                if (ValidateModel(model))
                {


                    var created = estudiantesRepo.Add(model);
                    estudiantesRepo.SaveChanges();
                    return new OperationResult(true, "Se ha creado este estudiante satisfactoriamente", created);
                }
                else
                {
                    return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
                }
            }

            /// <summary>
            /// Actualiza la información de un estudiante.
            /// </summary>
            /// <param name="idEstudiante"></param>
            /// <param name="model"></param>
            /// <returns></returns>
            // PUT api/Estudiantes/5
            [HttpPut]
            //[Autorizar(AllowAnyProfile = true)]
            public OperationResult Put(int idEstudiante, [FromBody] EstudiantesModel model)
            {
                if (ValidateModel(model))
                {
                    EstudiantesModel estudiante = estudiantesRepo.Get(x => x.idEstudiante == idEstudiante).FirstOrDefault();

                    if (estudiante == null)
                    {
                        return new OperationResult(false, "Este estudiante no existe.");
                    }





                    estudiantesRepo.Edit(model, idEstudiante);
                    return new OperationResult(true, "Se ha actualizado satisfactoriamente");
                }
                else
                {
                    return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
                }
            }
        
    }
}
