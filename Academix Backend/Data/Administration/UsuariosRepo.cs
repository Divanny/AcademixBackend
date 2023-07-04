using Data.Common;
using Data.Entities;
using Models.Administration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Data.Administration
{
    public class UsuariosRepo : Repository<Usuarios, UsuariosModel>
    {
        public UsuariosRepo(DbContext dbContext = null) : base
        (
            dbContext ?? new AcadmixEntities(),
            new ObjectsMapper<UsuariosModel, Usuarios>(u => new Usuarios()
            {
                idUsuario = u.idUsuario,
                Nombres = u.Nombres,
                Apellidos = u.Apellidos,
                NombreUsuario = u.NombreUsuario,
                CorreoElectronico = u.CorreoElectronico,
                Password = u.PasswordEncrypted,
                idPerfil = u.idPerfil,
                idEstado = u.idEstado,
                FechaRegistro = u.FechaRegistro,
                UltimoIngreso = u.UltimoIngreso,
                Telefono = u.Telefono
            }),
            (DB, filter) =>
            {
                EstudiantesRepo estudiantesRepo = new EstudiantesRepo(dbContext);
                MaestrosRepo maestrosRepo = new MaestrosRepo(dbContext);

                return (from u in DB.Set<Usuarios>().Where(filter)
                        join p in DB.Set<Perfiles>() on u.idPerfil equals p.idPerfil
                        join e in DB.Set<EstadosUsuarios>() on u.idEstado equals e.idEstado
                        let infoEstudiante = estudiantesRepo.Get(x => x.idUsuario == u.idUsuario).FirstOrDefault()
                        let infoMaestro = maestrosRepo.Get(x => x.idUsuario == u.idUsuario).FirstOrDefault()
                        select new UsuariosModel()
                        {
                            idUsuario = u.idUsuario,
                            Nombres = u.Nombres,
                            Apellidos = u.Apellidos,
                            NombreUsuario = u.NombreUsuario,
                            CorreoElectronico = u.CorreoElectronico,
                            PasswordEncrypted = u.Password,
                            idPerfil = u.idPerfil,
                            Perfil = p.Nombre,
                            idEstado = u.idEstado,
                            Estado = e.Nombre,
                            FechaRegistro = u.FechaRegistro,
                            UltimoIngreso = u.UltimoIngreso,
                            InfoEstudiante = infoEstudiante,
                            InfoMaestro = infoMaestro,
                            IdentificacionEstudiante = generateIdentificacion(u, infoEstudiante),
                            Telefono = u.Telefono
                        });
            }
        )
        { }

        public UsuariosModel GetByUsername(string nombreUsuario)
        {
            return this.Get(x => x.NombreUsuario == nombreUsuario).FirstOrDefault();
        }

        public UsuariosModel Get(int id)
        {
            var result = base.Get(a => a.idUsuario == id).FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            return null;
        }

        public List<EstadosUsuarios> GetEstadosUsuarios()
        {
            return dbContext.Set<EstadosUsuarios>().ToList();
        }

        private static string generateIdentificacion(Usuarios infoUsuario, EstudiantesModel infoEstudiante)
        {
            if (infoEstudiante == null)
            {
                return "";
            }
            string identificacion = "110";
            identificacion += infoUsuario.idUsuario + infoUsuario.FechaRegistro.Day.ToString() + infoEstudiante.idEstudiante;
            return identificacion;
        }

    }
}
