using Newtonsoft.Json;

namespace Parcial2_Parte_2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var persona = new
            {
                Nombre = "Cristell",
                Edad = "20",
                Carrera = "Ingenieria en Sistemas"
            };

            string json = JsonConvert.SerializeObject(persona);

            Console.WriteLine(json);
        }
    }
}
