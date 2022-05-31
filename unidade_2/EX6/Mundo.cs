/**
  Autor: Dalton Solano dos Reis
**/

//#define CG_Privado // código do professor.
#define CG_Gizmo  // debugar gráfico.
//#define CG_Debug // debugar texto.
#define CG_OpenGL // render OpenGL.
//#define CG_DirectX // render DirectX.

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK.Input;
using CG_Biblioteca;
using CG_N2;

namespace gcgcg
{
  class Mundo : GameWindow
  {
    private static Mundo instanciaMundo = null;

    private Mundo(int width, int height) : base(width, height) { }

    public static Mundo GetInstance(int width, int height)
    {
      if (instanciaMundo == null)
        instanciaMundo = new Mundo(width, height);
      return instanciaMundo;
    }

    private CameraOrtho camera = new CameraOrtho();
    protected List<Objeto> objetosLista = new List<Objeto>();
    private ObjetoGeometria objetoSelecionado = null;
    private char objetoId = '@';
    private bool bBoxDesenhar = false;
    int mouseX, mouseY;   //TODO: achar método MouseDown para não ter variável Global
    private bool mouseMoverPto = false;
   // private Retangulo obj_Retangulo;

    private SegReta obj_SegReta;
     private int QtdPontos = 0;
    private Ponto4D PontoA { get; set; }
    private Ponto4D PontoB  { get; set; }
    private Ponto4D PontoC  { get; set; }
    private Ponto4D PontoD { get; set; }
    private Ponto4D PontoSelecionado;

    private Spline obj_Spline;
#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      camera.xmin = -300; camera.xmax = 300; camera.ymin = -300; camera.ymax = 300;

      Console.WriteLine(" --- Ajuda / Teclas: ");
      Console.WriteLine(" [  H     ] mostra teclas usadas. ");
       Console.WriteLine("(deslocar para esquerda): E");
    Console.WriteLine("(deslocar para direita): D");
    Console.WriteLine("(deslocar para cima): C");
    Console.WriteLine("(deslocar para baixo): B");

        QtdPontos = 15;
        PontoA = new Ponto4D(x: -100, y: -100);
        PontoB = new Ponto4D(x: -100, y: 100);
        PontoC = new Ponto4D(x: 100, y: 100);
        PontoD = new Ponto4D(x: 100, y: -100);
        PontoSelecionado = PontoA;

    objetoId = Utilitario.charProximo(objetoId);
    obj_SegReta = new SegReta(objetoId, null, PontoC, PontoB);
    obj_SegReta.ObjetoCor.CorR = 87; obj_SegReta.ObjetoCor.CorG = 199; obj_SegReta.ObjetoCor.CorB = 212;
    objetosLista.Add(obj_SegReta);

    objetoId = Utilitario.charProximo(objetoId);
    obj_SegReta = new SegReta(objetoId, null, PontoB, PontoA);
    obj_SegReta.ObjetoCor.CorR = 87; obj_SegReta.ObjetoCor.CorG = 199; obj_SegReta.ObjetoCor.CorB = 212;
    objetosLista.Add(obj_SegReta);

    objetoId = Utilitario.charProximo(objetoId);
    obj_SegReta = new SegReta(objetoId, null, PontoD, PontoC);
    obj_SegReta.ObjetoCor.CorR = 87; obj_SegReta.ObjetoCor.CorG = 199; obj_SegReta.ObjetoCor.CorB = 212;
    objetosLista.Add(obj_SegReta);

    objetoId = Utilitario.charProximo(objetoId);
    obj_Spline = new Spline(objetoId, null, PontoA, PontoB, PontoC, PontoD, QtdPontos);
    obj_Spline.ObjetoCor.CorR = 255; obj_Spline.ObjetoCor.CorG = 255; obj_Spline.ObjetoCor.CorB = 0;
    objetosLista.Add(obj_Spline);
    objetoSelecionado = obj_Spline;


#if CG_Privado
      objetoId = Utilitario.charProximo(objetoId);
      obj_SegReta = new Privado_SegReta(objetoId, null, new Ponto4D(50, 150), new Ponto4D(150, 250));
      obj_SegReta.ObjetoCor.CorR = 255; obj_SegReta.ObjetoCor.CorG = 255; obj_SegReta.ObjetoCor.CorB = 0;
      objetosLista.Add(obj_SegReta);
      objetoSelecionado = obj_SegReta;

      objetoId = Utilitario.charProximo(objetoId);
      obj_Circulo = new Privado_Circulo(objetoId, null, new Ponto4D(100, 300), 50);
      obj_Circulo.ObjetoCor.CorR = 0; obj_Circulo.ObjetoCor.CorG = 255; obj_Circulo.ObjetoCor.CorB = 255;
      obj_Circulo.PrimitivaTipo = PrimitiveType.Points;
      obj_Circulo.PrimitivaTamanho = 5;
      objetosLista.Add(obj_Circulo);
      objetoSelecionado = obj_Circulo;
#endif
#if CG_OpenGL
      GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
#endif
    }
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);
#if CG_OpenGL
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadIdentity();
      GL.Ortho(camera.xmin, camera.xmax, camera.ymin, camera.ymax, camera.zmin, camera.zmax);
