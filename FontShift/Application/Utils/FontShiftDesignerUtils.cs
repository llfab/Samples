// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using Avalonia.Media;

namespace FontShift.Application.Utils
{
    public static class FontShiftDesignerUtils
    {
        public static string GetConfirmationDesignText()
        {
            return AvaloniaUtils.GetResource<string>("ConfirmationMessageDesignMessageText");
        }

        public static IImage GetErrorConfirmationDesignImage()
        {
            return AvaloniaUtils.GetResourceBitmap("avares://FontShift/AssetsEmbedded/Images/fluoro-image.png");
        }

        public static string GetErrorConfirmationDesignHeaderText()
        {
            return AvaloniaUtils.GetResource<string>("ErrorConfirmationMessageDesignHeaderText");
        }

        public static string GetErrorConfirmationDesignMessageText()
        {
            return AvaloniaUtils.GetResource<string>("ErrorConfirmationMessageDesignMessageText");
        }
    }
}
