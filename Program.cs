using RandM.RMLib;
using System;
using System.Diagnostics;
using System.Threading;

namespace Aperture
{
    class Program
    {
        static Process proc;

        static void Main(string[] args)
        {
            try
            {
                var cmdIdx = Environment.CommandLine.IndexOf(" -- ");

                if (cmdIdx == -1)
                {
                    Console.Error.WriteLine("Syntax: APERTURE.EXE -H<handle> -N<node> -- <command> <arguments>");
                    return;
                }

                Door.Startup();
                proc = new Process();
                proc.StartInfo.FileName = args[3];
                proc.StartInfo.Arguments = Environment.CommandLine.Substring(cmdIdx + args[3].Length + 5);
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                // we're not redirecting stderr, but that's okay :P
                proc.StartInfo.UseShellExecute = false;
                proc.EnableRaisingEvents = true;
                proc.Start();
                proc.StandardInput.AutoFlush = true;
                var to = new Thread(GetOutput);
                to.IsBackground = true;
                to.Start();
                var ti = new Thread(GetInput);
                ti.IsBackground = true;
                ti.Start();

                while (ti.IsAlive && to.IsAlive)
                {
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Thread.Sleep(3000);
            }
            finally
            {
                Door.Shutdown();
            }
        }

        static void GetInput()
        {
            char? k;

            while (!proc.HasExited)
            {
                try
                {
                    k = Door.ReadKey();
                }
                catch (Exception)
                {
                    // idle timeout
                    return;
                }

                if (k != null)
                {
                    proc.StandardInput.Write(k);
                }
            }
        }

        static void GetOutput()
        {
            int ch;

            while (!proc.HasExited)
            {
                while ((ch = proc.StandardOutput.Read()) != -1)
                {
                    Door.Write(((char)ch).ToString());
                }

                Thread.Sleep(100);
            }
        }
    }
}
