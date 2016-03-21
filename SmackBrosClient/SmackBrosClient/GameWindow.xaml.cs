using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Media;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shapes;
using SharpGL.SceneGraph;
using SharpGL;
using SharpGL.Enumerations;
using OpenTK;
using OpenTK.Graphics;

namespace SmackBrosClient
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        ScreenManager screenManager = new ScreenManager();
        private bool DebugMode = true; 

        public GameWindow()
        {
            InitializeComponent();
        }
        
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            OpenGL gl = openGLControl.OpenGL;
            screenManager.DrawScreens(); 
        }
        private void Update()
        {
            screenManager.UpdateScreens(); 
        }
        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            //  TODO: Initialise OpenGL here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the clear color.
            gl.ClearColor(0, 0, 0, 0);
            
        }
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
        }
        protected void OnUnload(EventArgs e)
        {
        } 
        public void FadeBufferToBlack(float alpha)
        {
            
        }
    }
}
