using Data.Common;
using Data.Entities;
using Models.Administration;
using Models.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Administration
{
    public class PublicacionRepo:Repository<Publicacion,PublicacionModel>
    {
        public PublicacionRepo(DbContext dbContext = null) : base
          (
              dbContext ?? new AcadmixEntities(),
              new ObjectsMapper<PublicacionModel, Publicacion>(u => new Publicacion()
              {
                  idPublicacion = u.idPublicacion,
                  idListadoEstudiante = u.idListadoEstudiante,
                  idCalificacion = u.idCalificacion,
                  fechaPublicacion = u.fechaPublicacion,

              }),
              (DB, filter) => (from u in DB.Set<Publicacion>().Where(filter)
                               join m in DB.Set<Listado_Estudiantes>() on u.idListadoEstudiante equals m.idListadoEstudiante
                               join e in DB.Set<Estudiante>() on m.idEstudiante equals e.idEstudiante
                               join z in DB.Set<Usuarios>() on e.idUsuario equals z.idUsuario
                               join s in DB.Set<Seccion_Asignatura>() on m.idSeccion equals s.idSeccion
                               join a in DB.Set<Asignatura>() on s.idAsignatura equals a.idAsignatura
                           
                               select new PublicacionModel()
                               {
                                   idPublicacion = u.idPublicacion,
                                   idListadoEstudiante = u.idListadoEstudiante,
                                   nombreAsignatura = a.nombreAsignatura,
                                   codigoSeccion = s.codigoSeccion,
                                   nombreEstudiante = z.Nombres,
                                   idCalificacion = u.idCalificacion,
                                   literal = getLiteral(u.idCalificacion),
                                   fechaPublicacion = u.fechaPublicacion,
                               })
          )
        { }
        static public string getLiteral(int calificacion)
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

        public OperationResult PostPublicacionListado(List<ListadoEstudiantesModel> listados)
        {
            var publicacionSet = dbContext.Set<Publicacion>();

            if (listados != null)
            {
                foreach(var listado in listados)
                {
                    publicacionSet.Remove(publicacionSet.Where(x => x.idListadoEstudiante == listado.idListadoEstudiante).FirstOrDefault());

                    publicacionSet.Add(new Publicacion()
                    {
                        idListadoEstudiante = listado.idListadoEstudiante,
                        fechaPublicacion = DateTime.Now,
                        idCalificacion = listado.calificacion
                    });
                }
                
                SaveChanges();
                return new OperationResult(true, "Se han guardado las publicaciones satisfactoriamente");
            }
            else
            {
                return new OperationResult(false, "No se han enviado las publicaciones");
            }
        }



    }
}