#endif
    }
    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);
#if CG_OpenGL
      GL.Clear(ClearBufferMask.ColorBufferBit);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();
#endif
#if CG_Gizmo      
      Sru3D();
#endif
      for (var i = 0; i < objetosLista.Count; i++)
        objetosLista[i].Desenhar();
#if CG_Gizmo
      if (bBoxDesenhar && (objetoSelecionado != null))
        objetoSelecionado.BBox.Desenhar();
#endif
      this.SwapBuffers();
    }

    protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
    {
      if (e.Key == Key.H)
        Utilitario.AjudaTeclado();
      else if (e.Key == Key.Escape)
        Exit();
      /*else if (e.Key == Key.E)
      {
        Console.WriteLine("--- Objetos / Pontos: ");
        for (var i = 0; i < objetosLista.Count; i++)
        {
          Console.WriteLine(objetosLista[i]);
        }
      }*/
      else if (e.Key == Key.E)
        {
            PontoSelecionado.X--;

        }
        else if (e.Key == Key.D)
        {
            PontoSelecionado.X++;
        }
        else if (e.Key == Key.C)
        {
            PontoSelecionado.Y++;
        }
        else if (e.Key == Key.B)
        {
            PontoSelecionado.Y--;
        }
        else if (e.Key == Key.Minus || e.Key == Key.KeypadMinus)
        {
           if (obj_Spline.qtdPontos > 1)
            obj_Spline.qtdPontos--;
        }
        else if (e.Key == Key.Plus || e.Key == Key.KeypadPlus)
        {
           obj_Spline.qtdPontos++;
        }
        else if (e.Key == Key.Number1 || e.Key == Key.Keypad1)
        {
            PontoSelecionado = PontoA;
        }
        else if (e.Key == Key.Number2 || e.Key == Key.Keypad2)
        {
            PontoSelecionado = PontoB;
        }
        else if (e.Key == Key.Number3 || e.Key == Key.Keypad3)
        {
            PontoSelecionado = PontoC;
        }
        else if (e.Key == Key.Number4 || e.Key == Key.Keypad4)
        {
            PontoSelecionado = PontoD;
        }

#if CG_Gizmo
      else if (e.Key == Key.O)
        bBoxDesenhar = !bBoxDesenhar;
#endif
      else if (e.Key == Key.V)
        mouseMoverPto = !mouseMoverPto;   //TODO: falta atualizar a BBox do objeto
      else
        Console.WriteLine(" __ Tecla não implementada.");
    }

    //TODO: não está considerando o NDC
    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
      mouseX = e.Position.X; mouseY = 600 - e.Position.Y; // Inverti eixo Y
      if (mouseMoverPto && (objetoSelecionado != null))
      {
        objetoSelecionado.PontosUltimo().X = mouseX;
        objetoSelecionado.PontosUltimo().Y = mouseY;
      }
    }
    


#if CG_Gizmo
        private void Sru3D()
    {
      GL.LineWidth(1);
      GL.Begin(PrimitiveType.Lines);
      // GL.Color3(1.0f,0.0f,0.0f);
      GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
      GL.Vertex3(0, 0, 0); GL.Vertex3(200, 0, 0);
      // GL.Color3(0.0f,1.0f,0.0f);
      GL.Color3(Convert.ToByte(0), Convert.ToByte(255), Convert.ToByte(0));
      GL.Vertex3(0, 0, 0); GL.Vertex3(0, 200, 0);
      // GL.Color3(0.0f,0.0f,1.0f);
      GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(255));
      GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 200);
      GL.End();

    GL.PointSize(9);
    GL.Begin(PrimitiveType.Points);
    if (PontoA.Equals(PontoSelecionado)) {
        GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
    } else
    {
        GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0));
    }
    GL.Vertex3(PontoA.X, PontoA.Y, 0);
    if (PontoB.Equals(PontoSelecionado))
    {
        GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
    }
    else
    {
        GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0));
    }
    GL.Vertex3(PontoB.X, PontoB.Y, 0);
    if (PontoC.Equals(PontoSelecionado))
    {
        GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
    }
    else
    {
        GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0));
    }
    GL.Vertex3(PontoC.X, PontoC.Y, 0);
    if (PontoD.Equals(PontoSelecionado))
    {
        GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
    }
    else
    {
        GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0));
    }
    GL.Vertex3(PontoD.X, PontoD.Y, 0);
    GL.End();
    }
#endif    
  }
  class Program
  {
    static void Main(string[] args)
    {
      Mundo window = Mundo.GetInstance(600, 600);
      window.Title = "CG_N2";
      window.Run(1.0 / 60.0);
    }
  }
}