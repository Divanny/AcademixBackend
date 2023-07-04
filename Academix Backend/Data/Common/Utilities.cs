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

        public string GenerateRandomPassword(int length)
        {
            string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            Random random = new Random();

            string password = new string(Enumerable.Repeat(validChars, length)
                                                  .Select(s => s[random.Next(s.Length)])
                                                  .ToArray());

            return password;
        }

         public string getCalificacionLiteral(int calificacion)
         {
            string letra;
            if (calificacion >= 90)
            {
                letra = "A";
            }
            else if (calificacion >= 85)
            {
                letra = "B+";
            }
            else if (calificacion >= 80)
            {
                letra = "B";
            }
            else if (calificacion >= 75)
            {
                letra = "C+";
            }
            else if (calificacion >= 70)
            {
                letra = "C";
            }
            else if (calificacion >= 60)
            {
                letra = "D";
            }
            else
            {
                letra = "F";
            }
            return letra;
         }

        public double ValorDelLiteral(string literal)
        {
            double valor = 0;

            switch (literal)
            {
                case "A":
                    valor = 4.0;
                    break;
                case "B+":
                    valor = 3.5;
                    break;
                case "B":
                    valor = 3.0;
                    break;
                case "C+":
                    valor = 2.5;
                    break;
                case "C":
                    valor = 2.0;
                    break;
                case "D":
                    valor = 1.0;
                    break;
                case "F":
                    valor = 0.0;
                    break;
                default:
                    // Valor por defecto si el literal no coincide con ningún caso
                    break;
            }

            return valor;
        }

    }
}
