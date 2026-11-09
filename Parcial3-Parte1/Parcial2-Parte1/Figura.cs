using System;
using System.Collections.Generic;
using System.Text;

namespace Parcial2_Parte1
{
    enum TrianguloTipo
    {
        Equilatero,
        Isosceles,
        Escaleno
    };

    class Figura
    {
        public virtual double calcularArea()
        {
            return 0;
        }

        public virtual double calcularPerimetro()
        {
            return 0;
        }
    };

    class Cuadrado : Figura
    {
        double lado;
        public Cuadrado(double lado)
        {
            this.lado = lado;
        }
        public override double calcularArea()
        {
            return lado * lado;
        }
        public override double calcularPerimetro()
        {
            return 4 * lado;
        }
    };

    class Rectangulo : Figura
    {
        double baseRectangulo;
        double altura;
        public Rectangulo(double baseRectangulo, double altura)
        {
            this.baseRectangulo = baseRectangulo;
            this.altura = altura;
        }
        public override double calcularArea()
        {
            return baseRectangulo * altura;
        }
        public override double calcularPerimetro()
        {
            return 2 * (baseRectangulo + altura);
        }
    }

    class Triangulo : Figura
    {
        double baseTriangulo;
        double altura;

        public TrianguloTipo tipo;

        public Triangulo(double baseTriangulo, double altura, TrianguloTipo tipo)
        {
            this.baseTriangulo = baseTriangulo;
            this.altura = altura;
            this.tipo = tipo;
        }
        public override double calcularArea()
        {
            return (baseTriangulo * altura) / 2;
        }
        public override double calcularPerimetro()
        {
            switch (tipo)
            {
                case TrianguloTipo.Equilatero:
                    return 3 * baseTriangulo;
                case TrianguloTipo.Isosceles:
                    // Assuming two sides are equal and the third is the base
                    return 2 * baseTriangulo + altura;
                case TrianguloTipo.Escaleno:
                    // Assuming all sides are different
                    return baseTriangulo + altura + Math.Sqrt(baseTriangulo * baseTriangulo + altura * altura);
                default:
                    return 0;
            }
        }
    }
}
