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
    [SerializationTypeId(nameof(ProNavigationConfiguration))]
    public class ProNavigationConfiguration : IAutoserializable
    {
        [Autoserialize]
        public bool Enabled { get; set; }

        [Autoserialize]
        public string NavigationServerUri { get; set; }

        public override string ToString()
        {
            return string.Format("Enabled[{0}] NavigationServerUri[{1}]", 
                Enabled, NavigationServerUri);
        }
    }
}
