// ===========================================================================
//                     B I T S   O F   N A T U R E 
// ===========================================================================
//
// This file is part of the Bits Of Nature source code.
//
// © 2018 Bits Of Nature GmbH. All rights reserved.
// ===========================================================================

using BitsOfNature.Core.IO.Serialization;

namespace CrossPlatformApp.Configuration
{
    [SerializationTypeId(nameof(FluoroImageAcquisitionConfiguration))]
    public class FluoroImageAcquisitionConfiguration : IAutoserializable
    {
        [Autoserialize]
        public bool Enabled { get; set; }

        [Autoserialize]
        public string EngineServerIpAddress { get; set; }

        [Autoserialize]
        public int EngineServerPort { get; set; }

        [Autoserialize]
        public string ImageAcquisitionServerUri { get; set; }

        public override string ToString()
        {
            return string.Format("Enabled[{0}] EngineServerIpAddress[{1}] EngineServerPort[{2}] ImageAcquisitionServerUri[{3}]", 
                Enabled, EngineServerIpAddress, EngineServerPort, ImageAcquisitionServerUri);
        }
    }
}
