// ===========================================================================\r
//                          B I T S   O F   N A T U R E\r
// ===========================================================================\r
//  This document contains proprietary information. It is the exclusive\r
//  confidential property of Stryker Corporation and its affiliates.\r
//  \r
//  Copyright (c) 2022 Stryker\r
// ===========================================================================

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using BitsOfNature.Core;
using BitsOfNature.Core.Geometry3D;
using BitsOfNature.Core.Grid;
using BitsOfNature.Core.Utils;
using BitsOfNature.Rendering.Vulkan.Engine;
using BitsOfNature.Rendering.Vulkan.Interop.Win32;
using BitsOfNature.Rendering.Vulkan.SceneGraph;
using SharpDX;
using D3D9 = SharpDX.Direct3D9;
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;
using D3D = SharpDX.Direct3D;

namespace BitsOfNature.Rendering.Vulkan.Wpf.DXInterop
{
    /// <summary>
    ///     This helper class is responsible for whole rendering chain an calling its dependencies, 
    ///     done by RequestRenderer:
    ///     1. Providing dxgi texture to render into
    ///     2. Calling vulkan renderer with its dependencies and initiating rendering on dxgi texture
    ///     3. Setting dxgi texture by d3d9Ex interface and D3D9 proxy texture into D3DImage backbuffer. 
    /// </summary>
    internal class DxgiToDx9InteropDispatcher : IDisposable 
    {
        #region Private Attributes
        /// <summary>
        ///     Internal pixelsize of rendering texture
        /// </summary>
        private Int2 _pixelSize;
        /// <summary>
        ///     Window handle used to create internal d3d9Ex device
        /// </summary>
        private HWND _hwnd;
        /// <summary>
        ///     Used d3dImage structure to render into
        /// </summary>
        private readonly D3DImage _d3dImage;
        /// <summary>
        ///     Indicates if surfaces are initialized
        /// </summary>
        private bool _surfacesInitialized;
        /// <summary>
        ///     Indicates if d3d is initialized
        /// </summary>
        private bool _isD3DInitialized;
        /// <summary>
        ///     Used D3D9Ex device
        /// </summary>
        private D3D9.DeviceEx _deviceD3D9Ex;
        /// <summary>
        /// Used D3D9Ex interface
        /// </summary>
        private D3D9.Direct3DEx _d3d9Ex;
        
        /// <summary>
        ///     Shared D3D11 device
        /// </summary>
        private D3D11.Device DeviceD3D11 => InstanceBuilder.DX11Device;

        /// <summary>
        ///     This handles the DirectX9 main texture
        /// </summary>
        private MainTexture _dx9MainTexture;

        /// <summary>
        ///     Shared surface mapper for DX11/DXGI resource
        /// </summary>
        private SurfaceProxyDx11 _sharedSurfaceMapperDx11;
        /// <summary>
        ///     Shared surface mapper for DX9(ex) rendertarget/texture resource
        /// </summary>
        private SurfaceProxyDx9 _sharedSurfaceMapperDx9;

        #endregion

        #region Delegates
        /// <summary>
        ///     Render callback if foreign rendering has to be called
        /// </summary>
        /// <param name="newSurface">
        ///     Indicates if a new surface was allocated
        /// </param>
        /// <param name="resource">
        ///     Given resource to render into
        /// </param>
        public delegate void RenderDxgiDelegate(bool newSurface, DXGI.Resource resource);
        #endregion

        #region Public Properties

        /// <summary>
        ///     Render delegate, called if rendering of foreign resource has to be called
        /// </summary>
        public RenderDxgiDelegate RenderDxgiCallback; 

        /// <summary>
        ///     Windowshandle of owner to create device etc.
        /// </summary>
        public HWND WindowsOwner => _hwnd;
        #endregion

        #region Construction
        /// <summary>
        ///     Constructor to create appropriate helper structure to render on wpf surface.
        /// </summary>
        /// <param name="windowHandle">
        ///     Windowhandle is needed to create an apropriate d3d9Ex device
        /// </param>
        /// <param name="image">
        ///     Associated D3DImage to render into
        /// </param>
        /// <param name="pixelSize">
        ///     Valid pixelsize of D3D9Ex textures to create
        /// </param>
        public DxgiToDx9InteropDispatcher(HWND windowHandle, D3DImage image, Int2 pixelSize)
        {
            this._pixelSize = pixelSize;
            this._hwnd = windowHandle;
            this._d3dImage = image;
        }
        #endregion
        
        #region IDisposable Members
        /// <summary>
        ///     Disposes D3D dependencies
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            CleanupD3D();
        }

        #endregion

