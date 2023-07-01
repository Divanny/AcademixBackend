using Data.Administration;
using Data.Common;
using Models.Administration;
using Models.Common;
using Models.Enums;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using WebAPI.Infraestructure;
using System.Web.Http;
using Data.Entities;

namespace WebAPI.Controllers
{
    /// <summary>
    /// API para manejar los usuarios del sistema.
    /// </summary>
    [RoutePrefix("api/Usuarios")]
    public class UsuariosController : ApiBaseController
    {
        public AcadmixEntities academixEntities { get; set; }
        public UsuariosRepo usuariosRepo { get; set; }
        public PerfilesRepo perfilesRepo { get; set; }
        public MaestrosRepo maestrosRepo { get; set; }
        public EstudiantesRepo estudiantesRepo { get; set; }

        Utilities utilities = new Utilities();
        public UsuariosController()
        {
            academixEntities = new AcadmixEntities();
            usuariosRepo = new UsuariosRepo(academixEntities);
            perfilesRepo = new PerfilesRepo(academixEntities);
            maestrosRepo = new MaestrosRepo(academixEntities);
            estudiantesRepo = new EstudiantesRepo(academixEntities);
        }

        /// <summary>
        /// Obtiene un listado de los usuarios registrados.
        /// </summary>
        /// <returns></returns>
        // GET api/Usuarios
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public List<UsuariosModel> Get()
        {
            return usuariosRepo.Get().ToList();
        }

        /// <summary>
        /// Obtiene un usuario en específico.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/Usuarios/5
        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        public UsuariosModel Get(int id)
        {
            return usuariosRepo.Get(x => x.idUsuario == id).FirstOrDefault();
        }

