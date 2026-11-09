using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Parcial2HPACapas.Datos;
using Parcial2HPACapas.Entidades;
using Parcial2HPACapas.Negocio.Contratos;

namespace Parcial2HPACapas.Negocio
{
    /// <summary>
    /// Logica de negocio de autenticacion. Coordina el repositorio de usuarios
    /// (Datos) con los contratos de hash (Negocio). Contiene las validaciones de
    /// negocio del inicio de sesion y del registro.
    /// </summary>
    public class AutenticacionService
    {
        /// <summary>
        /// Excepcion de inicio de sesion fallido. Solo transporta el mensaje
        /// para la Vista; el registro en el historial lo hace el servicio.
        /// </summary>
        public class LoginFailure : Exception
        {
            public LoginFailure(string mensaje) : base(mensaje) { }
        }

        private readonly UsuarioRepositorio _usuarioRepositorio;
        private readonly IHashService _hashService;
        private readonly IVerificarHashService _verificadorService;

        /// <summary>
        /// Crea el servicio inyectando sus dependencias (inversion de control).
        /// </summary>
        public AutenticacionService(
            UsuarioRepositorio usuarioRepositorio,
            IHashService hashService,
            IVerificarHashService verificadorService)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _hashService = hashService;
            _verificadorService = verificadorService;
        }

        /// <summary>
        /// Valida las credenciales e inicia sesion. Lanza una excepcion con un
        /// mensaje claro si las validaciones o la autenticacion fallan.
        /// </summary>
        /// <returns>El <see cref="Usuario"/> autenticado.</returns>
        public Usuario IniciarSesion(string correo, string password)
        {
            // Validaciones de entrada
            if (string.IsNullOrWhiteSpace(correo))
                throw new LoginFailure("Debe ingresar el correo electronico.");
            if (string.IsNullOrWhiteSpace(password))
                throw new LoginFailure("Debe ingresar la contrasena.");

            correo = correo.Trim();
            Usuario usuario = _usuarioRepositorio.ObtenerPorCorreo(correo);

            // Mensaje generico para no revelar si el correo existe o no
            if (usuario == null)
                throw Fallo(null, correo, "Correo inexistente", "Correo o contrasena incorrectos.");

            if (!usuario.Activo)
                throw Fallo(usuario.Id, correo, "Cuenta inactiva", "La cuenta se encuentra inactiva.");

            // Usa el CONTRATO de verificacion (no el de generacion)
            if (!_verificadorService.Verificar(password, usuario.PasswordHash, usuario.Salt))
                throw Fallo(usuario.Id, correo, "Contrasena incorrecta", "Correo o contrasena incorrectos.");

            RegistrarIntento(usuario.Id, correo, true, null);
            return usuario;
        }

        /// <summary>
        /// Registra el intento fallido en el historial y devuelve la
        /// <see cref="LoginFailure"/> lista para lanzar.
        /// </summary>
        private LoginFailure Fallo(int? usuarioId, string correo, string motivo, string mensajeUsuario)
        {
            RegistrarIntento(usuarioId, correo, false, motivo);
            return new LoginFailure(mensajeUsuario);
        }

        /// <summary>
        /// Guarda el intento en <c>login_historial</c>. Si la auditoria falla no
        /// se interrumpe el login: el error de registro no debe tapar el resultado.
        /// </summary>
        private void RegistrarIntento(int? usuarioId, string correo, bool exito, string motivo)
        {
            try
            {
                _usuarioRepositorio.RegistrarLoginHistorial(usuarioId, correo, ObtenerIpLocal(), exito, motivo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("No se pudo registrar el intento de login: " + ex.Message);
            }
        }

        /// <summary>Devuelve la IPv4 local del equipo (o 127.0.0.1 si no hay).</summary>
        private static string ObtenerIpLocal()
        {
            try
            {
                IPAddress ip = Dns.GetHostAddresses(Dns.GetHostName())
                    .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                return ip?.ToString() ?? "127.0.0.1";
            }
            catch (SocketException)
            {
                return "127.0.0.1";
            }
        }

        /// <summary>
        /// Registra un usuario nuevo. Usa el CONTRATO de generacion de hash para
        /// no guardar nunca la contrasena en texto plano.
        /// </summary>
        public void RegistrarUsuario(string nombre, string correo, string password, string rol)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("Debe ingresar el nombre.");
            if (string.IsNullOrWhiteSpace(correo))
                throw new ArgumentException("Debe ingresar el correo.");
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                throw new ArgumentException("La contrasena debe tener al menos 8 caracteres.");

            if (_usuarioRepositorio.ObtenerPorCorreo(correo.Trim()) != null)
                throw new InvalidOperationException("Ya existe un usuario con ese correo.");

            // Usa el CONTRATO de generacion (no el de verificacion)
            ResultadoHash resultado = _hashService.GenerarHash(password);

            var usuario = new Usuario
            {
                Nombre = nombre.Trim(),
                Correo = correo.Trim(),
                PasswordHash = resultado.Hash,
                Salt = resultado.Salt,
                Rol = string.IsNullOrWhiteSpace(rol) ? "Usuario" : rol.Trim(),
                Activo = true
            };

            _usuarioRepositorio.Insertar(usuario);
        }
    }
}