        #region Private Methods
        /// <summary>
        ///     Initialized D3D9(ex) device
        /// </summary>
        /// <returns>
        ///     true if device creation is successful
        /// </returns>
        private bool InitD3D9()
        {
            _d3d9Ex = new D3D9.Direct3DEx();
            D3D9.PresentParameters d3dpp = new D3D9.PresentParameters();
            d3dpp.Windowed = true;
            d3dpp.SwapEffect = D3D9.SwapEffect.Discard;
            d3dpp.PresentationInterval = D3D9.PresentInterval.Immediate;

            try
            {
                _deviceD3D9Ex = new D3D9.DeviceEx(_d3d9Ex, 0, D3D9.DeviceType.Hardware, _hwnd,
                    D3D9.CreateFlags.HardwareVertexProcessing | D3D9.CreateFlags.Multithreaded |
                    D3D9.CreateFlags.FpuPreserve, d3dpp);
            }
            catch (SharpDXException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Cleanup of D3D9(ex) device
        /// </summary>
        private void CleanupD3D9()
        {
            CommonUtils.DisposeAndSetNull(ref _deviceD3D9Ex);
            CommonUtils.DisposeAndSetNull(ref _d3d9Ex);
        }

        /// <summary>
        ///     Inits D3D dependencies
        /// </summary>
        /// <returns>
        ///     true, if D3D dependencies are created successfully
        /// </returns>
        private bool InitD3D()
        {
            bool result = true;
            if (!_isD3DInitialized)
            {
                // note: can't throw exception if any error is occurred (e.g. no valid hwnd handle, differs from native code)
                result &= InitD3D9();

                _isD3DInitialized = result;
            }
            return result;
        }

        /// <summary>
        ///     Cleanup of d3d dependencies
        /// </summary>
        private void CleanupD3D()
        {
            if (_surfacesInitialized)
            {
                CleanupSurfaces();
            }

            _isD3DInitialized = false;
            CleanupD3D9();
        }

        /// <summary>
        ///     Init dxgi and wpf surface handlers
        /// </summary>
        private void InitSurfaces()
        {
            Int2 size = new Int2(_pixelSize.Width, _pixelSize.Height);

            Assert.That(_deviceD3D9Ex != null, "D3D9 device must not null.");

            this._dx9MainTexture = new MainTexture(this._deviceD3D9Ex, size);
            this._sharedSurfaceMapperDx11 = new SurfaceProxyDx11(this.DeviceD3D11, this._dx9MainTexture);
            this._sharedSurfaceMapperDx9 = new SurfaceProxyDx9(this._deviceD3D9Ex, size, this._dx9MainTexture);

            this._surfacesInitialized = true;
        }

        /// <summary>
        ///     Cleanup surfaces
        /// </summary>
        private void CleanupSurfaces()
        {
            this._surfacesInitialized = false;
            CommonUtils.DisposeAndSetNull(ref _dx9MainTexture);
            CommonUtils.DisposeAndSetNull(ref _sharedSurfaceMapperDx9);
        }


        /// <summary>
        ///     Initialized all d3d dependencies like d3d9(ex) device and surface handlers
        /// </summary>
        /// <returns>
        ///     true if successful
        /// </returns>
        private bool Initialize()
        {
            if (_isD3DInitialized)
            {
                D3D9.DeviceState checkDevice = _deviceD3D9Ex.CheckDeviceState((IntPtr)null);
                if (checkDevice != D3D9.DeviceState.Ok)
                {
                    CleanupD3D();
                }
            }
            else
            {
                if (!InitD3D())
                {
                    CleanupD3D();
                    return false;
                }
            }

            if (!_surfacesInitialized)
            {
                InitSurfaces();
            }

            return true;
        }

        #endregion
        
        #region Public Methods
        /// <summary>
        ///     Initiates rendering from vulkan rendertarget to WPF D3DImage backbuffer
        ///         1. Checks if object is initialized and forces it, if not
        ///         2. Locks D3DImage for new backbuffer content
        ///         3. Prepares DXGI resource to render on
        ///         4. Calls Vulkan rendering backend
        ///         5. Propagates content to WPF D3DImage backbuffer
        ///         6. Set a dirtyRect on D3DImage backbuffer and unlocks it 
        /// </summary>
        /// <returns>
        ///     true if method was successful
        /// </returns>
        public bool RequestRenderer()
        {
            // Important to check this before initialize is called!
            bool isNewSurface = !_surfacesInitialized;

            if (_d3dImage == null || !Initialize())
            {
                return false;
            }

            bool dirtyRect = false;

            _d3dImage.Lock();

            // -------------------
            // Consumer DX9
            ComObject comDXGISurface = this._sharedSurfaceMapperDx11.Resource;
            if (comDXGISurface == null)
            {
                goto Cleanup;
            }
            DXGI.Resource dxgiResource = comDXGISurface.QueryInterface<DXGI.Resource>();
            if (dxgiResource == null)
            {
                goto Cleanup;
            }

            if (this.RenderDxgiCallback != null)
            {
                this.RenderDxgiCallback(isNewSurface, dxgiResource);
            }

            // Consumer DX9
            ComObject comTex9 = this._sharedSurfaceMapperDx9.Resource;
            if (comTex9 == null)
            {
                goto Cleanup;
            }
            D3D9.Texture tex9 = comTex9.QueryInterface<D3D9.Texture>();
            D3D9.Surface surface9 = tex9.GetSurfaceLevel(0);
            if (surface9 != null)
            {
                // enableSoftwareFallback
                // Supports fallback to software rendering for Remote Desktop, etc...
                // Was added in WPF 4.5
                _d3dImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface9.NativePointer, true);
                dirtyRect = true;
            }

            Cleanup:
            try
            {
                if (dirtyRect)
                {
                    _d3dImage.AddDirtyRect(new Int32Rect(0, 0, _d3dImage.PixelWidth,
                        _d3dImage.PixelHeight));
                }
            }
            finally
            {
                _d3dImage.Unlock();
            }
            return dirtyRect;
        }
        #endregion

    }
}
