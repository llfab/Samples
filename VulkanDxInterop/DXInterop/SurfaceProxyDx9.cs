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
    ///     SurfaceProxy for DX9 WPF texture, which is only a proxy of given MainTexture
    /// </summary>
    internal class SurfaceProxyDx9 : IDisposable
    {
        #region Private Attributes

        private D3D9.Texture _resource;
        #endregion

        #region Public Properties

        /// <summary>
        ///     Given proxy texture for WPF D3DImage backbuffer target
        /// </summary>
        public D3D9.Texture Resource => _resource;
        #endregion

        #region Construction
        /// <summary>
        ///     Constructor to retrieve DX9 proxy resource of mainTexture for WPF D3DImage backbuffer target
        /// </summary>
        /// <param name="device">
        ///     Given D3D9ex device
        /// </param>
        /// <param name="size">
        ///     Given size of texture
        /// </param>
        /// <param name="mainTexture">
        ///     DX9ex rendertarget mainTexture 
        /// </param>
        public SurfaceProxyDx9(D3D9.DeviceEx device, Int2 size, MainTexture mainTexture)
        {
            IntPtr handle = mainTexture.SurfaceHandle;
            this._resource = new D3D9.Texture(device,  size.Width, size.Height, 1, D3D9.Usage.RenderTarget, D3D9.Format.A8R8G8B8,
                D3D9.Pool.Default, ref handle);
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        ///     Disposes interop helper dependencies
        /// </summary>
        public void Dispose()
        {
            CommonUtils.DisposeAndSetNull(ref _resource);
        }
        #endregion
    }
}
