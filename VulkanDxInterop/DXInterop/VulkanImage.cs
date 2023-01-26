// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using System;
using System.Windows;
using System.Windows.Interop;
using BitsOfNature.Core.Utils;
using BitsOfNature.Rendering.Vulkan.Engine;
using DXGI = SharpDX.DXGI;

namespace BitsOfNature.Rendering.Vulkan.Wpf.DXInterop
{
    /// <summary>
    ///     VulkanImage is derived from <see cref="D3DImage"/>
    ///     It is used as container to render Vulkan content into D3DImage backbuffer
    ///
    ///     WindowOwner and pixelsize of (texture-)content has to be set to work properly
    /// </summary>
    public class VulkanImage : D3DImage, IDisposable
    {
        #region Private Attributes
        /// <summary>
        ///     Internal dispatcher which manages the content to interop from Vulkan backend to D3DImage backbuffer
        /// </summary>
        private DxgiToDx9InteropDispatcher _dispatcher;
        
        /// <summary>
        ///     Current pixelsize
        /// </summary>
        private Int2 _pixelSize = new Int2(0, 0);
        
        /// <summary>
        ///     Desired  pixelsize if someone has called SetPixelSize.
        /// </summary>
        private Int2 _desiredPixelSize = new Int2(0, 0);

        /// <summary>
        ///     Actual rendering context
        /// </summary>
        private RenderingContext _renderingContext;

        /// <summary>
        ///     Internal interop rendertarget
        /// </summary>
        private DxInteropRenderTarget _renderTarget;
        #endregion

        #region Delegates
        public delegate void RenderDelegate();

        #endregion

        #region Public Properties

        public RenderDelegate renderCallback;

        /// <summary>
        /// Returns current desired pixel size
        /// </summary>
        public Int2 DesiredPixelSize => _desiredPixelSize;

        /// <summary>
        ///     Current rendertarget, which is used by DxgiToDx9InteropDispatcher
        /// </summary>
        public DxInteropRenderTarget InteropRenderTarget => _renderTarget;
        
        /// <summary>
        ///     Current WindowOwner of VulkanImage
        /// </summary>
        public IntPtr windowOwner
        {
            get => (IntPtr)GetValue(s_windowOwnerProperty);
            set => SetValue(s_windowOwnerProperty, value);
        }

        /// <summary>
        ///     Sets new vulkan rendering context
        /// </summary>
        public RenderingContext RenderingContext
        {
            get => _renderingContext;
            set
            {
                this._renderingContext = value;
            }
        }
        #endregion

        #region Dependency Properties
        /// <summary>
        ///     Used to track if WindowOwner has changed
        /// </summary>
        private static readonly DependencyProperty s_windowOwnerProperty = DependencyProperty.Register("WindowOwner", typeof(IntPtr),
            typeof(VulkanImage), new PropertyMetadata(IntPtr.Zero, new PropertyChangedCallback(HwndOwnerChanged)));
        #endregion

        #region Static Methods
        /// <summary>
        ///     Called if WindowOwner has changed by dependency property
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     Arguments of sender
        /// </param>
        private static void HwndOwnerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            VulkanImage image = (VulkanImage)sender;
            image.EnsureRenderer((IntPtr)args.NewValue);
        }
        #endregion

        #region Private Methdos
        /// <summary>
        ///     Ensures that Interop dispatcher is constructed properly, which is responsible for rendering
        ///     Checks if size has changed and/or if WindowOwner has changed and reconstructs dispatcher again for rendering with new properties
        /// </summary>
        /// <param name="hwnd">
        ///     Current WindowsOwner
        /// </param>
        private void EnsureRenderer(IntPtr hwnd)
        {
            if (this._desiredPixelSize.Width != 0 && this._desiredPixelSize.Height != 0 && this._renderingContext != null)
            {
                bool hasToChange = this._pixelSize != _desiredPixelSize;
                if (this._dispatcher != null)
                {
                    hasToChange |= hwnd != this._dispatcher.WindowsOwner;
                } else
                {
                    hasToChange |= hwnd != IntPtr.Zero;
                }
                
                if (hasToChange)
                {
                    this._pixelSize = _desiredPixelSize;
                    CommonUtils.DisposeAndSetNull(ref _dispatcher);
                    this._dispatcher = new DxgiToDx9InteropDispatcher(this.windowOwner, this, this._pixelSize);
                    this._dispatcher.RenderDxgiCallback = RenderDxgi;
                }
            } 
        }

        /// <summary>
        ///     Called to render dxgi surface
        /// </summary>
        /// <param name="newSurface">
        ///     Indicates if a new surface is occurred
        /// </param>
        /// <param name="resource">
        ///     Resource/Surface to render to
        /// </param>
        private void RenderDxgi(bool newSurface, DXGI.Resource resource)
        {
            if (this._renderingContext != null)
            {
                if (newSurface || this._renderTarget == null)
                {
                    CommonUtils.DisposeAndSetNull(ref _renderTarget);
                    this._renderTarget = InstanceBuilder.CreateRenderTarget(this._renderingContext, resource);
                }

                if (this.renderCallback != null)
                {
                    this.renderCallback();
                }
            }

        }
        #endregion

        #region IDisposable Members
        /// <summary>
        ///     Disposes interop dispatcher dependencies
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Disposes resources
        /// </summary>
        /// <param name="disposing">
        ///     Indicates if it is disposing
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            CommonUtils.DisposeAndSetNull(ref _renderTarget);
            CommonUtils.DisposeAndSetNull(ref _dispatcher);
        }        
        #endregion

        #region Public Methods
        /// <summary>
        ///     Request one rendercall, called interop handler is responsible to render exactly one frame
        /// </summary>
        /// <returns>
        ///     true, if rendering was successful
        /// </returns>
        public bool RequestRender()
        {
            EnsureRenderer(this.windowOwner);
            return this._dispatcher != null && this._dispatcher.RequestRenderer();
        }

        /// <summary>
        ///     Sets desired pixelsize of VulkanImage
        /// </summary>
        /// <param name="pixelWidth">
        ///     Sets current pixel width of texture
        /// </param>
        /// <param name="pixelHeight">
        ///     Sets current pixel height of texture
        /// </param>
        public void SetPixelSize(int pixelWidth, int pixelHeight)
        {
            this._desiredPixelSize = new Int2(pixelWidth, pixelHeight);
        }
        #endregion

        #region Freezable Overrides
        /// <summary>
        ///     <inheritdoc/>
        /// </summary>
        protected override Freezable CreateInstanceCore()
        {
            return new VulkanImage();
        }
        #endregion
    }
}
