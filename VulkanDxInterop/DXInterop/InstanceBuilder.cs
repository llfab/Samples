// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2021 Stryker
// ===========================================================================

using BitsOfNature.Core;
using BitsOfNature.Rendering.Vulkan.Engine;
using D3D = SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;


namespace BitsOfNature.Rendering.Vulkan.Wpf.DXInterop
{
    /// <summary>
    ///     Builds necessary DirectX(11) instances
    /// </summary>
    public static class InstanceBuilder
    {
        #region Private static Attributes
        
        /// <summary>
        ///     DirectX11 device singleton
        /// </summary>
        private static D3D11.Device s_d3dDevice;
        
        /// <summary>
        ///     Indicates if debugging capabilities of devices are enabled
        /// </summary>
        private static bool s_debugEnabled;
        #endregion

        #region Public static Methods
        /// <summary>
        ///     Called to enable Debug capabilities of vulkan and directX devices
        /// </summary>
        /// <returns></returns>
        public static bool EnableDebug()
        {
            s_debugEnabled = true;
            return s_debugEnabled;
        }
        #endregion


        #region Internal Properties
        /// <summary>
        ///     Creates and returns DirectX11 device singleton, used for interop textures and DXGI interfacing
        /// </summary>
        internal static D3D11.Device DX11Device 
        {
            get
            {
                if (s_d3dDevice == null)
                {
                    D3D.FeatureLevel minimumFeatureLevel = D3D.FeatureLevel.Level_11_0;
                    D3D11.DeviceCreationFlags flags = s_debugEnabled ?
                        D3D11.DeviceCreationFlags.Debug : D3D11.DeviceCreationFlags.None;
                    s_d3dDevice = new D3D11.Device(D3D.DriverType.Hardware, flags | D3D11.DeviceCreationFlags.BgraSupport);
                    Assert.That(s_d3dDevice.FeatureLevel >= minimumFeatureLevel,
                        $"Graphics adapter must support Direct3D {minimumFeatureLevel.ToString()[6..].Replace("_", ".")} (or higher)!");
                }

                return s_d3dDevice;
            }
        }
        #endregion

        #region Public static Properties
        /// <summary>
        ///     Creates a vulkan rendertarget with given vulkan renderingcontext and appropriate wpf resource
        /// </summary>
        /// <param name="renderingContext">
        ///     Given vulkan rendering context
        /// </param>
        /// <param name="wpfResource">
        ///     DXGI resource to render with
        /// </param>
        /// <returns>
        ///     Created rendertarget
        /// </returns>
        public static DxInteropRenderTarget CreateRenderTarget(RenderingContext renderingContext, DXGI.Resource wpfResource)
        {
            DxInteropRenderTarget renderTarget =
                new DxInteropRenderTarget(renderingContext, DX11Device, wpfResource);

            return renderTarget;
        }

        #endregion


    }
}
