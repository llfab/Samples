// ===========================================================================\r
//                          B I T S   O F   N A T U R E\r
// ===========================================================================\r
//  This document contains proprietary information. It is the exclusive\r
//  confidential property of Stryker Corporation and its affiliates.\r
//  \r
//  Copyright (c) 2022 Stryker\r
// ===========================================================================

using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;

namespace BitsOfNature.Rendering.Vulkan.Wpf.DXInterop
{
    /// <summary>
    ///     SurfaceProxy for DX11/DXGI resource 
    /// </summary>
    internal class SurfaceProxyDx11
    {

        #region Public Property
        /// <summary>
        ///     DXGI interface of DX9ex mainTexture 
        /// </summary>
        public DXGI.Resource Resource { get; }
        #endregion

        #region Construction
        /// <summary>
        ///     Constructor to retrieve DXGI resource of mainTexture
        /// </summary>
        /// <param name="device">
        ///     Given D3D11Device to retrieve DXGI resource 
        /// </param>
        /// <param name="createdMainTexture">
        ///     DX9ex rendertarget mainTexture 
        /// </param>
        public SurfaceProxyDx11(D3D11.Device device, MainTexture createdMainTexture)
        {
            Resource = device.OpenSharedResource<DXGI.Resource>(createdMainTexture.SurfaceHandle);
        }
        #endregion
    }

}
