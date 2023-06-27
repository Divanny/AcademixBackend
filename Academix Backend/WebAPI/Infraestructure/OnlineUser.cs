using Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Infraestructure
{
    public class OnlineUser
    {
        /// <summary>
        /// Obtiene el ID del usuario en línea
        /// </summary>
        /// <returns></returns>
        public static int GetUserId()
        {
            var userID = Convert.ToInt32(SessionData.Get().idUsuario);
            return userID;
        }
    }
}