// ===========================================================================\r
//                          B I T S   O F   N A T U R E\r
// ===========================================================================\r
//  This document contains proprietary information. It is the exclusive\r
//  confidential property of Stryker Corporation and its affiliates.\r
//  \r
//  Copyright (c) 2022 Stryker\r
// ===========================================================================

using System;
using BitsOfNature.Core.Geometry3D;
using BitsOfNature.Core.Grid;
using BitsOfNature.Core.Imaging.Colors;
using BitsOfNature.Core.IO.FileFormats;
using BitsOfNature.Core.Mathematics;
using BitsOfNature.Core.Mesh;
using BitsOfNature.Rendering.Vulkan.Engine;
using BitsOfNature.Rendering.Vulkan.Engine.Renderers;
using BitsOfNature.Rendering.Vulkan.Memory;
using BitsOfNature.Rendering.Vulkan.SceneGraph;
using BitsOfNature.Rendering.Vulkan.Utils;

namespace BitsOfNature.Rendering.Vulkan.Wpf.DXInterop
{
    /// <summary>
    ///     WPF Testrenderer, used to check correct rendering of vulkan content
    /// </summary>
    public class TestRendererWpf
    {
        #region Private Attributes
        /// <summary>
        ///     Determines if testrenderer is initialized
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        ///     The renderable mesh
        /// </summary>
        private RenderableMesh _boneMeshData;

        /// <summary>
        ///     The reflextion map for mesh
        /// </summary>
        private ReflectionMap _reflectionMap;
        
        /// <summary>
        ///     Used rendertarget for renderer
        /// </summary>
        private readonly DxInteropRenderTarget _renderTarget;

        /// <summary>
        ///     Used scenecamera
        /// </summary>
        private SceneCamera _camera;
        #endregion

        #region Public Properties
        /// <summary>
        ///     Returns current camera
        /// </summary>
        public SceneCamera Camera { get => _camera; set => _camera = value; }

        /// <summary>
        ///     Used viewport
        /// </summary>
        public GridRect Viewport;
        #endregion

        #region Construction
        /// <summary>
        ///     Creates appropriate testrenderer
        /// </summary>
        /// <param name="renderTarget">
        ///     Used rendertarget
        /// </param>
        public TestRendererWpf(DxInteropRenderTarget renderTarget)
        {
            this._renderTarget = renderTarget;
            _camera = new SceneCamera();
            _camera.LookAtOrtho(Box3D.CreateCenterBox(new Point3D(0, 0, 0), 200, 220, 2048));
            Viewport = new GridRect(0, 0, renderTarget.Size.Width, renderTarget.Size.Height);
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Called to render the scene
        /// </summary>
        public void Render()
        {
            if (_reflectionMap == null) { _reflectionMap = ReflectionMap.LoadDefault(); }
            DirectionalLight[] lights = new[] { new DirectionalLight(_camera.ViewDirection) };

            _renderTarget.RenderingContext.Begin(_renderTarget, Argb.Black, this.Viewport);
            GraphicsRenderPass pass =  _renderTarget.RenderingContext.UseGraphicsPass(_camera, lights, _reflectionMap);

            if (!_isInitialized) { Init(); }

            RenderScene(pass);

            _renderTarget.RenderingContext.End();
        }
        #endregion

        #region Private Methods
        // TODO (FH): temporary comment for sonar exclusion check
        /// <summary>
        ///     Inititalizes internal object structures
        /// </summary>
        /// <param name="pass">
        ///     Current renderpass
        /// </param>
        private void Init()
        {
            Mesh3D mesh = MeshIO.Load(AppDomain.CurrentDomain.BaseDirectory + "_Sandbox/Pelvis.mbs");

            mesh.TransformBy(Pose3D.CreateRotation(CartesianAxis.X, 0.5 * Math.PI));
            mesh.RepositionVerticesAroundCenter();
            _boneMeshData = new RenderableMesh(mesh, MemoryUsageHint.Gpu);
            _boneMeshData.Attributes.Material = MaterialUtils.CreateBoneMaterial();
            _isInitialized = true;
        }

        /// <summary>
        ///     Renders the scene
        /// </summary>
        /// <param name="pass">
        ///     Current pass
        /// </param>
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
        #endregion
    }
}
