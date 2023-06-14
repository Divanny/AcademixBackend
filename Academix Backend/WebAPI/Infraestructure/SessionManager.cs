using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace WebAPI.Infraestructure
{
    public class UserSesionInfo
    {
        public EstadoSesion Estado { get; set; }
        public string idUsuario { get; set; }
        public string idPerfil { get; set; }
    }
    public enum EstadoSesion
    {
        NoIniciada = 0,
        Iniciada = 1,
        NoAutorizado = 2
    }
    /// <summary>
    /// Maneja los datos del usuario en sesion POR MEDIO DE TOKEN
    /// </summary>
    /// 
    public static class SessionData
    {
        private static int Duration = Properties.Settings.Default.SessionDuration;
        private static string Key = Properties.Settings.Default.JwtSecret;

        public static string Set(UserSesionInfo info)
        {
            var key = Encoding.ASCII.GetBytes(Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("idUsuario", info.idUsuario),
                    new Claim("idPerfil", info.idPerfil),
                }),
                Expires = DateTime.UtcNow.AddMinutes(Duration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenKey = tokenHandler.WriteToken(token);
            HttpContext.Current.Response.Headers.Add("Authorization", "Bearer " + tokenKey);
            return "Bearer " + tokenKey;
        }
        public static UserSesionInfo Get()
        {
            UserSesionInfo info = new UserSesionInfo();

            var authHeader = HttpContext.Current.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader))
            {
                info.Estado = EstadoSesion.NoIniciada;
                return info;
            }

            try
            {
                if (!authHeader.StartsWith("Bearer "))
                {
                    info.Estado = EstadoSesion.NoIniciada;
                    return info;
                }
                var token = authHeader.Substring("Bearer ".Length).Trim();

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(Key);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken validatedToken;
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                info.Estado = EstadoSesion.Iniciada;
                info.idUsuario = claimsPrincipal.FindFirst("idUsuario").Value;
                info.idPerfil = claimsPrincipal.FindFirst("idPerfil").Value;
            }
            catch (Exception ex)
            {
                info.Estado = EstadoSesion.NoIniciada;
                return info;
            }

            return info;
        }

        public static void Clear()
        {
            HttpContext.Current.Items.Remove("AuthToken");
        }
    }
}