        /// <summary>
        /// Agrega un nuevo usuario (se necesita permiso de administrador).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Usuarios
        [HttpPost]
        [Autorizar(VistasEnum.GestionarUsuarios)]
        public OperationResult Post([FromBody] UsuariosModel model)
        {
            if (ValidateModel(model))
            {
                using (var trx = academixEntities.Database.BeginTransaction())
                {
                    try
                    {
                        UsuariosModel usuario = usuariosRepo.GetByUsername(model.NombreUsuario);
                        if (usuario != null)
                        {
                            Validation.Errors.Add(nameof(model.NombreUsuario), "Este usuario ya está registrado");
                            return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
                        }

                        if (model.Password == null || model.Password == "")
                        {
                            Validation.Errors.Add(nameof(model.Password), "Se debe colocar una contraseña válida");
                            return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
                        }


                        model.PasswordEncrypted = Cipher.Encrypt(model.Password, Properties.Settings.Default.JwtSecret);

                        if (model.idEstado == 0)
                        {
                            model.idEstado = (int)EstadoUsuarioEnum.Activo;
                        }

                        model.FechaRegistro = DateTime.Now;
                        model.UltimoIngreso = DateTime.Now;

                        if (model.idPerfil == 0)
                        {
                            var DefaultProfile = perfilesRepo.GetDefaultProfile();
                            model.idPerfil = DefaultProfile.idPerfil;
                        }

                        var created = usuariosRepo.Add(model);

                        if (model.idPerfil == (int)PerfilesEnum.Estudiante)
                        {
                            if (!ValidateModel(model.InfoEstudiante))
                            {
                                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
                            }
                            if (model.InfoEstudiante != null)
                            {
                                model.InfoEstudiante.idUsuario = created.idUsuario;
                                estudiantesRepo.Add(model.InfoEstudiante);
                            }
                            else
                            {
                                return new OperationResult(false, "No se ha proporcionado información del estudiante");
                            }
                        }
                        else if (model.idPerfil == (int)PerfilesEnum.Maestro)
                        {
                            if (!ValidateModel(model.InfoMaestro))
                            {
                                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
                            }
                            if (model.InfoMaestro != null)
                            {
                                model.InfoMaestro.idUsuario = created.idUsuario;
                                maestrosRepo.Add(model.InfoMaestro);
                            }
                            else
                            {
                                return new OperationResult(false, "No se ha proporcionado información del maestro");
                            }
                        }

                        trx.Commit();
                        return new OperationResult(true, "Se creado este usuario satisfactoriamente", created);

                    }
                    catch (Exception ex)
                    {
                        usuariosRepo.LogError(ex);

                        trx.Rollback();
                        return new OperationResult(false, "Error en la inserción de datos");
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
        /// <param name="idUsuario"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/Usuarios/5
        [HttpPut]
        [Autorizar(VistasEnum.GestionarUsuarios)]
        public OperationResult Put(int idUsuario, [FromBody] UsuariosModel model)
        {
            if (ValidateModel(model))
            {
                using (var trx = academixEntities.Database.BeginTransaction())
                {
                    try
                    {
                        UsuariosModel usuario = usuariosRepo.Get(x => x.idUsuario == idUsuario).FirstOrDefault();

                        if (usuario == null)
                        {
                            return new OperationResult(false, "Este usuario no existe.");
                        }

                        var usuarioExists = usuariosRepo.Get(x => x.NombreUsuario == model.NombreUsuario).FirstOrDefault();

                        if (usuarioExists != null)
                        {
                            if (usuarioExists.idUsuario != idUsuario)
                            {
                                return new OperationResult(false, "Este usuario ya está registrado");
                            }
                        }

                        if (model.Password == null)
                        {
                            model.PasswordEncrypted = usuario.PasswordEncrypted;
                        }
                        else
                        {
                            model.PasswordEncrypted = Cipher.Encrypt(model.Password, Properties.Settings.Default.JwtSecret);
                        }

                        model.UltimoIngreso = DateTime.Now;

                        if (model.idPerfil == (int)PerfilesEnum.Estudiante)
                        {
                            if (model.InfoEstudiante != null)
                            {
                                if (!ValidateModel(model.InfoEstudiante))
                                {
                                    return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
                                }
                            
                                var estudiante = estudiantesRepo.Get(x => x.idUsuario == model.idUsuario).FirstOrDefault();
                            
                                if (estudiante != null)
                                {
                                    estudiantesRepo.Edit(model.InfoEstudiante, model.InfoEstudiante.idEstudiante);
                                }
                                else
                                {
                                    model.InfoEstudiante.idUsuario = usuario.idUsuario;
                                    estudiantesRepo.Add(model.InfoEstudiante);
                                }
                            } 
                            else
                            {
                                return new OperationResult(false, "Debe especificar la información del estudiante");
                            }
                        }
                        else if (model.idPerfil == (int)PerfilesEnum.Maestro)
                        {
                            if (model.InfoMaestro != null)
                            {
                                if (!ValidateModel(model.InfoMaestro))
                                {
                                    return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
                                }

                                var maestro = maestrosRepo.Get(x => x.idUsuario == model.idUsuario).FirstOrDefault();

                                if (maestro != null)
                                {
                                    maestrosRepo.Edit(model.InfoMaestro, model.InfoMaestro.idMaestro);
                                }
                                else
                                {
                                    model.InfoMaestro.idUsuario = usuario.idUsuario;
                                    maestrosRepo.Add(model.InfoMaestro);
                                }
                            }
                            else
                            {
                                return new OperationResult(false, "Debe especificar la información del estudiante");
                            }
                        }

                        usuariosRepo.Edit(model, idUsuario);
                        trx.Commit();
                        return new OperationResult(true, "Se ha actualizado satisfactoriamente");
                    }
                    catch (Exception ex)
                    {
                        usuariosRepo.LogError(ex);
                        trx.Rollback();
                        return new OperationResult(false, "Error en la inserción de datos");
                    }
                }
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        /// <summary>
        /// Cambia la contraseña de un usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Route("CambiarContraseña")]
        [HttpPut]
        [Autorizar(AllowAnyProfile = true)]
        public OperationResult CambiarContraseña(int idUsuario, string password)
        {
            UsuariosModel usuario = usuariosRepo.Get(x => x.idUsuario == idUsuario).FirstOrDefault();

            if (usuario == null)
            {
                return new OperationResult(false, "Este usuario no existe.");
            }

            var PasswordValidation = utilities.ValidarContraseña(password);

            if (!PasswordValidation.Success)
            {
                return PasswordValidation;
            }

            usuario.PasswordEncrypted = Cipher.Encrypt(password, Properties.Settings.Default.JwtSecret);
            usuariosRepo.Edit(usuario, idUsuario);
            return new OperationResult(true, "La contraseña se ha actualizado satisfactoriamente");


            return new OperationResult(false, "Los datos ingresados no son válidos");
        }

        /// <summary>
        /// Obtiene los diferentes estados que puede estar un usuario.
        /// </summary>
        /// <returns></returns>
        // GET api/Usuarios/GetEstadosUsuarios
        [Route("GetEstadosUsuarios")]
        [HttpGet]
        [Autorizar(AllowAnyProfile = true)]
        public object GetEstadosUsuarios()
        {
            return usuariosRepo.GetEstadosUsuarios();
        }
    }
}