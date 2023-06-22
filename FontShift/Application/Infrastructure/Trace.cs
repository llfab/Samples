using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FontShift.Application.Infrastructure
{
    public class Trace
    {
        [Flags]
        public enum TraceLevel
        {
            Fatal = 0x00000001,
            Error = 0x00000002,
            Warning = 0x00000004,
            Info = 0x00000010,
            Debug = 0x00000020,
            Verbose = 0x00000040,
            Always = -1,
            Default = Fatal | Error | Warning,
        }

        public Trace(string traceName, Trace parentTrace)
        {
        }

        public Trace(string traceName)
            : this(traceName, null)
        { }

        public static Trace ForType<TSource>() => new Trace(typeof(TSource).Name);

        public static Trace ForTypeWithPostfix<TSource>(string postfix) => new Trace(postfix);

        public void Always(string messageFormat, params object[] values) { Message(TraceLevel.Always, messageFormat, values); }
        public void Fatal(string messageFormat, params object[] values) { Message(TraceLevel.Fatal, messageFormat, values); }
        public void Error(string messageFormat, params object[] values) { Message(TraceLevel.Error, messageFormat, values); }
        public void Warning(string messageFormat, params object[] values) { Message(TraceLevel.Warning, messageFormat, values); }
        public void Info(string messageFormat, params object[] values) { Message(TraceLevel.Info, messageFormat, values); }
        public void Debug(string messageFormat, params object[] values) { Message(TraceLevel.Debug, messageFormat, values); }
        public void Verbose(string messageFormat, params object[] values) { Message(TraceLevel.Verbose, messageFormat, values); }

        public void Message(TraceLevel level, string messageFormat, params object[] values)
        {
            
        }

        public static string Site() => "";
    }
}
