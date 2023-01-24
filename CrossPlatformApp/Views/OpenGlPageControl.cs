using System;
using System.Diagnostics;
using System.Numerics;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using BitsOfNature.Core.Geometry3D;
using BitsOfNature.Core.Grid;
using BitsOfNature.Core.IO.Tracing;
using BitsOfNature.Core.Utils;
using BitsOfNature.Rendering.Vulkan._Sandbox;
using BitsOfNature.Rendering.Vulkan.Engine;
using BitsOfNature.Rendering.Vulkan.Interop;
using BitsOfNature.Rendering.Vulkan.Interop.Win32;
using BitsOfNature.Rendering.Vulkan.LowLevel;
using BitsOfNature.UI.Avalonia.Input;
using BitsOfNature.UI.Avalonia.Manipulation;
using BitsOfNature.UI.Avalonia.Rendering;
using CrossPlatformApp.Configuration;
using CrossPlatformApp.Infrastructure;
using static Avalonia.OpenGL.GlConsts;

namespace CrossPlatformApp.Views
{
    public class OpenGlPageControl : OpenGlControlBase, ISceneViewerControl
    {

        private class GLData
        {
            // Semaphores Shares between OpenGL and Vulkan
            public uint[] glReady = new uint[1];
            public uint[] glComplete = new uint[1];
            // Memory Object
            public int[] mem = new int[1];
            // Texture
            public int[] sharedTexture = new int[1];
            public int[] destinationGLTexture = new int[1]; 
        }

        private string _info;

        public static readonly DirectProperty<OpenGlPageControl, string> InfoProperty =
            AvaloniaProperty.RegisterDirect<OpenGlPageControl, string>("Info", o => o.Info, (o, v) => o.Info = v);

        public string Info
        {
            get => _info;
            private set => SetAndRaise(InfoProperty, ref _info, value);
        }

        public SceneCamera Camera => camera;

        public GridRect Viewport
        {
            get
            {
                return new GridRect(0, 0, (int)Bounds.Width, (int)Bounds.Height);
            }
        }

        private double vulcanRendererElapsedTimeInMs = 0;
        private GLData glData;
        private RenderingContext renderContext;
        private OGlInteropRenderTarget renderTarget;
        private TestRenderer testRenderer;

        private SceneCamera camera;
        private MouseAdapter mouseAdapter;


        private int vertexShader;
        private int fragmentShader;
        private int shaderProgram;
        private int vertexBufferObject;
        private int indexBufferObject;
        private int vertexArrayObject;
        private GlInterface glInterface;
        private GlExtrasInterface glExt;

        public bool InfoVisible { get; private set; }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8604 // Possible null reference argument.


        static OpenGlPageControl()
        {
            CreateOpenGLContext();
        }

        private static void CreateOpenGLContext()
        {
            try
            {

            }
            catch (Exception ex)
            {
                TraceApplication.Trace.Error("CreateOpenGLContext(): Error.\n{0}", ex);
            }
        }


        private string GetShader(bool fragment, string shader)
        {
            var version = (GlVersion.Type == GlProfileType.OpenGL ?
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? 150 : 130 :
                100);
            var data = "#version " + version + "\n";
            if (GlVersion.Type == GlProfileType.OpenGLES)
                data += "precision mediump float;\n";
            if (version >= 150)
            {
                shader = shader.Replace("attribute", "in");
                if (fragment)
                    shader = shader
                        .Replace("varying", "in")
                        .Replace("//DECLAREGLFRAG", "out vec4 outFragColor;")
                        .Replace("gl_FragColor", "outFragColor");
                else
                    shader = shader.Replace("varying", "out");
            }

            data += shader;

            return data;
        }

