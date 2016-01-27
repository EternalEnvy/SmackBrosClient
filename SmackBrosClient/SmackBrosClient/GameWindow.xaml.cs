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
using Assimp;
using Assimp.Configs;
using OpenTK;
using OpenTK.Graphics;

namespace SmackBrosClient
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        //Network based
        static bool serverInitialized = false;
        static int port1 = 1521;
        static int port2 = 1522;
        static int port3 = 1523;
        static UdpClient client;
        static UdpClient client2;
        static UdpClient client3;
        static Thread ReceivingThread;
        static Thread GameUpdateThread;
        static Thread InputThread;
        static string ServerIP;
        static List<string> clientIPList = new List<string>();
        static readonly object packetProcessQueueLock = new object();
        static Queue<Packet> packetProcessQueue = new Queue<Packet>();
        static DateTime lastUpdateInputThread = DateTime.Now;
        static DateTime lastUpdateUpdateThread = DateTime.Now;
        static int BufferStateLength = 4;

        //The current rotation.
        private float rotation = 0.0f;
        private bool TexturesInitialised = false;
        private Bitmap gImage1;
        private System.Drawing.Imaging.BitmapData gbitmapdata;
        private uint[] gtexture = new uint[1];

        private Assimp.Scene m_model;
        private Vector3 m_sceneCenter, m_sceneMin, m_sceneMax;
        //private float m_angle;
        //private int m_displayList;
        //private int m_texId;

        //Collections
        public Dictionary<string, SoundPlayer> songs = new Dictionary<string, SoundPlayer>();
        public Dictionary<string, SoundPlayer> effects = new Dictionary<string, SoundPlayer>();
        public Dictionary<string, Bitmap> images = new Dictionary<string, Bitmap>();

        AssimpContext Importer = new AssimpContext();

        List<Smacker> smackers = new List<Smacker>();
        private List<Inputs> inputlist = new List<Inputs>();

        //Temporary; for fun
        public bool songStarted = false;

        public GameWindow()
        {
            StartThreads(); 
            InitializeComponent();
            LoadContent();
        }
        public void LoadContent()
        {
            songs.Add("Melee Menu", new SoundPlayer(@"C:\Users\Lee\Documents\GitHub\SmackBrosClient\SmackBrosClient\files\menu.wav"));
            OpenGL gl = openGLControl.OpenGL;
            String fileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "duck.dae");
            AssimpContext importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            m_model = importer.ImportFile(fileName, PostProcessPreset.TargetRealTimeMaximumQuality);
            ComputeBoundingBox();
            
        }
        private void playSimpleSound()
        {
            if (!songStarted)
            {
                songs["Melee Menu"].Play();
            }
            songStarted = true;
        }
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        { 
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;
            if (!TexturesInitialised)
            {
                InitialiseTexture(ref gl);
            }
            //  Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //  Load the identity matrix.
            gl.LoadIdentity();
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_NORMALIZE);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, gtexture[0]);
            gl.Color(1.0f, 1.0f, 1.0f, 0.1f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.FrontFace(OpenGL.GL_FRONT_FACE);

            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(gImage1.Width, gImage1.Height, 1.0f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(0.0f, gImage1.Height, 1.0f);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(0.0f, 0.0f, 1.0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(gImage1.Width, 0.0f, 1.0f);

            gl.End();
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Ortho(0.0, (double)gImage1.Width, (double)gImage1.Height, 0.0, -1.0, 1.0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Disable(OpenGL.GL_DEPTH_TEST);
        }
        private void InitialiseTexture(ref OpenGL gl)
        {
            gImage1 = new Bitmap(@"C:\Users\Lee\Documents\GitHub\SmackBrosClient\SmackBrosClient\files\meleemenu1.jpg");
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, gImage1.Width, gImage1.Height);
            gbitmapdata = gImage1.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            gImage1.UnlockBits(gbitmapdata);
            gl.GenTextures(1, gtexture);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, gtexture[0]);
            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA, gImage1.Width, gImage1.Height, 0, OpenGL.GL_BGR_EXT, OpenGL.GL_UNSIGNED_BYTE, gbitmapdata.Scan0);

            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);            

            TexturesInitialised = true;
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
            if(!songStarted)
            {
                playSimpleSound();
                songStarted = true;
            }
        }
        private void ComputeBoundingBox()
        {
            m_sceneMin = new Vector3(1e10f, 1e10f, 1e10f);
            m_sceneMax = new Vector3(-1e10f, -1e10f, -1e10f);

            m_sceneCenter.X = (m_sceneMin.X + m_sceneMax.X) / 2.0f;
            m_sceneCenter.Y = (m_sceneMin.Y + m_sceneMax.Y) / 2.0f;
            m_sceneCenter.Z = (m_sceneMin.Z + m_sceneMax.Z) / 2.0f;
        }
        private void ComputeBoundingBox(Node node, ref Vector3 min, ref Vector3 max, ref Matrix4 trafo)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.PushMatrix();
            gl.LoadIdentity();
            Matrix4 prev = trafo;
            trafo = Matrix4.Mult(prev, FromMatrix(node.Transform));

            if (node.HasMeshes)
            {
                foreach (int index in node.MeshIndices)
                {
                    Mesh mesh = m_model.Meshes[index];
                    for (int i = 0; i < mesh.VertexCount; i++)
                    {
                        Vector3 tmp = FromVector(mesh.Vertices[i]);
                        Vector3.Transform(tmp, trafo);

                        min.X = Math.Min(min.X, tmp.X);
                        min.Y = Math.Min(min.Y, tmp.Y);
                        min.Z = Math.Min(min.Z, tmp.Z);

                        max.X = Math.Max(max.X, tmp.X);
                        max.Y = Math.Max(max.Y, tmp.Y);
                        max.Z = Math.Max(max.Z, tmp.Z);
                    }
                }
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                ComputeBoundingBox(node.Children[i], ref min, ref max, ref trafo);
            }
            trafo = prev;
        }
        protected void OnUnload(EventArgs e)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.DeleteTextures(1, gtexture);
        } 
        //Handles the Resized event of the openGLControl1 control 
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            if (!TexturesInitialised)
            {
                gl.Ortho(-1, 1, -1, 1, -1, 1);
            }
            else
            {
                gl.Ortho(0, gImage1.Width, gImage1.Height, 0, -1, 1);
            }
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Disable(OpenGL.GL_DEPTH_TEST);

            //  Create a perspective transformation.
            gl.Perspective(45.0f, (double)Width / (double)Height, 1.0, 1000.0);
            gl.Viewport(0, 0, (int)Width, (int)Height);
            //  Use the 'look at' helper function to position and aim the camera.
            gl.LookAt(-5, 5, -5, 0, 0, 0, 0, 1, 0);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }
        private bool LoadModelAsset (string path)
        {
            path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
	        /* we are taking one of the postprocessing presets to avoid
	        spelling out 20+ single postprocessing flags here. */ 
	        m_model = Importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);
	        if (m_model != null) 
            {
		        Get_Bounding_Box(m_sceneMin, m_sceneMax);
                m_sceneCenter.X = (m_sceneMin.X + m_sceneMin.X) / 2.0f;
                m_sceneCenter.Y = (m_sceneMin.Y + m_sceneMin.Y) / 2.0f;
                m_sceneCenter.Z = (m_sceneMin.Z + m_sceneMin.Z) / 2.0f;
		        return true;
            }
	        return false;
        }
        void Get_Bounding_Box (Vector3 min, Vector3 max)
        {

	        Matrix4x4 trafo = Matrix4x4.Identity;

            min.X = min.Y = min.Z =  1e10f;
	        max.X = max.Y = max.Z = -1e10f;
	        Get_Bounding_Box_For_Node(m_model.RootNode,min,max, trafo);
        }
        void Get_Bounding_Box_For_Node (Node nd, Vector3 min, Vector3 max, Matrix4x4 trafo)
        {
            OpenGL gl = openGLControl.OpenGL;
            Matrix4x4 prev = trafo; 
            trafo = trafo * nd.Transform;
            for (int n = 0; n < nd.MeshCount; ++n) {
                Mesh mesh = m_model.Meshes[nd.MeshIndices[n]];
                for (int t = 0; t < mesh.VertexCount; ++t) 
                {
                    Vector3 tmp = FromVector(mesh.Vertices[t]);
                    tmp = Vector3.Transform(tmp, FromMatrix(trafo));
                    min.X = Math.Min(min.X,tmp.X);
                    min.Y = Math.Min(min.Y,tmp.Y);
                    min.Z = Math.Min(min.Z,tmp.Z);

                    max.X = Math.Max(max.X,tmp.X);
                    max.Y = Math.Max(max.Y,tmp.Y);
                    max.Z = Math.Max(max.Z,tmp.Z);
                }
            }
            for (int k = 0; k < nd.ChildCount; ++k) 
            {
                Get_Bounding_Box_For_Node(nd.Children[k],min,max,trafo);
            }
            trafo = prev;
        }
        private void RecursiveRender(Assimp.Scene scene, Node node)
        {
            OpenGL gl = openGLControl.OpenGL;
            Matrix4 m = FromMatrix(node.Transform);
            m.Transpose();
            gl.PushMatrix();
            gl.MultMatrix(FloatFromMatrix(m));

            if (node.HasMeshes)
            {
                foreach (int index in node.MeshIndices)
                {
                    Mesh mesh = scene.Meshes[index];
                    ApplyMaterial(m_model.Materials[mesh.MaterialIndex]);

                    if (mesh.HasNormals)
                    {

                        gl.Enable(OpenGL.GL_LIGHTING);
                    }
                    else
                    {
                        gl.Disable(OpenGL.GL_LIGHTING);
                    }

                    bool hasColors = mesh.HasVertexColors(0);
                    if (hasColors)
                    {
                        gl.Enable(OpenGL.GL_COLOR_MATERIAL);
                    }
                    else
                    {
                        gl.Disable(OpenGL.GL_COLOR_MATERIAL);
                    }

                    bool hasTexCoords = mesh.HasTextureCoords(0);

                    foreach (Assimp.Face face in mesh.Faces)
                    {
                        BeginMode faceMode;
                        switch (face.IndexCount)
                        {
                            case 1:
                                faceMode = BeginMode.Points;
                                break;
                            case 2:
                                faceMode = BeginMode.Lines;
                                break;
                            case 3:
                                faceMode = BeginMode.Triangles;
                                break;
                            default:
                                faceMode = BeginMode.Polygon;
                                break;
                        }

                        gl.Begin(faceMode);
                        for (int i = 0; i < face.IndexCount; i++)
                        {
                            int indice = face.Indices[i];
                            if (hasColors)
                            {
                                Color4 vertColor = FromColor(mesh.VertexColorChannels[0][indice]);
                                if (mesh.HasNormals)
                                {
                                    Vector3 normal = FromVector(mesh.Normals[indice]);
                                    gl.Normal(normal.X, normal.Y, normal.Z);
                                }
                                if (hasTexCoords)
                                {
                                    Vector3 uvw = FromVector(mesh.TextureCoordinateChannels[0][indice]);
                                    gl.TexCoord(uvw.X, 1 - uvw.Y);
                                }
                                Vector3 pos = FromVector(mesh.Vertices[indice]);
                                gl.Vertex(pos.X,pos.Y,pos.Z);
                            }
                            gl.End();
                        }
                    }
                }

                for (int i = 0; i < node.ChildCount; i++)
                {
                    RecursiveRender(m_model, node.Children[i]);
                }
            }
        }
        private void ApplyMaterial(Material mat)
        {
            OpenGL gl = openGLControl.OpenGL;
            if (mat.GetMaterialTextureCount(TextureType.Diffuse) > 0)
            {
                TextureSlot tex = new TextureSlot();
                mat.GetMaterialTexture(TextureType.Diffuse, 0, out tex);
                LoadModelAsset(tex.FilePath);
            }

            Color4 color = new Color4(.8f, .8f, .8f, 1.0f);
            if (mat.HasColorDiffuse)
            {
                // color = FromColor(mat.ColorDiffuse);
            }
            var colorf = new float[4];
            color4_to_float4(color, colorf);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, colorf);

            color = new Color4(0, 0, 0, 1.0f);
            if (mat.HasColorSpecular)
            {
                color = FromColor(mat.ColorSpecular);
            }
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SPECULAR, colorf);

            color = new Color4(.2f, .2f, .2f, 1.0f);
            if (mat.HasColorAmbient)
            {
                color = FromColor(mat.ColorAmbient);
            }
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT, colorf);

            color = new Color4(0, 0, 0, 1.0f);
            if (mat.HasColorEmissive)
            {
                color = FromColor(mat.ColorEmissive);
            }
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_EMISSION, colorf);

            float shininess = 1;
            float strength = 1;
            if (mat.HasShininess)
            {
                shininess = mat.Shininess;
            }
            if (mat.HasShininessStrength)
            {
                strength = mat.ShininessStrength;
            }

            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SHININESS, shininess * strength);
        } 
        void set_float4(float[] f, float a, float b, float c, float d)
        {
	        f[0] = a;
	        f[1] = b;
	        f[2] = c;
	        f[3] = d;
        }
        void color4_to_float4(Color4 c, float[]f)
        {
	        f[0] = c.R;
	        f[1] = c.G;
	        f[2] = c.B;
	        f[3] = c.A;
        }
        private float[] FloatFromMatrix(Matrix4 mat)
        {
            float[] f = new float[16];
            f[0] = mat.M11;
            f[1] = mat.M12;
            f[2] = mat.M13;
            f[3] = mat.M14;
            f[4] = mat.M21;
            f[5] = mat.M22;
            f[6] = mat.M23;
            f[7] = mat.M24;
            f[8] = mat.M31;
            f[9] = mat.M32;
            f[10] = mat.M33;
            f[11] = mat.M34;
            f[12] = mat.M41;
            f[13] = mat.M42;
            f[14] = mat.M43;
            f[15] = mat.M44;
            return f;
        }
        private Matrix4 FromMatrix(Matrix4x4 mat)
        {
            Matrix4 m = new Matrix4();
            m.M11 = mat.A1;
            m.M12 = mat.A2;
            m.M13 = mat.A3;
            m.M14 = mat.A4;
            m.M21 = mat.B1;
            m.M22 = mat.B2;
            m.M23 = mat.B3;
            m.M24 = mat.B4;
            m.M31 = mat.C1;
            m.M32 = mat.C2;
            m.M33 = mat.C3;
            m.M34 = mat.C4;
            m.M41 = mat.D1;
            m.M42 = mat.D2;
            m.M43 = mat.D3;
            m.M44 = mat.D4;
            return m;
        }
        void StartThreads()
        {
            new Task(() =>
            {
                client = client ?? new UdpClient(port1, AddressFamily.InterNetwork);
                client2 = client2 ?? new UdpClient(port2, AddressFamily.InterNetwork);
                client3 = client3 ?? new UdpClient(port3, AddressFamily.InterNetwork);
                IPHostEntry host;
                string localIP = "?";
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                    }
                }
                ServerIP = localIP;
                ReceivingThread = new Thread(() => PacketQueue.Instance.ReceivingLoop(client2, new IPEndPoint(IPAddress.Any, port2), packetProcessQueue, packetProcessQueueLock));
                ReceivingThread.IsBackground = true;
                ReceivingThread.Start();
            }).Start();
            new Task(() =>
            {

                InputThread = new Thread(() => HandleInput());
                InputThread.SetApartmentState(ApartmentState.STA);
                InputThread.Start();
            }).Start();
            serverInitialized = true;
        }
        private void UpdateToState(GameStatePacket statePacket)
        {

        }
        private void LinearInterpolateUpdateToState(GameStatePacket statePacket1, GameStatePacket statePacket2, long targetSequence)
        {
            float weight = (targetSequence - statePacket1.Sequence) / (statePacket2.Sequence - statePacket1.Sequence);
            for(int i = 0; i < smackers.Count(); i++)
            {
                smackers[i].Position = Vector3.Lerp(statePacket1.Smackers[i], statePacket2.Smackers[i], weight);
            }
        }
        private Vector3 FromVector(Vector3D vec)
        {
            Vector3 v = new Vector3(vec.X, vec.Y,vec.Z);
            return v;
        }

        private Color4 FromColor(Color4D color)
        {
            Color4 c;
            c.R = color.R;
            c.G = color.G;
            c.B = color.B;
            c.A = color.A;
            return c;
        }
    }
}
