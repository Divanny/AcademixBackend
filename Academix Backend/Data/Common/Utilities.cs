using Data.Entities;
using Models.Common;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Data.Common
{
    public class Utilities
    {
        public OperationResult ValidarContraseña(string contraseña)
        {
            // Verificar que la contraseña tiene al menos 8 caracteres
            if (contraseña.Length < 8)
            {
                return new OperationResult(false, "La contraseña no puede ser menor a 8 carácteres");
            }

            // Verificar que la contraseña contiene al menos una letra mayúscula
            if (!contraseña.Any(char.IsUpper))
            {
                return new OperationResult(false, "La contraseña debe contener al menos una letra mayúscula");
            }

            // Verificar que la contraseña contiene al menos una letra minúscula
            if (!contraseña.Any(char.IsLower))
            {
                return new OperationResult(false, "La contraseña debe contener al menos una letra minúscula");
            }

            // Verificar que la contraseña contiene al menos un número
            if (!contraseña.Any(char.IsDigit))
            {
                return new OperationResult(false, "La contraseña debe contener al menos un número");
            }

            // Verificar que la contraseña contiene al menos un caracter especial
            if (!contraseña.Any(c => !char.IsLetterOrDigit(c)))
            {
                return new OperationResult(false, "La contraseña debe contener al menos un carácter especial");
            }

            // Si todos los criterios son satisfechos, entonces la contraseña es segura
            return new OperationResult(true, "La contraseña es segura");
        }
        //public int GetActualPeriodo()
        //{
        //    AcadmixEntities acadmixEntities = new AcadmixEntities();
        //    var periodo = acadmixEntities.Periodo.Where(x => x.incioPeriodo >= DateTime.Now && x.finPeriodo <= DateTime.Now).FirstOrDefault();
        //    return periodo.idPeriodo;


        //}

        public int ObtenerTrimestreActual()
        {
            DateTime fechaActual = DateTime.Now;
            int mes = fechaActual.Month;

            if (mes >= 2 && mes <= 4)
            {
                return (int)PeriodosEnum.febero_abril;
            }
            else if (mes >= 5 && mes <= 7)
            {
                return (int)PeriodosEnum.mayo_julio;
            }
            else if (mes >= 8 && mes <= 10)
            {
                return (int)PeriodosEnum.agosto_octubre;
            }
            else
            {
                return (int)PeriodosEnum.noviembre_enero;
            }
        }
        



    }
}