        private string VertexShaderSource => GetShader(false, @"
            uniform mat4 projection_matrix;
            uniform mat4 view_matrix;
            uniform mat4 model_matrix;
            uniform vec3 color;
            in vec3 in_position;
            in vec3 in_normal;
            in vec2 in_uv;
            out vec2 uv;
            void main(void) { 
                uv = in_uv;
                gl_Position = projection_matrix * view_matrix * model_matrix * vec4(in_position, 1); 
            }");

        private string FragmentShaderSource => GetShader(true, @"
            uniform sampler2D texture;
            in vec2 uv;
            out vec4 fragment;
            uniform vec3 color;
            void main(void) { fragment = texture2D(texture, uv);}");

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct Vertex
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 UV0;
        }

        private struct Mesh
        {
            public Vertex[] Vertices;
            public ushort[] Indices;
        }

        private Mesh mainMesh;

        public static void CalculateNormals(ReadOnlySpan<Vector3> vertexData, ReadOnlySpan<ushort> elementData, Span<Vector3> normalData)
        {
            if ((elementData.Length % 3) != 0)
            {
                throw new ArgumentException($"Expected {nameof(elementData)} to be a multiple of 3 as each triangle consists of 3 points.", nameof(elementData));
            }
            if (vertexData.Length != normalData.Length)
            {
                throw new ArgumentException($"Expected {nameof(vertexData)} and {nameof(normalData)} to have the same length.", nameof(vertexData));
            }

            for (int i = 0; i < elementData.Length; i += 3)
            {
                int cornerAIndex = (int)elementData[i + 0];
                int cornerBIndex = (int)elementData[i + 1];
                int cornerCIndex = (int)elementData[i + 2];

                Vector3 cornerA = vertexData[cornerAIndex];
                Vector3 cornerB = vertexData[cornerBIndex];
                Vector3 cornerC = vertexData[cornerCIndex];

                Vector3 ab = cornerB - cornerA;
                Vector3 ac = cornerC - cornerA;

                Vector3 normal = Vector3.Normalize(Vector3.Cross(ab, ac));

                normalData[cornerAIndex] += normal;
                normalData[cornerBIndex] += normal;
                normalData[cornerCIndex] += normal;
            }

            for (int i = 0; i < normalData.Length; i++) normalData[i] = Vector3.Normalize(normalData[i]);
        }

        public static Vector3[] CalculateNormals(ReadOnlySpan<Vector3> vertexData, ReadOnlySpan<ushort> elementData)
        {
            Vector3[] normalData = new Vector3[vertexData.Length];
            CalculateNormals(vertexData, elementData, normalData);
            return normalData;
        }

        private Vertex[] ConstructVertexList(ReadOnlySpan<Vector3> vertexData, ReadOnlySpan<Vector3> normalData)
        {
            if (vertexData.Length != normalData.Length)
            {
                throw new ArgumentException($"Expected {nameof(vertexData)} and {nameof(normalData)} to have the same length.", nameof(vertexData));
            }
            Vertex[] vertices = new Vertex[vertexData.Length];
            for (int i = 0; (i < vertexData.Length) && (i < normalData.Length); i++)
            {
                vertices[i] = new Vertex
                {
                    Position = vertexData[i],
                    Normal = normalData[i]
                };
            }
            return vertices;
        }

        private Vertex[] ConstructVertexList(ReadOnlySpan<Vector3> vertexData, ReadOnlySpan<Vector2> uvData, ReadOnlySpan<Vector3> normalData)
        {
            if (vertexData.Length != normalData.Length && vertexData.Length != uvData.Length)
            {
                throw new ArgumentException($"Expected {nameof(vertexData)} ,{nameof(normalData)} and {nameof(uvData)} to have the same length.", nameof(vertexData));
            }
            Vertex[] vertices = new Vertex[vertexData.Length];
            for (int i = 0; (i < vertexData.Length) && (i < normalData.Length); i++)
            {
                vertices[i] = new Vertex
                {
                    Position = vertexData[i],
                    Normal = normalData[i],
                    UV0 = uvData[i],
                };
            }
            return vertices;
        }

        private Mesh CreateQuadWithNormals(Vector2 location, Vector2 size, Vector2 uvMin, Vector2 uvMax)
        {
            Vector3[] vertex = new Vector3[] { 
                new Vector3(location.X, location.Y, 0), 
                new Vector3(location.X + size.X, location.Y, 0),
                new Vector3(location.X + size.X, location.Y + size.Y, 0), 
                new Vector3(location.X, location.Y + size.Y, 0) };
            Vector2[] uvCoords = new Vector2[] {
                uvMax,
                new Vector2(uvMin.X, uvMax.Y),
                uvMin,
                new Vector2(uvMax.X, uvMin.Y),
            };
            ushort[] indices = new ushort[] { 0, 1, 2, 2, 3, 0 };
            Vector3[] normals = CalculateNormals(vertex, indices);
            return new Mesh
            {
                Vertices = ConstructVertexList(vertex, uvCoords, normals),
                Indices = indices,
            };
        }

        private Mesh CreateCube(Vector3 min, Vector3 max)
        {
            Vector3[] vertex = new Vector3[] {
                new Vector3(max.X, max.Y, min.Z),   new Vector3(min.X, max.Y, min.Z),   new Vector3(min.X, max.Y, max.Z),    new Vector3(max.X, max.Y, max.Z),
                new Vector3(max.X, min.Y, max.Z),   new Vector3(min.X, min.Y, max.Z),   new Vector3(min.X, min.Y, min.Z),    new Vector3(max.X, min.Y, min.Z),
                new Vector3(max.X, max.Y, max.Z),   new Vector3(min.X, max.Y, max.Z),   new Vector3(min.X, min.Y, max.Z),    new Vector3(max.X, min.Y, max.Z),
                new Vector3(max.X, min.Y, min.Z),   new Vector3(min.X, min.Y, min.Z),   new Vector3(min.X, max.Y, min.Z),    new Vector3(max.X, max.Y, min.Z),
                new Vector3(min.X, max.Y, max.Z),   new Vector3(min.X, max.Y, min.Z),   new Vector3(min.X, min.Y, min.Z),    new Vector3(min.X, min.Y, max.Z),
                new Vector3(max.X, max.Y, min.Z),   new Vector3(max.X, max.Y, max.Z),   new Vector3(max.X, min.Y, max.Z),    new Vector3(max.X, min.Y, min.Z)
            };

            Vector2[] uvCoords = new Vector2[]
            {
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)            };

            ushort[] indices = new ushort[] {
                0, 1, 2,
                0, 2, 3,
                4, 5, 6,
                4, 6, 7,
                8, 9, 10,
                8, 10, 11,
                12, 13, 14,
                12, 14, 15,
                16, 17, 18,
                16, 18, 19,
                20, 21, 22,
                20, 22, 23
            };
            Vector3[] normals = CalculateNormals(vertex, indices);

            return new Mesh
            {
                Vertices = ConstructVertexList(vertex, uvCoords, normals),
                Indices = indices,
            };
        }

