using System;
using CG_Biblioteca;
using gcgcg;
using OpenTK.Graphics.OpenGL;

namespace CG_N2
{
    internal class Circulo : ObjetoGeometria
    {

        private double raio;
        private Ponto4D ptoCentro = new Ponto4D();

        public Circulo(char rotulo, Objeto paiRef, Ponto4D ptoCentro, double raio) : base(rotulo, paiRef)
        {
            this.raio = raio;
            this.ptoCentro = ptoCentro;
        }

        protected override void DesenharObjeto()
        {

            Ponto4D pto;
            
            for (int angulo = 0; angulo < 360; angulo += 5)
            {
                pto = Matematica.GerarPtosCirculo(angulo, raio);
                pto += ptoCentro;
                base.PontosAdicionar(pto);
            }

            GL.PointSize(4);
            GL.Begin(PrimitiveType.Points);
            foreach (Ponto4D ponto in pontosLista)
            {
                
                GL.Vertex2(ponto.X, ponto.Y);
            }
            GL.End();
           
        }
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Circulo: " + base.rotulo + "\n";
            for (var i = 0; i < pontosLista.Count; i++)
            {
                retorno += "P" + i + "[" + pontosLista[i].X + "," + pontosLista[i].Y + "," + pontosLista[i].Z + "," + pontosLista[i].W + "]" + "\n";
            }
            return (retorno);
        }
    }
}