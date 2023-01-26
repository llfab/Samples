// ===========================================================================\r
//                          B I T S   O F   N A T U R E\r
// ===========================================================================\r
//  This document contains proprietary information. It is the exclusive\r
//  confidential property of Stryker Corporation and its affiliates.\r
//  \r
//  Copyright (c) 2022 Stryker\r
// ===========================================================================

using System;
using BitsOfNature.Core.Utils;
using D3D9 = SharpDX.Direct3D9;

namespace BitsOfNature.Rendering.Vulkan.Wpf.DXInterop
{
    /// <summary>
    ///     Creates a D3DDevice9 texture as rendertarget and uses it as a maintexture/container for DXGI resource
    ///     for vulkan and as D3D9 texture for D3DImage backbuffer
    /// </summary>
    internal class MainTexture : IDisposable
    {
        #region Private Attributes
        /// <summary>
        ///     D3D9 texture, WPF texture container 
        /// </summary>
#pragma warning disable S4487 // Unread "private" fields should be removed: needed here for sharedhandle which is linked with texture
        private  D3D9.Texture _texture;
#pragma warning restore S4487 
        #endregion
        
        #region Public Properties
        /// <summary>
        ///     Indicates if creation of main texture has errors
        /// </summary>
        public bool HasError { get; }

        /// <summary>
        ///     Returns DX9 surface handle (SharedHandle) of d3d9(ex) rendertarget texture
        /// </summary>
        public IntPtr SurfaceHandle { get; }
        #endregion

        #region Construction
        /// <summary>
        ///     Creates a d3d9(ex) texture with given device, used as main texture to interop between DX11/DXGI and WPF
        /// </summary>
        /// <param name="device">
        ///     D3d9(ex) device used to create rendertarget texture
        /// </param>
        /// <param name="size">
        ///     Given texture size
        /// </param>
        public MainTexture(D3D9.DeviceEx device, Int2 size)
        {
            HasError = false;
            IntPtr sharedHandle = new IntPtr();

            _texture = new D3D9.Texture(device, size.Width, size.Height, 1,
                D3D9.Usage.RenderTarget, D3D9.Format.A8R8G8B8, D3D9.Pool.Default, ref sharedHandle);
            SurfaceHandle = sharedHandle;
            if (SurfaceHandle.ToInt64() == 0)
            {
                HasError = true;
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        ///     Disposes interop helper dependencies
        /// </summary>
        public void Dispose()
        {
            CommonUtils.DisposeAndSetNull(ref _texture);
        }
        #endregion

    }
}