        public OpenGlPageControl()
        {
            if (!Design.IsDesignMode)
            {
                mouseAdapter = new MouseAdapter(this);
                mouseAdapter.Controllers.Add(new CameraMouseController(this, Avalonia.Input.KeyModifiers.Control));
                mouseAdapter.Activate();

                InfoVisible = CrossPlatformAppSystem.Instance.Services.Get<CrossPlatformAppConfiguration>().ShowGraphicsAdapterInfo;
            }
        }

        private static void CheckError(string context, GlInterface gl)
        {
            int err;
            while ((err = gl.GetError()) != GL_NO_ERROR)
            {
                TraceApplication.Trace.Always("Context[{0}] Error[0x{1:x8}]", context, err);
            }
        }


        protected unsafe override void OnOpenGlInit(GlInterface GL, int fb)
        {
            // set internal vulkan camera object
            camera = new SceneCamera();
            camera.LookAtOrtho(Box3D.CreateCenterBox(new Point3D(0, 0, 0), 180, 120, 2048));


            CheckError("OnOpenGlInit(): called", GL);
            glInterface = GL;
            glExt = new GlExtrasInterface(GL);

            Info = $"Renderer: {GL.GetString(GL_RENDERER)} Version: {GL.GetString(GL_VERSION)}";
            glData = new GLData();

            // Load the source of the vertex shader and compile it.
            vertexShader = GL.CreateShader(GL_VERTEX_SHADER);
            TraceApplication.Trace.Always("CreateShader GL_VERTEX_SHADER Error[{0}]", GL.CompileShaderAndGetError(vertexShader, VertexShaderSource));

            // Load the source of the fragment shader and compile it.
            fragmentShader = GL.CreateShader(GL_FRAGMENT_SHADER);
            TraceApplication.Trace.Always("CreateShader GL_FRAGMENT_SHADER Error[{0}]", GL.CompileShaderAndGetError(fragmentShader, FragmentShaderSource));

            // Create the shader program, attach the vertex and fragment shaders and link the program.
            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            const int positionLocation = 0;
            const int normalLocation = 1;
            const int uvLocation = 2;
            GL.BindAttribLocationString(shaderProgram, positionLocation, "in_position");
            GL.BindAttribLocationString(shaderProgram, normalLocation, "in_normal");
            GL.BindAttribLocationString(shaderProgram, uvLocation, "in_uv");
            TraceApplication.Trace.Always("BindAttribLocationString Error[{0}]", GL.LinkProgramAndGetError(shaderProgram));

            // Create the vertex buffer object (VBO) for the vertex data.
            vertexBufferObject = GL.GenBuffer();
            // Bind the VBO and copy the vertex data into it.
            GL.BindBuffer(GL_ARRAY_BUFFER, vertexBufferObject);
            CheckError("BindBuffer GL_ARRAY_BUFFER", GL);

            Int2 currentImageBounds = new Int2((int)Bounds.Width, (int)Bounds.Height);
            Int2 currentViewportBounds = currentImageBounds; 

            Vector2 uvMin = new Vector2(0, 0);
            Vector2 uvMax = new Vector2(1, 1);
            InitVulcan(currentImageBounds, CrossPlatformAppSystem.Instance.Services.Get<CrossPlatformAppConfiguration>().UseSharedTexture);
            float aspect;
            if (currentViewportBounds.Width > currentViewportBounds.Height)
            {
                aspect = (float)currentViewportBounds.Width / (float)currentViewportBounds.Height;
            } else
            {
                aspect = (float)currentViewportBounds.Height / (float)currentViewportBounds.Width;
            }


            mainMesh = CreateQuadWithNormals(new Vector2(-10 * aspect, -10), new Vector2(20 * aspect, 20), uvMin, uvMax);

            var vertexSize = Marshal.SizeOf<Vertex>();
            fixed (void* pdata = mainMesh.Vertices)
                GL.BufferData(GL_ARRAY_BUFFER, new IntPtr(mainMesh.Vertices.Length * vertexSize), new IntPtr(pdata), GL_STATIC_DRAW);

            // Bind the Indices and copy the index data into it.
            indexBufferObject = GL.GenBuffer();
            GL.BindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBufferObject);
            CheckError("BindBuffer GL_ELEMENT_ARRAY_BUFFER", GL);
            fixed (void* pdata = mainMesh.Indices)
                GL.BufferData(GL_ELEMENT_ARRAY_BUFFER, new IntPtr(mainMesh.Indices.Length * sizeof(ushort)), new IntPtr(pdata), GL_STATIC_DRAW);
            CheckError("BufferData", GL);


