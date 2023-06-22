// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

namespace FontShift.Application.Utils
{
    public static class FontShiftConstants
    {
        // General
        public static readonly string GlobalBackendDateTimeDefaultFormat = AvaloniaUtils.GetResource<string>("GlobalBackendDateTimeDefaultFormat");
        public static readonly string GlobalBackendTimeDefaultFormat = AvaloniaUtils.GetResource<string>("GlobalBackendTimeDefaultFormat");
        public static readonly string GlobalBackendDateDefaultFormat = AvaloniaUtils.GetResource<string>("GlobalBackendDateDefaultFormat");
        public static readonly string GlobalLoggingDateTimeDefaultFormat = AvaloniaUtils.GetResource<string>("GlobalLoggingDateTimeDefaultFormat");
        public static readonly string GlobalLoggingTimeDefaultFormat = AvaloniaUtils.GetResource<string>("GlobalLoggingTimeDefaultFormat");
        public static readonly string GlobalLoggingDateDefaultFormat = AvaloniaUtils.GetResource<string>("GlobalLoggingDateDefaultFormat");
        public static readonly int GlobalAnonymizationMinLength = AvaloniaUtils.GetResource<int>("GlobalAnonymizationMinLength");
        public static readonly char GlobalAnonymizationChar = AvaloniaUtils.GetResource<string>("GlobalAnonymizationChar")[0];

        public static T Get<T>(string key) => AvaloniaUtils.GetResource<T>(key);
    }
}
