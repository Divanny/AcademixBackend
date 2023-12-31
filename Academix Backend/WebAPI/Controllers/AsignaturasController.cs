﻿using Data.Administration;
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
    [RoutePrefix("api/Asignaturas")]
    public class AsignaturasController : ApiBaseController
    {
        AsignaturasRepo asignaturasRepo = new AsignaturasRepo();

        /// <summary>
        /// Obtiene un listado de las asignaturas registradas.
        /// </summary>
        /// <returns></returns>
        // GET api/Asignaturas
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public List<AsignaturasModel> Get()
        {
            return asignaturasRepo.Get().ToList();
        }

        /// <summary>
        /// Obtiene una asignatura en específico.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/Asignaturas/5
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public AsignaturasModel Get(int id)
        {
            return asignaturasRepo.Get(x => x.idAsignatura == id).FirstOrDefault();
        }

        /// <summary>
        /// Agrega una nueva asignatura (se necesita permiso de administrador).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Asignaturas
        [HttpPost]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Post([FromBody] AsignaturasModel model)
        {
            if (ValidateModel(model))
            {
                try
                {

                    AsignaturasModel asignatura = asignaturasRepo.GetByName(model.NombreAsignatura);
                    if (asignatura != null)
                    {
                        return new OperationResult(false, "Esta asignatura ya está registrado");
                    }
                    asignatura = asignaturasRepo.GetByCode(model.CodigoAsignatura);
                    if (asignatura != null)
                    {
                        return new OperationResult(false, "Este codigo de asignatura ya está registrado");
                    }

                    if (model.Dependencias != null)
                    {
                        foreach (var item in model.Dependencias)
                        {
                            if (item.idAsignatura == item.idPrerrequisito)
                            {
                                return new OperationResult(false, "No se puede tener de prerequisito la misma asignatura");
                            } 

                        }


                    }
                    var created = asignaturasRepo.Add(model);
                    asignaturasRepo.SaveChanges();
                    return new OperationResult(true, "Se ha creado esta asignatura satisfactoriamente", created);
                }
                catch (Exception ex)
                {

                    asignaturasRepo.LogError(ex);

                    return new OperationResult(false, "Error en la inserción de datos");
                }

            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        /// <summary>
        /// Actualiza la información de una asignatura.
        /// </summary>
        /// <param name="idAsignatura"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/Asignaturas/5
        [HttpPut]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Put(int idAsignatura, [FromBody] AsignaturasModel model)
        {
            if (ValidateModel(model))
            {
                try
                {

                AsignaturasModel asignaturas = asignaturasRepo.Get(x => x.idAsignatura == idAsignatura).FirstOrDefault();

                if (asignaturas == null)
                {
                    return new OperationResult(false, "Esta asignatura no existe.");
                }

                var asignaturaExists = asignaturasRepo.Get(x => x.nombreAsignatura == model.NombreAsignatura).FirstOrDefault();

                if (asignaturaExists != null)
                {
                    if (asignaturaExists.idAsignatura != idAsignatura)
                    {
                        return new OperationResult(false, "Esta asignatura ya esta registrada");
                    }
                }
                    if (model.Dependencias != null)
                    {
                        foreach (var item in model.Dependencias)
                        {
                            if (item.idAsignatura == item.idPrerrequisito)
                            {
                                return new OperationResult(false, "No se puede tener de prerequisito la misma asignatura");
                            }

                        }


                    }



                    asignaturasRepo.Edit(model, idAsignatura);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente");
                }
                catch (Exception ex)
                {

                    asignaturasRepo.LogError(ex);

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
