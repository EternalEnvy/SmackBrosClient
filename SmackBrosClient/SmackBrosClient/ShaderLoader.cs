using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.IO;
using SharpGL;

namespace SmackBrosClient
{
    public partial class GameWindow:Window 
    {
        string ReadFile(string filePath)
        {
            if(File.Exists(filePath))
            {
                StringBuilder sb = new StringBuilder();
                using (StreamReader reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        sb.AppendLine(reader.ReadLine());
                    }
                }
                return sb.ToString();
            }
            return "";
        }
        uint LoadShader(string vertexPath, string fragmentPath)
        {
            OpenGL gl = openGLControl.OpenGL;
            uint vertShader = gl.CreateShader(OpenGL.GL_VERTEX_SHADER);
            uint fragShader = gl.CreateShader(OpenGL.GL_FRAGMENT_SHADER);
            string vertShaderStr = ReadFile(vertexPath);
            string fragShaderStr = ReadFile(fragmentPath);
            
            if(DebugMode)
                Console.WriteLine("Compiling vertex shader");

            gl.ShaderSource(vertShader, vertShaderStr);
            gl.CompileShader(vertShader);

            StringBuilder builder = new StringBuilder(2048);
            gl.GetShaderInfoLog(vertShader, 2048, IntPtr.Zero, builder);
            if (DebugMode)
                Console.WriteLine(builder.ToString());

            if (DebugMode) 
                Console.WriteLine("Compiling fragment shader");

            gl.ShaderSource(fragShader, fragShaderStr);
            gl.CompileShader(fragShader);

            builder = new StringBuilder();
            gl.GetShaderInfoLog(vertShader, 2048, IntPtr.Zero, builder);
            if (DebugMode)
                Console.WriteLine(builder.ToString());

            if (DebugMode) 
                Console.WriteLine("Linking...");

            uint program = gl.CreateProgram();
            gl.AttachShader(program, vertShader);
            gl.AttachShader(program, fragShader);
            gl.LinkProgram(program);

            builder = new StringBuilder();
            gl.GetProgramInfoLog(program, 2048, IntPtr.Zero, builder);

            if (DebugMode) 
                Console.WriteLine(builder.ToString());

            gl.DeleteShader(vertShader);
            gl.DeleteShader(fragShader);
            return program;
        }
    }
}
