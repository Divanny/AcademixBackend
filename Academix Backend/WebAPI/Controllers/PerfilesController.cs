using Data.Administration;
using Models.Administration;
using Models.Common;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using WebAPI.Infraestructure;

namespace WebAPI.Controllers
{
    /// <summary>
    /// API para manejar los perfiles o roles del sistema.
    /// </summary>
    [RoutePrefix("api/Perfiles")]
    public class PerfilesController : ApiBaseController
    {
        PerfilesRepo perfilesRepo = new PerfilesRepo();

        /// <summary>
        /// Obtiene un listado de todos los perfiles del sistema.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Autorizar(VistasEnum.GestionarPerfiles)]
        public List<PerfilesModel> Get()
        {
            List<PerfilesModel> perfiles = perfilesRepo.Get().ToList();
            foreach (var item in perfiles)
            {
                item.Usuarios = GetUsuarios(item.idPerfil);
                item.Vistas = GetPermisos(item.idPerfil);
            }
            return perfiles;

        }

        /// <summary>
        /// Crea un nuevo perfil al sistema.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Autorizar(VistasEnum.GestionarPerfiles)]
        public OperationResult Post(PerfilesModel model)
        {
            if (ValidateModel(model))
            {
                try
                {
                var ifExist = perfilesRepo.Get(x => x.Nombre == model.Nombre).FirstOrDefault();

                if (ifExist != null)
                {
                    return new OperationResult(false, "Este perfil ya existe");
                }

                var created = perfilesRepo.Add(model);
                perfilesRepo.Log(created);
                return new OperationResult(true, "Se ha creado satisfactoriamente", created);

                }
                catch (Exception ex)
                {

                    perfilesRepo.LogError(ex);

                    return new OperationResult(false, "Error en la inserción de datos");
                }

            }
            else
                return new OperationResult(false, "Los datos suministrados no son válidos", Validation.Errors);
        }

        /// <summary>
        /// Actualiza la información de un perfil.
        /// </summary>
        /// <param name="idPerfil"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Autorizar(VistasEnum.GestionarPerfiles)]
        public OperationResult Put(int idPerfil, PerfilesModel model)
        {
            if (ValidateModel(model))
            {
                try
                {
                perfilesRepo.Edit(model);
                perfilesRepo.Log(model);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente", model);

                }
                catch (Exception ex)
                {

                    perfilesRepo.LogError(ex);

                    return new OperationResult(false, "Error en la inserción de datos");
                }

            }
            else
                return new OperationResult(false, "Los datos suministrados no son válidos", Validation.Errors);
        }

        /// <summary>
        /// Elimina un perfil.
        /// </summary>
        /// <param name="idPerfil"></param>
        /// <returns></returns>
        [HttpDelete]
        [Autorizar(VistasEnum.GestionarPerfiles)]
        public OperationResult Delete(int idPerfil)
        {
            try
            {

                try
                {

                perfilesRepo.Delete(idPerfil);
                perfilesRepo.Log(idPerfil);
                return new OperationResult(true, "Se ha eliminado satisfactoriamente");
                }
                catch (Exception ex)
                {

                    perfilesRepo.LogError(ex);

                    return new OperationResult(false, "Error en la inserción de datos");
                }

            }
            catch (Exception ex)
            {
                return new OperationResult(false, "No se ha podido eliminar este perfil");
            }
        }

        /// <summary>
        /// Obtiene un listado de usuarios de un perfil.
        /// </summary>
        /// <param name="idPerfil"></param>
        /// <returns></returns>
        //[Autorizar(VistasEnum.GestionarPerfiles)]
        [HttpGet]
        [Route("GetUsuarios")]
        [Autorizar(VistasEnum.GestionarPerfiles)]
        public List<UsuariosModel> GetUsuarios(int idPerfil)
        {
            return perfilesRepo.GetUsuarios(idPerfil).ToList();
        }


        /// <summary>
        /// Obtiene un listado de permisos de un perfil.
        /// </summary>
        /// <param name="idPerfil"></param>
        /// <returns></returns>
        //[Autorizar(VistasEnum.GestionarPerfiles)]
        [HttpGet]
        [Route("GetPermisos")]
        [Autorizar(VistasEnum.GestionarPerfiles)]
        public List<VistasModel> GetPermisos(int idPerfil)
        {
            return perfilesRepo.GetPermisos(idPerfil).ToList();
        }

        /// <summary>
        /// Obtiene un listado de todas las vistas del sistema.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetVistas")]
        [Autorizar(VistasEnum.GestionarPerfiles)]
        public List<VistasModel> GetVistas()
        {
            return perfilesRepo.GetVistas().ToList();
        }
    }
}