using Data.Administration;
using Data.Common;
using Data.Entities;
using Models.Administration;
using Models.Common;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Infraestructure;
using OnlineUser = WebAPI.Infraestructure.OnlineUser;

namespace WebAPI.Controllers
{
    /// <summary>
    /// API para manejar los estudiantes de las diferentes secciones.
    /// </summary>
    public class ListadoEstudiantesController : ApiBaseController
    {
        ListadoEstudiantesRepo listadoEstudiantesRepo = new ListadoEstudiantesRepo();
        SeccionAsignaturaRepo seccionAsignaturaRepo = new SeccionAsignaturaRepo();

        /// <summary>
        /// Obtiene un listado de las Areas registradas.
        /// </summary>
        /// <returns></returns>
        // GET api/Areas
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public List<ListadoEstudiantesModel> Get()
        {
            return listadoEstudiantesRepo.Get().ToList();
        }

        /// <summary>
        /// Obtiene un area en específico.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/Areas/5
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public ListadoEstudiantesModel Get(int id)
        {
            return listadoEstudiantesRepo.Get(x => x.idListadoEstudiante == id).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene todas las solicitudes realizadas por un usuario
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMisAsignaturasActual")]
        public List<SeccionAsignaturaModel> GetMisAsignaturasActual()
        {
            AcadmixEntities academixEntities = new AcadmixEntities();

            int idUsuario = OnlineUser.GetUserId();

            var idEstudiante = academixEntities.Estudiante
                           .Where(z => z.idUsuario == idUsuario)
                           .Select(x => x.idEstudiante)
                           .FirstOrDefault();

            List<int> ids = academixEntities.Listado_Estudiantes
                           .Where(z => z.idEstudiante == 9)
                           .Select(x => x.idSeccion).ToList();

            List<SeccionAsignaturaModel> seccionesEstudiante = new List<SeccionAsignaturaModel>();


            foreach (var item in ids)
            {
                seccionesEstudiante = seccionAsignaturaRepo.Get(x => x.idMaestro == item).ToList();
            }               

           
            return seccionesEstudiante;
        }

        /// <summary>
        /// Agrega una nueva area (se necesita permiso de administrador).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Areas
        [HttpPost]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Post([FromBody] ListadoEstudiantesModel model)
        {
            if (ValidateModel(model))
            {
                try
                {
                    AcadmixEntities academixEntities = new AcadmixEntities();
                    Utilities utilities = new Utilities();

                    model.idPeriodo = utilities.ObtenerTrimestreActual();
                    model.anioPeriodo = DateTime.Now.Year;


                    ListadoEstudiantesModel lista = listadoEstudiantesRepo.GetByIdSeccion(model.idSeccion,model.idEstudiante,model.anioPeriodo,model.idPeriodo);
                    if (lista != null)
                    {
                        return new OperationResult(false, "Ya perteneces a esta seccion");
                    }

                    int cupoActual = listadoEstudiantesRepo.CupoActual(model.idSeccion, model.anioPeriodo, model.idPeriodo);
                    int limite = listadoEstudiantesRepo.CapacidadMaxima(model.idSeccion);

                   

                    if (cupoActual >= limite)
                    {
                        return new OperationResult(false, "Esta seccion no tiene cupos disponible");
                    }
                    List<SeccionHorarioDetalle> detalle = listadoEstudiantesRepo.ObtenerHorarioPorIdSeccion(model.idSeccion);

                    foreach (var item in detalle)
                    {
                        
                        if (!listadoEstudiantesRepo.validarChoqueEstudiante(model.idEstudiante,model.anioPeriodo,model.idPeriodo, item.idDia, item.horaDesde, item.horaHasta))
                        {
                            return new OperationResult(false, $"El estudiante no tiene disponibilidad para el dia {(DiasSemanaEnum)item.idDia} en la hora proporcionada");
                        }

                    }

                    //Esto seria lo ultimo
                    int seccionAsignaturaRepetida = listadoEstudiantesRepo.verificarSeccionRepetida(model.idEstudiante, model.anioPeriodo, model.idPeriodo, model.idSeccion);

                    if(seccionAsignaturaRepetida != 0)
                    {
                        //Eliminar el registro de listadoEstudiante
                        listadoEstudiantesRepo.Delete(seccionAsignaturaRepetida);
                        
                    }

                    var created = listadoEstudiantesRepo.Add(model);
                    listadoEstudiantesRepo.SaveChanges();
                    return new OperationResult(true, "Se le ha agregado a esta seccion correctamente", created);
                }
                catch (Exception ex)
                {
                    listadoEstudiantesRepo.LogError(ex);

                    return new OperationResult(false, "Error en la inserción de datos");
                }


            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        /// <summary>
        /// Elimina un registro del listado estudiante.
        /// </summary>
        /// <param name="idPerfil"></param>
        /// <returns></returns>
        [HttpDelete]
        //[Autorizar(VistasEnum.GestionarPerfiles)]
        public OperationResult Delete(int idListadoEstudiante)
        {
            try
            {

                try
                {

                    listadoEstudiantesRepo.Delete(idListadoEstudiante);
                    //perfilesRepo.Log(idPerfil);
                    return new OperationResult(true, "Se ha eliminado satisfactoriamente");
                }
                catch (Exception ex)
                {

                    

                    return new OperationResult(false, "Error en la inserción de datos");
                }

            }
            catch (Exception ex)
            {
                return new OperationResult(false, "No se ha podido eliminar este perfil");
            }
        }


    }
}
