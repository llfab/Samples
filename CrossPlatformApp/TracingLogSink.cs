// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2021 Stryker
// ===========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Utilities;
using Avalonia.Logging;
using BitsOfNature.Core.IO.Tracing;

namespace CrossPlatformApp
{
    public class TracingLogSink : ILogSink
    {
        private readonly TraceLevel traceLevel;
        private readonly IList<string> areas;

        private readonly Trace trace;

        public TracingLogSink(
            TraceLevel traceLevel,
            IList<string> areas = null)
        {
            this.traceLevel = traceLevel;
            this.areas = areas?.Count > 0 ? areas : null;
            this.trace = new Trace("Avalonia");
        }

        public bool IsEnabled(LogEventLevel level, string area)
        {
            TraceLevel incomingLevel = ToTraceLevel(level);
            return ((this.traceLevel & incomingLevel) == incomingLevel) && (areas?.Contains(area) ?? true);
        }

        private TraceLevel ToTraceLevel(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Fatal:
                    return TraceLevel.Fatal;
                case LogEventLevel.Error:
                    return TraceLevel.Error;
                case LogEventLevel.Warning:
                    return TraceLevel.Warning;
                case LogEventLevel.Information:
                    return TraceLevel.Info;
                case LogEventLevel.Debug:
                    return TraceLevel.Debug;
                case LogEventLevel.Verbose:
                    return TraceLevel.Verbose;
                default:
                    return TraceLevel.Always;
            }
        }

        public void Log(LogEventLevel level, string area, object source, string messageTemplate)
        {
            if (IsEnabled(level, area))
            {
                trace.Message(ToTraceLevel(level), Format<object, object, object>(area, messageTemplate, source));
            }
        }

        public void Log<T0>(LogEventLevel level, string area, object source, string messageTemplate, T0 propertyValue0)
        {
            if (IsEnabled(level, area))
            {
                trace.Message(ToTraceLevel(level), Format<T0, object, object>(area, messageTemplate, source, propertyValue0));
            }
        }

        public void Log<T0, T1>(LogEventLevel level, string area, object source, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            if (IsEnabled(level, area))
            {
                trace.Message(ToTraceLevel(level), Format<T0, T1, object>(area, messageTemplate, source, propertyValue0, propertyValue1));
            }
        }

        public void Log<T0, T1, T2>(LogEventLevel level, string area, object source, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            if (IsEnabled(level, area))
            {
                trace.Message(ToTraceLevel(level), Format(area, messageTemplate, source, propertyValue0, propertyValue1, propertyValue2));
            }
        }

        public void Log(LogEventLevel level, string area, object source, string messageTemplate, params object[] propertyValues)
        {
            if (IsEnabled(level, area))
            {
                trace.Message(ToTraceLevel(level), Format(area, messageTemplate, source, propertyValues));
            }
        }

        private static string Format<T0, T1, T2>(
            string area,
            string template,
            object source,
            T0 v0 = default,
            T1 v1 = default,
            T2 v2 = default)
        {
            var result = new StringBuilder(template.Length);
            var r = new CharacterReader(template.AsSpan());
            var i = 0;

            result.Append('[');
            result.Append(area);
            result.Append("] ");

            while (!r.End)
            {
                var c = r.Take();

                if (c != '{')
                {
                    result.Append(c);
                }
                else
                {
                    if (r.Peek != '{')
                    {
                        result.Append('\'');
                        result.Append(i++ switch
                        {
                            0 => v0,
                            1 => v1,
                            2 => v2,
                            _ => null
                        });
                        result.Append('\'');
                        r.TakeUntil('}');
                        r.Take();
                    }
                    else
                    {
                        result.Append('{');
                        r.Take();
                    }
                }
            }

            if (source is object)
            {
                result.Append(" (");
                result.Append(source.GetType().Name);
                result.Append(" #");
                result.Append(source.GetHashCode());
                result.Append(')');
            }

            return result.ToString();
        }

        private static string Format(
            string area,
            string template,
            object source,
            object[] v)
        {
            var result = new StringBuilder(template.Length);
            var r = new CharacterReader(template.AsSpan());
            var i = 0;

            result.Append('[');
            result.Append(area);
            result.Append(']');

            while (!r.End)
            {
                var c = r.Take();

                if (c != '{')
                {
                    result.Append(c);
                }
                else
                {
                    if (r.Peek != '{')
                    {
                        result.Append('\'');
                        result.Append(i < v.Length ? v[i++] : null);
                        result.Append('\'');
                        r.TakeUntil('}');
                        r.Take();
                    }
                    else
                    {
                        result.Append('{');
                        r.Take();
                    }
                }
            }

            if (source is object)
            {
                result.Append('(');
                result.Append(source.GetType().Name);
                result.Append(" #");
                result.Append(source.GetHashCode());
                result.Append(')');
            }

            return result.ToString();
        }
    }
}