            vertexArrayObject = glExt.GenVertexArray();
            glExt.BindVertexArray(vertexArrayObject);
            CheckError("BindVertexArray", GL);
            GL.VertexAttribPointer(positionLocation, 3, GL_FLOAT, 0, vertexSize, IntPtr.Zero);
            GL.VertexAttribPointer(normalLocation, 3, GL_FLOAT, 0, vertexSize, new IntPtr(12));
            GL.VertexAttribPointer(uvLocation, 2, GL_FLOAT, 0, vertexSize, new IntPtr(24));
            GL.EnableVertexAttribArray(positionLocation);
            GL.EnableVertexAttribArray(normalLocation);
            GL.EnableVertexAttribArray(uvLocation);
            CheckError("EnableVertexAttribArray", GL);

            if (renderTarget.UseSharedTexture)
            {
                // semaphores and shared memory objects...
                glExt.GenSemaphoresExt(1, glData.glReady);
                CheckError("GenSemaphoresExt1", GL);
                glExt.GenSemaphoresExt(1, glData.glComplete);
                CheckError("GenSemaphoresExt2", GL);
                glExt.CreateMemoryObjectsExt(1, glData.mem);
                CheckError("CreateMemoryObjectsExt", GL);
                // Link API semaphores and memory together...
                if(Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    glExt.ImportSemaphoreWin32HandleEXT((uint)glData.glReady[0], GlConsts.GL_HANDLE_TYPE_OPAQUE_WIN32_EXT, renderTarget.GlReadyHandle);
                    CheckError("ImportSemaphoreWin32EXT1", GL);
                    glExt.ImportSemaphoreWin32HandleEXT((uint)glData.glComplete[0], GlConsts.GL_HANDLE_TYPE_OPAQUE_WIN32_EXT, renderTarget.GlCompleteHandle);
                    CheckError("ImportSemaphoreWin32EXT2", GL);
                    glExt.ImportMemoryWin32HandleEXT((uint)glData.mem[0], renderTarget.MemoryImageShareSize, GlConsts.GL_HANDLE_TYPE_OPAQUE_WIN32_EXT, renderTarget.SharedMemoryHandle);
                    CheckError("ImportMemoryWin32HandleEXT", GL);
                }
                else 
                {
                    glExt.ImportSemaphoreFdEXT(glData.glReady[0], GlConsts.GL_HANDLE_TYPE_OPAQUE_FD_EXT, renderTarget.GlReadyLinuxHandle);
                    CheckError("ImportSemaphoreFdEXT1", GL);
                    glExt.ImportSemaphoreFdEXT(glData.glComplete[0], GlConsts.GL_HANDLE_TYPE_OPAQUE_FD_EXT, renderTarget.GlCompleteLinuxHandle);
                    CheckError("ImportSemaphoreFdEXT2", GL);
                    glExt.ImportMemoryFdEXT((uint)glData.mem[0], renderTarget.MemoryImageShareSize, GlConsts.GL_HANDLE_TYPE_OPAQUE_FD_EXT, renderTarget.SharedLinuxMemoryHandle);
                    CheckError("ImportMemoryFdEXT", GL);
                }

                // Create the texture for the FBO color attachment.
                // This only reserves the ID, it doesn't allocate memory
                GL.GenTextures(1, glData.sharedTexture);
                CheckError("GenTextures", GL);
                GL.ActiveTexture(GL_TEXTURE0);
                CheckError("ActiveTexture", GL);
                GL.BindTexture(GL_TEXTURE_2D, glData.sharedTexture[0]);
                CheckError("BindTexture", GL);
                GL.TexParameteri(GL_TEXTURE_2D, GL_TEXTURE_TILING_EXT, GL_OPTIMAL_TILING_EXT);
                CheckError("TexParameteri", GL);

                glExt.TexStorageMem2DEXT(GL_TEXTURE_2D, 1, GL_RGBA8, currentViewportBounds.Width, currentViewportBounds.Height, (uint)glData.mem[0], 0);
                CheckError("TextureStorageMem2DEXT", GL);

                GL.BindTexture(GL_TEXTURE_2D, 0);
            }

        }

