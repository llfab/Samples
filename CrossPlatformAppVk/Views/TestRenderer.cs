using System;
using BitsOfNature.Core.Geometry3D;
using BitsOfNature.Core.Grid;
using BitsOfNature.Core.Imaging.Colors;
using BitsOfNature.Core.IO.FileFormats;
using BitsOfNature.Core.Mesh;
using BitsOfNature.Rendering.Vulkan.Engine;
using BitsOfNature.Rendering.Vulkan.Engine.Renderers;
using BitsOfNature.Rendering.Vulkan.Memory;
using BitsOfNature.Rendering.Vulkan.SceneGraph;
using BitsOfNature.Rendering.Vulkan.Utils;
using BitsOfNature.Core.Mathematics;

namespace CrossPlatformAppVk.Views
{
    public class TestRenderer
    {
        private bool isInitialized;
        private RenderingContext context;
        private RenderableMesh boneMeshData;
        private ReflectionMap reflectionMap;
    
        public TestRenderer()
        {
        }
        
        public TestRenderer(RenderingContext context)
        {
            SetContext(context);
        }

        public void SetContext(RenderingContext context)
        {
            this.context = context;
        }
        
        public void Render(RenderTarget renderTarget, SceneCamera camera, GridRect viewport)
        {
            if (reflectionMap == null)
            {
                reflectionMap = ReflectionMap.LoadDefault();
            }
            DirectionalLight[] lights = new[] { new DirectionalLight(camera.ViewDirection) };

            context.Begin(renderTarget, Argb.Black, viewport);
            GraphicsRenderPass pass = context.UseGraphicsPass(camera, lights, reflectionMap);

            if (!isInitialized) { Init(pass); }

            RenderScene(pass);

            context.End();
        }
        
        public void Init(GraphicsRenderPass pass)
        {
            Mesh3D mesh = MeshIO.Load(AppDomain.CurrentDomain.BaseDirectory + "_Sandbox/Pelvis.mbs");
            mesh.TransformBy(Pose3D.CreateRotation(CartesianAxis.X, 0.5 * Math.PI));
            mesh.RepositionVerticesAroundCenter();
            boneMeshData = new RenderableMesh(mesh, MemoryUsageHint.Gpu);
            boneMeshData.Attributes.Material = MaterialUtils.CreateBoneMaterial();

            isInitialized = true;
        }
        
        private void RenderScene(GraphicsRenderPass pass)
        {
            IBackground bg = GridBackground.CreateDefault();
            bg.Render(pass);

            MeshRenderer mrender = new MeshRenderer(pass);
            Matrix matrix = HMatrix3D.CreateIdentity();
            MeshRendererItem item = boneMeshData.Prepare(pass, matrix, RenderState.Opaque(), true, 0)[0];

            mrender.Add(item);
            mrender.Render(ItemTransparencyGroup.Opaque);
            mrender.Render(ItemTransparencyGroup.Transparent);
        }

    }
}

