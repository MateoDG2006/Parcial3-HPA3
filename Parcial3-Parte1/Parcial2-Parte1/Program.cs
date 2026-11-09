namespace Parcial2_Parte1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Figura triangulo_i = new Triangulo(3, 4, TrianguloTipo.Isosceles);
            Figura triangulo_e = new Triangulo(3, 4, TrianguloTipo.Equilatero);
            Figura triangulo_es = new Triangulo(3, 4, TrianguloTipo.Escaleno);

            Figura cuadrado = new Cuadrado(5);

            Figura rectangulo = new Rectangulo(3, 4);


            Figura[] figuras = { triangulo_i, triangulo_e, triangulo_es, cuadrado, rectangulo };

            for(int i = 0; i < figuras.Length; i++)
            {
                Console.WriteLine($"Figura {i + 1}: {figuras[i].GetType().Name}");
                Console.WriteLine($"Área: {figuras[i].calcularArea()}");
                Console.WriteLine($"Perímetro: {figuras[i].calcularPerimetro()}");
                if (figuras[i] is Triangulo)
                {
                    Triangulo triangulo = (Triangulo)figuras[i];
                    Console.WriteLine($"Tipo de triángulo: {triangulo.tipo}");
                }
                Console.WriteLine();
            }
        }
    }
}
