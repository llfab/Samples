// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitsOfNature.Core;
using BitsOfNature.Core.IO.Tracing;
using BitsOfNature.Rendering.Vulkan.LowLevel;
using D3D = SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;

namespace BitsOfNature.Rendering.Vulkan.Wpf.DXInterop
{
    public class DxDeviceSelector : DeviceSelector
    {
        private static Trace s_trace = new Trace("DxDeviceSelector");

        public static DxDeviceSelector DxDevice => new DxDeviceSelector();

        protected override PhysicalDevice SelectDevice(IEnumerable<PhysicalDevice> candidates)
        {
            try
            {
                // Create dummy dx device to retrieve its physical device.
                // We need to use the very same physical device as will (later)
                // be used be WPF (DirectX) to share textures
                // Note: We assume that the creation of a (hardware) DX11 device is based on the same hardware as the WPF subsystem
                DXGI.Factory1 factory = new DXGI.Factory1();
                int count = factory.GetAdapterCount();
                for (int i = 0; i < count; i++)
                {
                    DXGI.Adapter1 current = factory.GetAdapter1(i);
                    switch (current.Description.VendorId)
                    {
                        case 0x10DE:
                            s_trace.Verbose("{0} Found Nvidia adapter.", Trace.Site());
                            break;
                        case 0x8086:
                            s_trace.Verbose("{0} Found Intel adapter.", Trace.Site());
                            break;
                        case 0x1002:
                            s_trace.Verbose("{0} Found AMD adapter.", Trace.Site());
                            break;
                        case 0x1414:
                            s_trace.Verbose("{0} Found MS Basic Driver adapter.", Trace.Site());
                            break;
                        default:
                            s_trace.Warning("{0} Unknown adapter found.", Trace.Site());
                            break;
                    }
                }


                D3D.FeatureLevel minimumFeatureLevel = D3D.FeatureLevel.Level_11_0;
                D3D11.DeviceCreationFlags flags = D3D11.DeviceCreationFlags.None;

                using (D3D11.Device d3dDevice = new D3D11.Device(D3D.DriverType.Hardware, flags | D3D11.DeviceCreationFlags.BgraSupport))
                {
                    Assert.That(d3dDevice.FeatureLevel >= minimumFeatureLevel, 
                        $"Graphics adapter must support Direct3D {minimumFeatureLevel.ToString()[6..].Replace("_", ".")} (or higher)!");
                    // find physical device and compare with candidates
                    // return the candidate device that is used by directx
                    DXGI.Device1 device1 = d3dDevice.QueryInterface<DXGI.Device1>();
                    int dxVendorId = device1.Adapter.Description.VendorId;
                    switch (dxVendorId)
                    {
                        case 0x10DE:
                            s_trace.Debug("{0} Default DX11 device matches with Nvidia adapter.", Trace.Site());
                            break;
                        case 0x8086:
                            s_trace.Debug("{0} Default DX11 device matches with Intel adapter.", Trace.Site());
                            break;
                        case 0x1002:
                            s_trace.Debug("{0} Default DX11 device matches with AMD adapter.", Trace.Site());
                            break;
                    }
                    foreach (PhysicalDevice vulkanDevice in candidates)
                    {
                        if (vulkanDevice.Properties.VendorId == dxVendorId)
                        {
                            s_trace.Info("{0} Vendor of DirectX and Vulkan matched, found '{1}' for rendering.", Trace.Site(), vulkanDevice.ToString());
                            return vulkanDevice;
                        }
                    }
                }
                s_trace.Error("{0} No valid vulkan device found, try to select default...", Trace.Site());
                return null;
            }
            catch (Exception ex)
            {
                s_trace.Error("{0} Error selecting the device that is used by DX11. Selecting default... Error[{1}]", Trace.Site(), ex);
                return null;
            }
        }
    }
}
