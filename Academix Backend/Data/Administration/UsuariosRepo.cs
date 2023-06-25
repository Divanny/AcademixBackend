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
            (DB, filter) => (from u in DB.Set<Usuarios>().Where(filter)
                             join p in DB.Set<Perfiles>() on u.idPerfil equals p.idPerfil
                             join e in DB.Set<EstadosUsuarios>() on u.idEstado equals e.idEstado
                             //join s in DB.Set<Estudiante>() on u.idUsuario equals s.idUsuario
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
                                 Telefono = u.Telefono,
                                 //InfoEstudiante = Estudiante(x => x.idUsuario == u.idUsuario)
                             })
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
    }
}
