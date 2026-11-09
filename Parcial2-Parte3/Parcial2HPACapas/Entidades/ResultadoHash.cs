namespace Parcial2HPACapas.Entidades
{
    /// <summary>
    /// Objeto de transferencia que agrupa el resultado de generar un hash:
    /// el <see cref="Hash"/> calculado y el <see cref="Salt"/> aleatorio que lo
    /// acompana. Ambos deben almacenarse juntos para poder verificar despues.
    /// </summary>
    public class ResultadoHash
    {
        /// <summary>Hash BCrypt de la contrasena (incluye version, costo y salt).</summary>
        public string Hash { get; set; }

        /// <summary>Salt aleatorio generado por BCrypt al crear el hash.</summary>
        public string Salt { get; set; }

        /// <summary>Crea un resultado de hash con su salt asociado.</summary>
        public ResultadoHash(string hash, string salt)
        {
            Hash = hash;
            Salt = salt;
        }
    }
}
