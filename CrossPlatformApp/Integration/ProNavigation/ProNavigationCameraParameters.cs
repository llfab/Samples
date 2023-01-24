// ===========================================================================
//                     B I T S   O F   N A T U R E 
// ===========================================================================
//
// This file is part of the Bits Of Nature source code.
//
// © 2018 Bits Of Nature GmbH. All rights reserved.
// ===========================================================================

namespace CrossPlatformApp.Integration.ProNavigation
{
    public class ProNavigationCameraParameters
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Calibrated { get; set; }
        public SizeD SensorSize { get; set; }
        public PointD OpticalCenter { get; set; }
        public SizeD PixelSize { get; set; }
        public double FocalLength { get; set; }
        public DistanceCoefficientParameters DistanceCoefficients { get; set; }
        public string ApplicationTypeId { get; set; }

        public override string ToString()
        {
            return string.Format("Id[{0}] Name[{1}] Type[{2}] Cal[{3}] SensSize[{4}] OptCtr[{5}] PxSize[{6}] FocLen[{7}] DistCoeff[{8}] ApplicationTypeId[{9}]", Id, Name, Type, Calibrated, SensorSize, OpticalCenter, PixelSize, FocalLength, DistanceCoefficients, ApplicationTypeId);
        }
    }

    public class DistanceCoefficientParameters
    {
        public double Radial1 { get; set; }
        public double Radial2 { get; set; }
        public double Radial3 { get; set; }
        public double Tangential1 { get; set; }
        public double Tangential2 { get; set; }

        public override string ToString()
        {
            return string.Format("Rad1[{0}] Rad2[{1}] Rad3[{2}] Tan1[{3}] Tan2[{4}]", Radial1, Radial2, Radial3, Tangential1, Tangential2);
        }
    }

    public class PointD
    {
        public double X { get; set; }
        public double Y { get; set; }

        public override string ToString()
        {
            return string.Format("{0:F4}x{1:F4}", X, Y); ;
        }
    }

    public class SizeD
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public override string ToString()
        {
            return string.Format("{0:F4}x{1:F4}", Width, Height); ;
        }
    }

}
