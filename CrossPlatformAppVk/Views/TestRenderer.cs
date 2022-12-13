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
        private bool _isInitialized;
        private RenderingContext _context;
        private RenderableMesh _boneMeshData;
        private ReflectionMap _reflectionMap;
    
        public TestRenderer(RenderingContext context)
        {
            SetContext(context);
        }

        private void SetContext(RenderingContext context)
        {
            this._context = context;
        }
        
        public void Render(RenderTarget renderTarget, SceneCamera camera, GridRect viewport)
        {
            if (_reflectionMap == null)
            {
                _reflectionMap = ReflectionMap.LoadDefault();
            }
            DirectionalLight[] lights = new[] { new DirectionalLight(camera.ViewDirection) };

            _context.Begin(renderTarget, Argb.Black, viewport);
            GraphicsRenderPass pass = _context.UseGraphicsPass(camera, lights, _reflectionMap);

            if (!_isInitialized) { Init(pass); }

            RenderScene(pass);

            _context.End();
        }

        private void Init(GraphicsRenderPass pass)
        {
            Mesh3D mesh = MeshIO.Load(AppDomain.CurrentDomain.BaseDirectory + "_Sandbox/Pelvis.mbs");
            mesh.TransformBy(Pose3D.CreateRotation(CartesianAxis.X, 0.5 * Math.PI));
            mesh.RepositionVerticesAroundCenter();
            _boneMeshData = new RenderableMesh(mesh, MemoryUsageHint.Gpu);
            _boneMeshData.Attributes.Material = MaterialUtils.CreateBoneMaterial();

            _isInitialized = true;
        }
        
        private void RenderScene(GraphicsRenderPass pass)
        {
            IBackground bg = GridBackground.CreateDefault();
            bg.Render(pass);

            MeshRenderer mrender = new MeshRenderer(pass);
            Matrix matrix = HMatrix3D.CreateIdentity();
            MeshRendererItem item = _boneMeshData.Prepare(pass, matrix, RenderState.Opaque(), true, 0)[0];

            mrender.Add(item);
            mrender.Render(ItemTransparencyGroup.Opaque);
            mrender.Render(ItemTransparencyGroup.Transparent);
        }

    }
}

