using System;
using Parcial2HPACapas.Entidades;
using Parcial2HPACapas.Negocio.Contratos;

namespace Parcial2HPACapas.Negocio
{
    /// <summary>
    /// Implementacion concreta del hashing de contrasenas usando BCrypt
    /// (libreria BCrypt.Net-Next). Una sola clase implementa los DOS contratos
    /// independientes (<see cref="IHashService"/> e <see cref="IVerificarHashService"/>).
    ///
    /// Reemplaza a la version anterior con SHA-256: gracias al desacoplamiento,
    /// el cambio no toca la Vista, el Presentador ni los contratos.
    ///
    /// Por que BCrypt:
    ///  - Es lento a proposito (factor de trabajo), lo que encarece los ataques
    ///    de fuerza bruta; SHA-256 esta pensado para ser rapido.
    ///  - Genera y guarda el salt DENTRO del propio hash
    ///    (formato "$2a$11$" + salt de 22 caracteres + hash de 31 caracteres),
    ///    por eso BCrypt.Verify() solo necesita la contrasena y el hash.
    /// </summary>
    public class BCryptHashService : IHashService, IVerificarHashService
    {
        /// <summary>
        /// Factor de trabajo (2^11 iteraciones). Cada +1 duplica el costo.
        /// </summary>
        private const int FactorTrabajo = 11;

        /// <inheritdoc/>
        public ResultadoHash GenerarHash(string textoPlano)
        {
            if (textoPlano == null)
                throw new ArgumentNullException(nameof(textoPlano));

            // Salt aleatorio generado por BCrypt (ya incluye version y factor de trabajo).
            string salt = BCrypt.Net.BCrypt.GenerateSalt(FactorTrabajo);
            string hash = BCrypt.Net.BCrypt.HashPassword(textoPlano, salt);

            // El salt tambien queda dentro del hash; se guarda aparte solo como
            // referencia, para respetar el contrato y la columna "salt" de la tabla.
            return new ResultadoHash(hash, salt);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <paramref name="saltAlmacenado"/> no es necesario: BCrypt lo lee del
        /// propio hash. Se mantiene el parametro para respetar el contrato.
        /// </remarks>
        public bool Verificar(string textoPlano, string hashAlmacenado, string saltAlmacenado)
        {
            if (string.IsNullOrEmpty(textoPlano) || string.IsNullOrEmpty(hashAlmacenado))
                return false;

            try
            {
                // Compara la contrasena ORIGINAL contra el hash guardado en la BD.
                return BCrypt.Net.BCrypt.Verify(textoPlano, hashAlmacenado);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // El hash guardado no tiene formato BCrypt (por ejemplo, un hash
                // SHA-256 antiguo): se trata como credencial invalida.
                return false;
            }
        }
    }
}
