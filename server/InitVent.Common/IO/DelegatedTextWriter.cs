using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InitVent.Common.IO
{
    /// <summary>
    /// A hub for redirecting and forking text streams.
    /// </summary>
    /// <remarks>
    /// To write to a stream, call any one of its Write() methods (as usual).  To read from
    /// a stream, attach a listener to its WriteMethod property.
    /// </remarks>
    public class DelegatedTextWriter : TextWriter
    {
        public static DelegatedTextWriter ConsoleOut { get; private set; }
        public static DelegatedTextWriter ConsoleError { get; private set; }
        public static DelegatedTextWriter ConsoleDuplex { get; private set; }

        static DelegatedTextWriter()
        {
            ConsoleDuplex = new DelegatedTextWriter();  //str => System.Diagnostics.Debug.Write(str)
            ConsoleOut = new DelegatedTextWriter(Console.Out.Write, ConsoleDuplex.Write);
            ConsoleError = new DelegatedTextWriter(Console.Error.Write, ConsoleDuplex.Write);
        }

        public Action<String> WriteMethod { get; set; }

        public DelegatedTextWriter(params Action<String>[] writeMethods)
        {
            foreach (var method in writeMethods)
                WriteMethod += method;
        }

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }

        public override void Write(char value)
        {
            Write(value.ToString());
        }

        public override void Write(char[] buffer, int index, int count)
        {
            var str = String.Join(null, buffer.Skip(index).Take(count));
            Write(str);
        }

        public override void Write(String value)
        {
            if (WriteMethod != null)
                WriteMethod(value);
        }

        public static void CaptureConsoleAll()
        {
            CaptureConsoleOut();
            CaptureConsoleError();
        }

        public static void CaptureConsoleOut()
        {
            Console.SetOut(ConsoleOut);
        }

        public static void CaptureConsoleError()
        {
            Console.SetError(ConsoleError);
        }
    }
}
