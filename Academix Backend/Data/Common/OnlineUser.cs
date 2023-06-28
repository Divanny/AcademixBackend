using System;
using System.Web;

namespace Data.Common
{
    public class OnlineUser
    {
        /// <summary>
        /// Obtiene el ID del usuario en línea
        /// </summary>
        /// <returns></returns>
        public static int GetUserId()
        {
            var userID = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
            return userID;
        }
    }
}
