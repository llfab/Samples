// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.IO;

namespace FontShift.Application.Utils
{
    /// <summary>
    ///     Provides utility functions using Avalonia within BitsOfNature
    /// </summary>
    public static class AvaloniaUtils
    {
        #region Bitmap Loading

        /// <summary>
        ///     Image cache store for loaded resource images
        /// </summary>
        private static readonly Dictionary<string, Bitmap> s_bitmapCache = new Dictionary<string, Bitmap>();

        /// <summary>
        ///     Loads and returns a <see cref="Bitmap"/> based on a resource path
        /// </summary>
        public static Bitmap LoadResourceBitmap(string resourcePath)
        {
            return new Bitmap(AssetLoader.Open(new Uri(resourcePath)));
        }

        /// <summary>
        ///     Returns a <see cref="Bitmap"/> based on a resource path.
        ///     Uses caching.
        /// </summary>
        public static Bitmap GetResourceBitmap(string resourcePath)
        {
            string normalizedResourcePath = resourcePath.ToLower();

            if (!s_bitmapCache.ContainsKey(normalizedResourcePath))
            {
                Bitmap image = LoadResourceBitmap(resourcePath);
                s_bitmapCache[normalizedResourcePath] = image;
            }

            return s_bitmapCache[normalizedResourcePath];
        }

        #endregion

        #region Text Loading

        /// <summary>
        ///     Text cache store for loaded resource texts
        /// </summary>
        private static readonly Dictionary<string, string> s_textCache = new Dictionary<string, string>();

        /// <summary>
        ///     Loads and returns a text based on a resource path
        /// </summary>
        public static string LoadResourceText(string resourcePath)
        {
            string text = null;

            using (StreamReader reader = new StreamReader(AssetLoader.Open(new Uri(resourcePath))))
            {
                text = reader.ReadToEnd();
            }

            return text;
        }

        /// <summary>
        ///     Returns a text based on a resource path.
        ///     Uses caching.
        /// </summary>
        public static string GetResourceText(string resourcePath)
        {
            string normalizedResourcePath = resourcePath.ToLower();

            if (!s_bitmapCache.ContainsKey(normalizedResourcePath))
            {
                string text = LoadResourceText(resourcePath);
                s_textCache[normalizedResourcePath] = text;
            }

            return s_textCache[normalizedResourcePath];
        }

        #endregion

        #region Resource Loading

        /// <summary>
        ///     Loads a resource in a robust manner
        /// </summary>
        public static T GetResource<T>(string resourceName)
        {
            return GetResource<T>(resourceName, null);
        }

        /// <summary>
        ///     Loads a resource in a robust manner
        /// </summary>
        public static T GetResource<T>(string resourceName, ThemeVariant themeVariant)
        {
            if (Avalonia.Application.Current.Styles.TryGetResource(resourceName, themeVariant, out object resourceObject))
            {
                T resource = (T)resourceObject;

                return resource;
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        ///     Loads a resource in a robust manner
        /// </summary>
        public static T TryGetResource<T>(string resourceName)
        {
            return TryGetResource<T>(resourceName, null);
        }

        /// <summary>
        ///     Loads a resource in a robust manner
        /// </summary>
        public static T TryGetResource<T>(string resourceName, ThemeVariant themeVariant)
        {
            if (Avalonia.Application.Current.Styles.TryGetResource(resourceName, themeVariant, out object resourceObject))
            {
                if (resourceObject == null)
                {
                    return default;
                }

                T resource = (T)resourceObject;

                if (EqualityComparer<T>.Default.Equals(resource, default))
                {
                    return default;
                }

                return resource;
            }
            else
            {
                return default;
            }
        }

        #endregion
    }
}
