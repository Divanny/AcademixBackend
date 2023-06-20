using Models.Common;
using Models.Enums;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Data.Entities;
using Data.Administration;

namespace WebAPI.Infraestructure
{
    class Authentication
    {
        public LogInResult LogIn(Credentials credentials)
        {
            LogInResult logInResult;
            using (var dbc = new AcadmixEntities())
            {
                UsuariosRepo ur = new UsuariosRepo(dbc);
                var usuario = ur.GetFirst(x => (x.NombreUsuario.Trim()).ToLower() == ((credentials.userName).Trim()).ToLower());

                if (usuario == null) return new LogInResult(false, "Usuario o contraseña invalidos");

                var passwordEncrypted = Cipher.Encrypt(credentials.password, Properties.Settings.Default.JwtSecret);

                if (passwordEncrypted != usuario.PasswordEncrypted) return new LogInResult(false, "La contraseña ingresada es incorrecta");

                if (usuario.idEstado == (int)EstadoUsuarioEnum.Inactivo) return new LogInResult(false, "El usuario " + credentials.userName + " está inactivo");

                var token = SessionData.Set(new UserSesionInfo() { idUsuario = usuario.idUsuario.ToString(), idPerfil = usuario.idPerfil.ToString() });

                usuario.UltimoIngreso = DateTime.Now;
                ur.Edit(usuario, usuario.idUsuario);

                logInResult = new LogInResult(true, "Exito al iniciar sesión", true, usuario.idUsuario, token);
            }
            return logInResult;
        }
    }
}