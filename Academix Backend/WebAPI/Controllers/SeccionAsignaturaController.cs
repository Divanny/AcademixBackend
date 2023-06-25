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

namespace WebAPI.Controllers
{
    [RoutePrefix("api/SeccionAsignatura")]
    public class SeccionAsignaturaController : ApiBaseController
    {
        //public AcadmixEntities academixEntities { get; set; }
        //public UsuariosRepo usuariosRepo { get; set; }
        //public PerfilesRepo perfilesRepo { get; set; }
        //public MaestrosRepo maestrosRepo { get; set; }
        //public EstudiantesRepo estudiantesRepo { get; set; }
        //public SeccionAsignaturaRepo seccionAsignaturaRepo { get; set; }
        //public SeccionHorarioDetalleRepo seccionHorarioDetalleRepo { get; set; }

        Utilities utilities = new Utilities();
        SeccionAsignaturaRepo seccionAsignaturaRepo = new SeccionAsignaturaRepo();
        //public SeccionAsignaturaController()
        //{
        //    academixEntities = new AcadmixEntities();
        //    usuariosRepo = new UsuariosRepo(academixEntities);
        //    perfilesRepo = new PerfilesRepo(academixEntities);
        //    maestrosRepo = new MaestrosRepo(academixEntities);
        //    estudiantesRepo = new EstudiantesRepo(academixEntities);
        //    seccionHorarioDetalleRepo = new SeccionHorarioDetalleRepo(academixEntities);
        //    seccionAsignaturaRepo = new SeccionAsignaturaRepo(academixEntities);
        //}

        /// <summary>
        /// Obtiene un listado de las secciones registradas.
        /// </summary>
        /// <returns></returns>
        // GET api/Usuarios
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public List<SeccionAsignaturaModel> Get()
        {
            List<SeccionAsignaturaModel> secciones = seccionAsignaturaRepo.Get().ToList();
            foreach (var item in secciones)
            {
                item.detalleSeccion = GetHorariosDetalles(item.idSeccion);
            }
            return secciones;
            
        }

        /// <summary>
        /// Obtiene una seccion en específico.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/Usuarios/5
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public SeccionAsignaturaModel Get(int id)
        {
            return seccionAsignaturaRepo.Get(x => x.idSeccion == id).FirstOrDefault();
        }

        /// <summary>
        /// Agrega una nueva seccion (se necesita permiso de administrador).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Usuarios
        [HttpPost]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Post([FromBody] SeccionAsignaturaModel model)
        {
            if (ValidateModel(model))
            {
               
                       //-> Manejar el estado de las secciones
                        
                        //if (model.esActivo == false)
                        //{
                        //    model.esActivo = (int)EstadoUsuarioEnum.Activo;
                        //}

                       

                        

                        if(model.idModalidad == 3)
                        {
                            return new OperationResult(false, "Se ha creado la seccion Asincronica");
                        }
                        else
                        {
                           if(model.detalleSeccion != null)
                           {
                        List<SeccionHorarioDetalleModel> maestroDetalle = seccionAsignaturaRepo.GetHorarioByMaesto(model.idMaestro);
                        List <SeccionHorarioDetalleModel> detalles = seccionAsignaturaRepo.GetHorarioByAsignatura(model.idAsignatura);
                                 if (detalles == null) 
                                 {
                                     return new OperationResult(false, "Ya existe una seccion en ese horario para esta asignatura");
                                 }

                                var created = seccionAsignaturaRepo.Add(model);
                                seccionAsignaturaRepo.SaveChanges();
                                return new OperationResult(true, "Se ha creado esta seccion satisfactoriamente", created);
                                
                           }

                           else
                           {
                               return new OperationResult(false, "No se ha proporcionado información del detalle de la seccion");
                           }
                            
                        }

                        

                        
                        

                    
                
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        /// <summary>
        /// Actualiza la información de un usuario.
        /// </summary>
        /// <param name="idSeccion"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/Usuarios/5
        [HttpPut]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Put(int idSeccion, [FromBody] SeccionAsignaturaModel model)
        {
            if (ValidateModel(model))
            {
               
                        //-> Manejar el estado de las secciones

                        //if (model.esActivo == false)
                        //{
                        //    model.esActivo = (int)EstadoUsuarioEnum.Activo;
                        //}

                        if (model.idModalidad == 3)
                        {
                            return new OperationResult(false, "Se ha creado la seccion Asincronica");
                        }
                        else
                        {
                            if (model.detalleSeccion != null)
                            {
                                seccionAsignaturaRepo.Edit(model);
                                seccionAsignaturaRepo.SaveChanges();
                                return new OperationResult(true, "Se ha creado esta seccion satisfactoriamente",model);

                            }

                            else
                            {
                                return new OperationResult(false, "No se ha proporcionado información del detalle de la seccion");
                            }

                        }






                    
                
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        [HttpGet]
        [Route("GetHorarios")]
        public List<SeccionHorarioDetalleModel> GetHorariosDetalles(int idSeccion)
        {
            return seccionAsignaturaRepo.GetHorariosDetalles(idSeccion).ToList();
        }


    }
}
