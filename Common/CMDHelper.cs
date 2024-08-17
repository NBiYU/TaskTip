using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskTip.Common
{
    internal class CMDHelper
    {
        private ProcessStartInfo _startProcess;

        public CMDHelper()
        {
            _startProcess = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                UseShellExecute = false,
                CreateNoWindow = false,
                Verb = "runas"
            };
        }

        public bool PermissioCheck()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public async Task<ExcuteResult> ExcuteCommand(string processPath, params string[] args)
        {
            if (args.Length != 0) _startProcess.Arguments = string.Join(" ", args);
            else _startProcess.Arguments = string.Empty;

            if (!PermissioCheck()) return new ExcuteResult(message: "当前权限不足");


            Process process = new Process() { StartInfo = _startProcess };
            try
            {
                process.Start();

                process.StandardInput.AutoFlush = true;
                process.StandardInput.WriteLine($"{processPath} {_startProcess.Arguments}");
                process.StandardInput.WriteLine("EXIT");
                process.WaitForExit();

                var content = process.StandardOutput.ReadToEnd().Split("\r\n").ToList();
                content.RemoveAll(x => string.IsNullOrEmpty(x));
                var result = content[^2].Split(":");

                return new ExcuteResult(bool.Parse(result[0]), result[1]);
            }
            catch (Exception e)
            {
                return new ExcuteResult(message: e.Message);
            }
        }


        public async Task<ExcuteResult> ExcuteApp(string processPath, params string[] args)
        {
            var startInfo = new ProcessStartInfo();
            if (args.Length != 0) startInfo.Arguments = string.Join(" ", args);
            else startInfo.Arguments = string.Empty;
            //创建启动对象

            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Path.GetDirectoryName(processPath);
            startInfo.FileName = processPath;
            //设置启动动作,确保以管理员身份运行
            startInfo.Verb = "runas";
            try
            {
                var process = Process.Start(startInfo);

                return new ExcuteResult("");
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message);
                return new ExcuteResult(message: e.Message);
            }
            //退出


        }
    }

    public class ExcuteResult
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public string Content { get; set; }

        public ExcuteResult(string message)
        {
            Result = string.IsNullOrEmpty(message);
            Message = message;
        }

        public ExcuteResult(bool result, string str)
        {
            Result = result;
            if (result)
            {
                Content = str;
            }
            else
            {
                Message = str;
            }

        }

        public ExcuteResult(bool result, string message, string content)
        {
            Result = result;
            Message = message;
            Content = content;
        }

        public override string ToString() => Message;
    }
}
