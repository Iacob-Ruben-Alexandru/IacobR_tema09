using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using System.Drawing.Imaging;
using System.Threading;

namespace Pacanele
{
    /// <summary>
    /// Clasa Form1
    /// </summary>
    public partial class Form1: Form
    {
        float eyePosX;
        bool gameOver = false;
        float eyePosY;
        float eyePosZ;
        int cicluri = 1;
        float camDepth;
        int bani = 100;
        int miza;
        bool inTragere = false;
        bool trage;
        int lastA = 0, lastB = 0, lastC = 0; 
        bool firstDraw = true;
        int[] textures = new int[4];
        Random rnd = new Random();
        int a, b, c;
        public Form1()
        {
            InitializeComponent();
            
            glControl1.Load += Form1_Load;
            glControl1.Paint += Form1_Paint;
        }
        /// <summary>
        /// Ce facem la Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            SetupValues();
            GL.Enable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            LoadTextures();
        }
        /// <summary>
        /// Initierea valorilor la load
        /// </summary>

        private void SetupValues()
        {
            camDepth = 1.04f;
            numericUpDown2.Value = 1;
        }
        /// <summary>
        /// incarcarea texturilor
        /// </summary>
        private void LoadTextures()
        {
            GL.GenTextures(textures.Length, textures);
            LoadTexture(textures[0], "cirese.png");
            LoadTexture(textures[1], "lamaie.png");
            LoadTexture(textures[2], "pruna.png");
            LoadTexture(textures[3], "sapte.png");
        }
        private void LoadTexture(int textureId, string filename)
        {
            Bitmap bmp = new Bitmap(filename);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                          bmp.Width, bmp.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                          PixelType.UnsignedByte, data.Scan0);

            bmp.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
        }
        /// <summary>
        /// Functia de paint care pana la primul roll afiseaza 7.7.7
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Paint(object sender, EventArgs e)
        {
            glControl1.MakeCurrent();
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView((float)camDepth, (float)glControl1.Width / (float)glControl1.Height, 1, 10000);   
            Matrix4 lookat = Matrix4.LookAt(80, 40, 100, 80, 40, 0, 0, 1, 0);                   
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);                                                    
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);                                     
            GL.Enable(EnableCap.DepthTest);                                                               
            GL.DepthFunc(DepthFunction.Less);
            //GL.Enable(EnableCap.Texture2D);
            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            //LoadTextures();
            if (!firstDraw)
            {
                drawSlot(0, 0, lastA);
                drawSlot(60, 0, lastB);
                drawSlot(120, 0, lastC);
            }
            else
            {
                label6.Text = "Distractie placuta";
                drawSlot(0, 0, 3);
                drawSlot(60, 0, 3);
                drawSlot(120, 0, 3);
            }

            label2.Text = bani.ToString();
           
            glControl1.SwapBuffers();
        }
        /// <summary>
        /// Logica de tragere, nu am cum trece la bani negativi, implementarea ciclurilor si apoi verificarea daca am castigat sau nu
        /// </summary>
        /// <returns></returns>
        private async Task Tragere()
        {
            int tempA, tempB, tempC; 
            if (bani - miza < 0)
            {
                miza = bani;
                numericUpDown1.Value = miza;
            }
            bani = bani - miza;
            label2.Text = bani.ToString();
            for (int i = 0; i < cicluri; i++)
            {
                tempA = rnd.Next(0, 4);
                tempB = rnd.Next(0, 4);
                tempC = rnd.Next(0, 4);

                lastA = tempA;
                lastB = tempB;
                lastC = tempC;

                glControl1.Invalidate();
                // ca sa fie conform cerintei ar trebui a pun await Task.Delay(500) dar e foarte slow;
                await Task.Delay(500);        
            }
            a = rnd.Next(0, 4);
            b = rnd.Next(0, 4);
            c = rnd.Next(0, 4);
            lastA = a;
            lastB = b;
            lastC = c;
            label2.Text = bani.ToString();
            //drawSlot(0, 0, a);
            //drawSlot(60, 0, b);
            //drawSlot(120, 0, c);
            if (a == b && b == c && c == 0)
            {
                bani += miza * 15;
                label6.Text = "Ati castigat";  
            }
            else if (a == b && b == c && c == 1)
            {
                bani += miza * 20;
                label6.Text = "Ati castigat";
            }
            else if (a == b && b == c && c == 2)
            {
                bani += miza * 10;
                label6.Text = "Ati castigat";
            }
            else if (a == b && b == c && c == 3)
            {
                bani += miza * 100;
                label6.Text = "Ati castigat";
            }
            else
            {
                label6.Text = "Ati pierdut";
            }
            if (bani == 0)
            {
                gameOver = true;
                label6.Text = "Ati pierdut tot";
            }

            firstDraw = false;
            label2.Text = bani.ToString();
            trage = false;
            inTragere = false;
            glControl1.Invalidate();
        }
        /// <summary>
        /// Cu acesta modific miza.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            miza = (int)numericUpDown1.Value;
        }
        /// <summary>
        /// Desenare Slots
        /// </summary>
        /// <param name="xStart"></param>
        /// <param name="yStart"></param>
        /// <param name="textureId"></param>

        private void drawSlot(int xStart, int yStart, int textureId)
        {
            GL.BindTexture(TextureTarget.Texture2D, textures[textureId]);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 1); GL.Vertex3(xStart, yStart, 0);
            GL.TexCoord2(1, 1); GL.Vertex3(xStart + 40, yStart, 0);
            GL.TexCoord2(1, 0); GL.Vertex3(xStart + 40, yStart + 80, 0);
            GL.TexCoord2(0, 0); GL.Vertex3(xStart, yStart + 80, 0);
            GL.End();
            
        }
        /// <summary>
        /// Cu acesta modific numarul de cicluri
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            cicluri = (int)numericUpDown2.Value;
            glControl1.Invalidate();
        }
        /// <summary>
        /// daca apas pe butonul de tragere, verific daca sunt in animatia de tragere, sa previn click-urile multiple si de asemenea am implementat logica pentru
        /// cand raman fara bani.
        /// </summary>
        int ok = 0;
        private async void button1_Click(object sender, EventArgs e)
        {
            firstDraw = false;
            if(!inTragere)
            {
                inTragere = true;
                

                await Tragere();
                if (gameOver)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        DialogResult result = MessageBox.Show(
                            "Ai pierdut tot! Vrei să încerci din nou?",
                            "Game Over",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );

                        if (result == DialogResult.Yes)
                        {
                            bani = 100;
                            label2.Text = bani.ToString();
                            gameOver = false;
                            label6.Text = "Distractie placuta";
                        }
                        else
                        {
                            Application.Exit();
                        }
                        gameOver = false;
                    }));
                }
            }
            
            glControl1.Invalidate();
        }
    }
}