        protected override void OnOpenGlDeinit(GlInterface GL, int fb)
        {
            // Unbind everything
            GL.BindBuffer(GL_ARRAY_BUFFER, 0);
            GL.BindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
            glExt.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all resources.
            GL.DeleteBuffers(2, new[] { vertexBufferObject, indexBufferObject });
            glExt.DeleteVertexArrays(1, new[] { vertexArrayObject });
            GL.DeleteProgram(shaderProgram);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            if (renderTarget.UseSharedTexture)
            {
                GL.DeleteTextures(1, glData.sharedTexture);
                glExt.DeleteSemaphoresEXT(1, glData.glReady);
                glExt.DeleteSemaphoresEXT(1, glData.glComplete);
            }
            CommonUtils.DisposeAndSetNull(ref renderTarget);
        }

        protected override unsafe void OnOpenGlRender(GlInterface gl, int fb)
        {
            Stopwatch elapsedTime = Stopwatch.StartNew();

            if (renderTarget.UseSharedTexture)
            {
                int[] dstLayout = { GL_LAYOUT_SHADER_READ_ONLY_EXT };
                glExt.SignalSemaphoreEXT((uint)glData.glReady[0], 0, null, 1, glData.sharedTexture, dstLayout);
                CheckError("SignalSemaphoreEXT", gl);
                gl.Flush();
            }

            RenderVulcan();

            if (renderTarget.UseSharedTexture)
            {
                // Wait (on the GPU side) for the Vulkan semaphore to be signaled (finished compute)
                int[] srcLayout = { GL_LAYOUT_COLOR_ATTACHMENT_EXT };
                glExt.WaitSemaphoreEXT((uint)glData.glComplete[0], 0, null, 1, glData.sharedTexture, srcLayout);
                CheckError("WaitSemaphoreEXT", gl);
                gl.Flush();
                gl.BindTexture(GL_TEXTURE_2D, glData.sharedTexture[0]);
                CheckError("BindTexture", gl);
            }
            else
            {
                if (glData.destinationGLTexture[0] == 0)
                {
                    glInterface.GenTextures(1, glData.destinationGLTexture);
                    glExt.PixelStorei(GL_UNPACK_ALIGNMENT, 1);
                    glInterface.BindTexture(GL_TEXTURE_2D, glData.destinationGLTexture[0]);
                }
                glInterface.TexImage2D(GL_TEXTURE_2D, 0, GL_RGBA8, renderTarget.ImageDimension.Width, renderTarget.ImageDimension.Height, 0, GL_RGBA, GL_UNSIGNED_BYTE, (IntPtr)renderTarget.RawScreenBuffer);
                CheckError("TexImage2D", glInterface);

                glInterface.BindTexture(GL_TEXTURE_2D, glData.destinationGLTexture[0]);
                CheckError("BindTexture", glInterface);
                glInterface.TexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
                glInterface.TexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
            }

            gl.ClearColor(0, 0, 0, 0);
            gl.Clear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
            gl.Enable(GL_DEPTH_TEST);
            gl.Viewport(0, 0, (int)Bounds.Width, (int)Bounds.Height);


            gl.BindBuffer(GL_ARRAY_BUFFER, vertexBufferObject);
            gl.BindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBufferObject);
            glExt.BindVertexArray(vertexArrayObject);
            gl.UseProgram(shaderProgram);
            CheckError("UseProgram", gl);

