using Data.Administration;
using Data.Entities;
using Models.Administration;
using Models.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Infraestructure;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/Publicacion")]
    public class PublicacionController : ApiBaseController
    {
        PublicacionRepo publicacionRepo = new PublicacionRepo();

        /// <summary>
        /// Obtiene un listado de los pensum registrados.
        /// </summary>
        /// <returns></returns>
        // GET api/Pensum
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public List<PublicacionModel> Get()
        {
            return publicacionRepo.Get().ToList();
        }

        /// <summary>
        /// Agrega una nueva calificacion (se necesita permiso de administrador).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Pensum
        [HttpPost]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Post(List<ListadoEstudiantesModel> model)
        {
            if (ValidateModel(model))
            {
                try
                {

                    foreach (var item in model)
                    {
                        if (item.calificacion > 100 || item.calificacion < 0)
                        {
                            return new OperationResult(false, "Ingresa una calificacion entre el rango de 0-100");
                        }
                    }

                    return publicacionRepo.PostPublicacionListado(model);
                    

                    PublicacionModel model2 = new PublicacionModel();

                    //model2.idListadoEstudiante = model.idListadoEstudiante;
                    //model2.idCalificacion = model.calificacion;
                    //model2.fechaPublicacion = DateTime.Now;

                   //VALIDAR LA INSERCION DE NOTAS VARIAS VECES O CAMBIAR CON PUT?????

                    //PublicacionModel publicacion = publicacionRepo.Get(x => x.idListadoEstudiante == model2.idListadoEstudiante).FirstOrDefault();
                    //if (publicacion != null)
                    //{
                    //    return new OperationResult(false, "Ya le pusiste calificacion a este estudiante");
                    //}

                    var created = publicacionRepo.Add(model2);
                    publicacionRepo.SaveChanges();
                    return new OperationResult(true, "Se ha colocado la clasificacion correctamente", created);



                }
                catch (Exception ex)
                {

                    publicacionRepo.LogError(ex);

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
        /// <param name="idPublicacion"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/Pensum/5
        [HttpPut]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Put(int idPublicacion, [FromBody] PublicacionModel model)
        {
            if (ValidateModel(model))
            {
                try
                {
                    if (model.idCalificacion > 100 || model.idCalificacion < 0)
                    {
                        return new OperationResult(false, "Ingresa una calificacion entre el rango de 0-100");
                    }

                   

                    
                    model.fechaPublicacion = DateTime.Now;

                   

                    publicacionRepo.Edit(model,idPublicacion);
                    publicacionRepo.SaveChanges();
                    return new OperationResult(true, "Se ha editado la clasificacion correctamente", model);



                }
                catch (Exception ex)
                {

                    publicacionRepo.LogError(ex);

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
