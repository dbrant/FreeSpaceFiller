using System;
using System.IO;

//
// Very simple brute-force method of permanently wiping free space on the disk
// by creating a file and allowing it to fill up remaining free space with
// zeros, until the OS itself no longer allows the file to be written.
// When finished, simply delete the huge temporary file.
//
// Naturally, this has the undesirable consequence of inflicting unnecessary
// wear onto the disk, but it's a tradeoff that must be accepted if this wiping
// method is used.
//
// Dmitry Brant, 2018
//
namespace FreeSpaceFiller
{
    class Program
    {
        const string FileName = "temp";
        const string FileExt = "bin";
        const int BufferSize = 65536;
        const int UpdateMillis = 5000;

        static void Main(string[] args)
        {
            long totalBytesWritten = 0;
            long millis = Environment.TickCount;
            var buffer = new byte[BufferSize];
            var random = new Random();

            string fileName;
            int fileIndex = 0;
            while (true) {
                fileName = FileName + fileIndex + "." + FileExt;
                if (File.Exists(fileName))
                {
                    fileIndex++;
                    continue;
                }
                break;
            }

            Console.WriteLine("Creating file: " + Path.GetFullPath(fileName));

            try
            {
                using (var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
                {
                    while (true)
                    {
                        random.NextBytes(buffer);
                        stream.Write(buffer, 0, buffer.Length);
                        totalBytesWritten += buffer.Length;
                        if (Environment.TickCount - millis > UpdateMillis)
                        {
                            millis = Environment.TickCount;
                            Console.WriteLine("Wrote " + (totalBytesWritten / 1000000) + " MB");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