            Matrix4x4 matProjection = Matrix4x4.CreatePerspectiveFieldOfView((float)(Math.PI / 4), (float)(Bounds.Width / Bounds.Height), 0.01f, 1000);
            Matrix4x4 matView = Matrix4x4.CreateLookAt(new Vector3(0, 0, 25), new Vector3(), new Vector3(0, -1, 0));
            Matrix4x4 matModel = Matrix4x4.Identity;  //Matrix4x4.CreateFromYawPitchRoll(_yaw, _pitch, _roll);
            Vector3 color = new Vector3(1, 0, 0);

            var modelMatLocator = gl.GetUniformLocationString(shaderProgram, "model_matrix");
            var viewMatLocator = gl.GetUniformLocationString(shaderProgram, "view_matrix");
            var projectionMatLocator = gl.GetUniformLocationString(shaderProgram, "projection_matrix");
            var colorLocator = gl.GetUniformLocationString(shaderProgram, "color");

            gl.UniformMatrix4fv(modelMatLocator, 1, false, &matModel);
            gl.UniformMatrix4fv(viewMatLocator, 1, false, &matView);
            gl.UniformMatrix4fv(projectionMatLocator, 1, false, &matProjection);
            glExt.Uniform3f(colorLocator, 1.0f, 0, 0);
            CheckError("Uniform3f", gl);


