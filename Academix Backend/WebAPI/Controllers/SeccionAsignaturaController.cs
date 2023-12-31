﻿using Data.Administration;
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
    [RoutePrefix("api/SeccionAsignatura")]
    public class SeccionAsignaturaController : ApiBaseController
    {
        public AcadmixEntities academixEntities { get; set; }
        public SeccionAsignaturaRepo seccionAsignaturaRepo { get; set; }
        public SeccionHorarioDetalleRepo seccionHorarioDetalleRepo { get; set; }

        Utilities utilities = new Utilities();

        public SeccionAsignaturaController()
        {
            academixEntities = new AcadmixEntities();
            seccionHorarioDetalleRepo = new SeccionHorarioDetalleRepo(academixEntities);
            seccionAsignaturaRepo = new SeccionAsignaturaRepo(academixEntities);
        }

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
        /// Obtiene todas las secciones enlazadas por un usuario
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMisSeccionesMaestro")]
        public List<SeccionAsignaturaModel> GetMisSeccionesMaestro()
        {
            List<SeccionAsignaturaModel> seccionesMaestro = seccionAsignaturaRepo.GetMaestroSecciones();
            return seccionesMaestro;

        }

        [HttpGet]
        [Route("ListadoEstudianteMaestro/{idSeccion}")]
        public List<ListadoEstudiantesModel> ListadoEstudianteMaestro(int idSeccion)
        {
            List<ListadoEstudiantesModel> listadoEstudiantes = seccionAsignaturaRepo.GetListadoEstudiantes(idSeccion, OnlineUser.GetUserId());
            return listadoEstudiantes;
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

                try
                {
                    Seccion_Asignatura created;
                    
                    SeccionAsignaturaModel seccionCode = seccionAsignaturaRepo.GetByCode(model.codigoSeccion, model.idAsignatura);
                    if (seccionCode != null)
                    {
                        return new OperationResult(false, "Este codigo de seccion ya está registrado");
                    }


                    if (model.idModalidad == (int)ModalidadesEnum.Asignaturaasincrónica)
                    {
                        if (model.detalleSeccion != null)
                        {
                            return new OperationResult(false, "No se puede agregar horario a una seccion Asincronica");
                        }
                        else
                        {
                            created = seccionAsignaturaRepo.Add(model);
                            seccionAsignaturaRepo.SaveChanges();
                            return new OperationResult(true, "Se ha creado la seccion Asincronica", created);
                        }

                    }
                    else
                    {
                        if (model.detalleSeccion != null)
                        {
                            AcadmixEntities academixEntities = new AcadmixEntities();


                            var infoAsignatura = academixEntities.Asignatura
                                           .Where(creditos => creditos.idAsignatura == model.idAsignatura)
                                           .Select(x => new { x.creditos, x.nombreAsignatura } )
                                           .FirstOrDefault();

                            

                            int diferenciaTotal = 0;
                            

                            foreach (var item in model.detalleSeccion)
                            {
                                if (item.horaHasta <= item.horaDesde)
                                {
                                    return new OperationResult(false, "No se puede crear una hora de finalizacion mayor que la de inicio");
                                }
                                if (!seccionAsignaturaRepo.ValidarChoquesdeHora(model.idMaestro, item.idDia, item.horaDesde, item.horaHasta, 0))
                                {
                                    return new OperationResult(false, $"El maestro no tiene disponibilidad para el dia {(DiasSemanaEnum)item.idDia} en la hora proporcionada");
                                }
                                if (!seccionAsignaturaRepo.ValidarChoquesdeHoraAula(item.idAula, item.idDia, item.horaDesde, item.horaHasta, 0))
                                {
                                    return new OperationResult(false, $"El aula no tiene disponibilidad para el dia {(DiasSemanaEnum)item.idDia} en la hora proporcionada");
                                }

                                int diferencia = (int)(item.horaHasta - item.horaDesde).TotalHours;
                                diferenciaTotal += diferencia;

                               



                            }

                            if (diferenciaTotal > infoAsignatura.creditos)
                            {
                                return new OperationResult(false, $"No puedes agregar mas horas que la cantidad de creditos que tiene la asignatura {infoAsignatura.nombreAsignatura}");
                            }
                            if (diferenciaTotal <= infoAsignatura.creditos - 2)
                            {
                                return new OperationResult(false, $"No puedes agregar menos horas que la cantidad de creditos que tiene la asignatura {infoAsignatura.nombreAsignatura}");
                            }

                            created = seccionAsignaturaRepo.Add(model);
                            seccionAsignaturaRepo.SaveChanges();
                            return new OperationResult(true, "Se ha creado la seccion", created);


                        }

                        else
                        {
                            return new OperationResult(false, "No se ha proporcionado información del detalle de la seccion");
                        }

                    }

                    
                }
                catch(Exception ex)
                {
                    seccionAsignaturaRepo.LogError(ex);

                    return new OperationResult(false, "Error en la inserción de datos");
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

                try
                {
                    Seccion_Asignatura created;


                    if (model.idModalidad == (int)ModalidadesEnum.Asignaturaasincrónica)
                    {
                        if (model.detalleSeccion != null)
                        {
                            return new OperationResult(false, "No se puede agregar horario a una seccion Asincronica");
                        }
                        else
                        {
                            seccionAsignaturaRepo.Edit(model,idSeccion);
                            seccionAsignaturaRepo.SaveChanges();
                            return new OperationResult(true, "Se ha creado la seccion Asincronica",model);
                        }

                    }
                    else
                    {
                        if (model.detalleSeccion != null)
                        {
                            AcadmixEntities academixEntities = new AcadmixEntities();


                            var infoAsignatura = academixEntities.Asignatura
                                           .Where(creditos => creditos.idAsignatura == model.idAsignatura)
                                           .Select(x => new { x.creditos, x.nombreAsignatura })
                                           .FirstOrDefault();

                            int diferenciaTotal = 0;

                            foreach (var item in model.detalleSeccion)
                            {
                                if (item.horaHasta <= item.horaDesde)
                                {
                                    return new OperationResult(false, "No se puede crear una hora de finalizacion mayor que la de inicio");
                                }
                                if (!seccionAsignaturaRepo.ValidarChoquesdeHora(model.idMaestro, item.idDia, item.horaDesde, item.horaHasta, model.idSeccion))
                                {
                                    return new OperationResult(false, $"El maestro no tiene disponibilidad para el dia {(DiasSemanaEnum)item.idDia} en la hora proporcionada");
                                }
                                if (!seccionAsignaturaRepo.ValidarChoquesdeHoraAula(item.idAula, item.idDia, item.horaDesde, item.horaHasta, model.idSeccion))
                                {
                                    return new OperationResult(false, $"El aula no tiene disponibilidad para el dia {(DiasSemanaEnum)item.idDia} en la hora proporcionada");
                                }
                                int diferencia = (int)(item.horaHasta - item.horaDesde).TotalHours;
                                diferenciaTotal += diferencia;

                                if (diferenciaTotal > infoAsignatura.creditos)
                                {
                                    return new OperationResult(false, $"No puedes agregar mas horas que la cantidad de creditos que tiene la asignatura {infoAsignatura.nombreAsignatura}");
                                }
                                if (diferencia <= infoAsignatura.creditos - 2)
                                {
                                    return new OperationResult(false, $"No puedes agregar menos horas que la cantidad de creditos que tiene la asignatura {infoAsignatura.nombreAsignatura}");
                                }

                            }
                            seccionAsignaturaRepo.Edit(model, idSeccion);
                            seccionAsignaturaRepo.SaveChanges();
                            return new OperationResult(true, "Se ha creado la seccion", model);


                        }

                        else
                        {
                            return new OperationResult(false, "No se ha proporcionado información del detalle de la seccion");
                        }

                    }


                }
                catch(Exception ex)
                {
                    seccionAsignaturaRepo.LogError(ex);

                    return new OperationResult(false, "Error en la inserción de datos");
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
