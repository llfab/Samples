using System;
using System.Threading.Tasks;

namespace ImagingSourceCaptureTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting...");
                WindowsImagingSourceCameraStream stream = new WindowsImagingSourceCameraStream();
                stream.Init();

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                stream.Exit();
                Console.WriteLine("Stopped!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("error.\n{0}", ex));
            }
        }
    }
}