            gl.DrawElements(GL_TRIANGLES, mainMesh.Indices.Length, GL_UNSIGNED_SHORT, IntPtr.Zero);
            CheckError("DrawElements", gl);
            var t = elapsedTime.Elapsed.TotalMilliseconds;
            Info = $"Renderer: {glInterface.GetString(GL_RENDERER)} Version: {glInterface.GetString(GL_VERSION)}, Vulkan Renderer elapsed:{vulcanRendererElapsedTimeInMs} ms, complete time:{t} ms";

            //TraceApplication.Trace.Always("OnOpenGlRender: elapsed ms: " + elapsedTime.Elapsed);
            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Background);
        }

        private void InitVulcan(Int2 bounds, bool useSharedTexture)
        {
            bool enableDebug = CrossPlatformAppSystem.Instance.Services.Get<CrossPlatformAppConfiguration>().DebugLayerEnabled;
            enableDebug = true;
            VulkanInstance instance = VulkanInstance.
               New().
               SetApplication("VulkanTest", new VersionInfo(0, 0, 1)).
               EnableDebug(enableDebug).
               ConfigureOpenGLInterop().
               ConfigurSwapChain(). // TODO @OH: check if this line can be removed (OpenGL interop does not present via swap chain)
               Build();

            Device device = Device.New(instance).
                            SetPhysicalDevice(instance.PhysicalDevices.First()).
                            ConfigureOpenGLInterop().
                            ConfigureSwapChain(). // TODO @OH: check if this line can be removed (OpenGL interop does not present via swap chain)
                            Build();

            VkSampleCountFlags sampleCountFlag = VkSampleCountFlags._8;
            VkSurfaceFormatKHR format = new VkSurfaceFormatKHR() { colorSpace = VkColorSpaceKHR.SrgbNonlinearKHR, format = VkFormat.R8g8b8a8Unorm };
            // we assume 8Bit sampling! Should be calculated by Device.
            renderContext = new RenderingContext(device, format, sampleCountFlag, true);

            // RenderTarget: Last Parameter: 
            // True: Sharable texture with usage of external semaphore and memory objects
            // False: Copied texture with help of CPU, loaded up by openGl. (most compatible)
            this.renderTarget = new OGlInteropRenderTarget(this.renderContext.Device, new Int2((int)bounds.Width, (int)bounds.Height), renderContext.RenderPass, sampleCountFlag, useSharedTexture);

            // Note: EndBufferCall must not called before presenter is called!
            renderContext.DisableEndCommandBufferCall = !this.renderTarget.UseSharedTexture;

            this.testRenderer = new TestRenderer(renderContext);
        }

        private void RenderVulcan()
        {

            //_renderTarget.EnsureSize(size);
            testRenderer.Render(renderTarget, camera);
        }


        public class GlExtrasInterface : GlInterfaceBase<GlInterface.GlContextInfo>
        {
            public GlExtrasInterface(GlInterface gl) : base(gl.GetProcAddress, gl.ContextInfo)
            {
            }

            public delegate void GlDeleteVertexArrays(int count, int[] buffers);
            [GlMinVersionEntryPoint("glDeleteVertexArrays", 3, 0)]
            [GlExtensionEntryPoint("glDeleteVertexArraysOES", "GL_OES_vertex_array_object")]
            public GlDeleteVertexArrays DeleteVertexArrays { get; }

            public delegate void GlBindVertexArray(int array);
            [GlMinVersionEntryPoint("glBindVertexArray", 3, 0)]
            [GlExtensionEntryPoint("glBindVertexArrayOES", "GL_OES_vertex_array_object")]
            public GlBindVertexArray BindVertexArray { get; }
            public delegate void GlGenVertexArrays(int n, int[] rv);

            [GlMinVersionEntryPoint("glGenVertexArrays", 3, 0)]
            [GlExtensionEntryPoint("glGenVertexArraysOES", "GL_OES_vertex_array_object")]
            public GlGenVertexArrays GenVertexArrays { get; }

            public delegate void GlGenSemaphoresExt(int n, uint[] semaphores);
            [GlMinVersionEntryPoint("glGenSemaphoresEXT", 3, 0)]
            [GlExtensionEntryPoint("glGenSemaphoresEXT", "GL_EXT_semaphore")]
            public GlGenSemaphoresExt GenSemaphoresExt { get; }

            public delegate void GlCreateMemoryObjectsExt(int n, int[] objects);
            [GlMinVersionEntryPoint("glCreateMemoryObjectsEXT", 3, 0)]
            [GlExtensionEntryPoint("glCreateMemoryObjectsEXT", "GL_EXT_memory_object")]
            public GlCreateMemoryObjectsExt CreateMemoryObjectsExt { get; }


            public delegate void glUniform3f(int location, float value1, float value2, float value3);
            [GlEntryPoint("glUniform3f")]
            public glUniform3f Uniform3f { get; }

            public delegate void glPixelStorei(int alignment, int param);
            [GlEntryPoint("glPixelStorei")]
            public glPixelStorei PixelStorei { get; }

            public delegate void glImportSemaphoreFdEXT(uint semaphore, int handleType, int fd);
            [GlEntryPoint("glImportSemaphoreFdEXT")]
            [GlOptionalEntryPoint]
            public glImportSemaphoreFdEXT ImportSemaphoreFdEXT { get; }

            public delegate void glImportMemoryFdEXT(uint memory, UInt64 size, int handleType, int fd);
            [GlEntryPoint("glImportMemoryFdEXT")]
            [GlOptionalEntryPoint]
            public glImportMemoryFdEXT ImportMemoryFdEXT { get; }

            public delegate void glImportSemaphoreWin32HandleEXT(uint semaphore, int handleType, HANDLE handle);
            [GlEntryPoint("glImportSemaphoreWin32HandleEXT")]
            [GlOptionalEntryPoint]
            public glImportSemaphoreWin32HandleEXT ImportSemaphoreWin32HandleEXT { get; }

            public delegate void glImportMemoryWin32HandleEXT(uint memory, UInt64 size, int handleType, HANDLE handle);
            [GlEntryPoint("glImportMemoryWin32HandleEXT")]
            [GlOptionalEntryPoint]
            public glImportMemoryWin32HandleEXT ImportMemoryWin32HandleEXT { get; }

            public delegate void glTexStorageMem2DEXT(int target, int levels, int internalFormat, int width, int height, uint memory, UInt64 offset);
            [GlEntryPoint("glTexStorageMem2DEXT")]
            public glTexStorageMem2DEXT TexStorageMem2DEXT { get; }

            public delegate void glWaitSemaphoreEXT(uint semaphore, uint numBufferBarriers, uint[] buffers, uint numTextureBarriers, int[] textures, int[] srcLayouts);
            [GlEntryPoint("glWaitSemaphoreEXT")]
            public glWaitSemaphoreEXT WaitSemaphoreEXT { get; }

            public delegate void glSignalSemaphoreEXT(uint semaphore, uint numBufferBarriers, uint[] buffers, uint numTextureBarriers, int[] textures, int[] dstLayouts);
            [GlEntryPoint("glSignalSemaphoreEXT")]
            public glSignalSemaphoreEXT SignalSemaphoreEXT { get; }

            public delegate void glDeleteSemaphoresEXT(int n, uint[] semaphores);
            [GlEntryPoint("glDeleteSemaphoresEXT")]
            public glDeleteSemaphoresEXT DeleteSemaphoresEXT { get; }

            public int GenVertexArray()
            {
                var rv = new int[1];
                GenVertexArrays(1, rv);
                return rv[0];
            }
        }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS8604 // Possible null reference argument.
}
