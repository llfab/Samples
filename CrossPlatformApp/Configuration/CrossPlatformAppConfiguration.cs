// ===========================================================================
//                     B I T S   O F   N A T U R E 
// ===========================================================================
//
// This file is part of the Bits Of Nature source code.
//
// © 2018 Bits Of Nature GmbH. All rights reserved.
// ===========================================================================

using System.Text;

using BitsOfNature.Core.IO.Serialization;

namespace CrossPlatformApp.Configuration
{
    [SerializationTypeId(nameof(CrossPlatformAppConfiguration))]
    public class CrossPlatformAppConfiguration : IAutoserializable
    {
        [Autoserialize]
        public bool UseSharedTexture { get; set; }

        [Autoserialize]
        public bool ShowGraphicsAdapterInfo { get; set; }

        [Autoserialize]
        public bool DebugLayerEnabled { get; set; }

        [Autoserialize]
        public FluoroImageAcquisitionConfiguration FluoroImageAcquisition { get; set; }

        [Autoserialize]
        public ProNavigationConfiguration ProNavigation { get; set; }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder(1024);
            buf.AppendFormat("UseSharedTexture[{0}]\n", UseSharedTexture);
            buf.AppendFormat("ShowGraphicsAdapterInfo[{0}]\n", ShowGraphicsAdapterInfo);
            buf.AppendFormat("DebugLayerEnabled[{0}]\n", DebugLayerEnabled);
            buf.AppendFormat("FluoroImageAcquisition[{0}]\n", FluoroImageAcquisition);
            buf.AppendFormat("ProNavigation[{0}]\n", ProNavigation);

            return buf.ToString();
        }
    }
}
