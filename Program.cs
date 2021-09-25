using log4net;
using Microsoft.Win32.TaskScheduler;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace NetworkAdapterRouteControl
{
    static class Program
    {

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static bool IsAdministrator()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch
            {
                return false;
            }
        }

        private static void ExecuteAsAdministrator(string fileName, string args = null)
        {
            ProcessStartInfo info = new ProcessStartInfo(fileName);
            if (args != null) info.Arguments = args;
            info.UseShellExecute = true;
            info.Verb = "runas";
            while(true)
            {
                try
                {
                    Process.Start(info);

                }
                catch (Win32Exception ex)
                {
                    if (ex.NativeErrorCode == WinApi.ApiError.ERROR_CANCELLED)
                    {
                        var result = MessageBox.Show("Для запуска приложения необходимы привилегии локального администратора",
                                                     "Контроль параметров маршрутизации",
                                                     MessageBoxButtons.RetryCancel,
                                                     MessageBoxIcon.Question);
                        if (result == DialogResult.Retry) continue;
                    }
                    else
                        throw;
                }
                break;
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                string action = String.Empty;
                if (args.Length > 0) action = args[0];
                var isAdministrator = IsAdministrator();
                if (String.Equals(action, "-createTask") || !isAdministrator)
                {
                    var taskPath = Application.ProductName.ToString() + "UA";
                    var applicationPath = "\"" + Application.ExecutablePath.ToString() + "\"";
                    var execAction = new ExecAction(applicationPath);
                    Task task = TaskService.Instance.GetTask(taskPath);
                    if (task == null ||
                        task.Definition.Actions.Find(x => String.Equals((x as ExecAction)?.Path, applicationPath)) == null)
                    {
                        if (isAdministrator)
                        {
                            TaskDefinition taskDef = TaskService.Instance.NewTask();
                            taskDef.Principal.LogonType = TaskLogonType.InteractiveToken;
                            taskDef.Principal.RunLevel = TaskRunLevel.Highest;
                            taskDef.Settings.AllowDemandStart = true;
                            taskDef.Settings.MultipleInstances = TaskInstancesPolicy.Parallel;
                            taskDef.RegistrationInfo.Author = Application.CompanyName;
                            taskDef.Actions.Add(new ExecAction(applicationPath));
                            task = TaskService.Instance.RootFolder.RegisterTaskDefinition(taskPath, taskDef);
                        }
                        else
                        {
                            if (String.Equals(action, "-createTask"))
                            {
                                MessageBox.Show("Для запуска приложения необходимы привилегии локального администратора",
                                                "Контроль параметров маршрутизации",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Exclamation);
                                return;
                            }
                            ExecuteAsAdministrator(Application.ExecutablePath.ToString(), "-createTask");
                            return;
                        }
                    }
                    task.Run();
                }
                else
                {
                    using (var spi = new SingleProgramInstance("(5ca967a8-c107-46d9-8ddb-3e947c029d7e)"))
                    {
                        if (spi.IsSingleInstance)
                        {
                            Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);
                            Application.Run(new MainForm());
                        }
                    }
                }
            } catch (Exception e)
            {
                _log.Fatal(e.ToString());
                MessageBox.Show("При запуске приложения произошла критическая ошибка",
                                "Контроль параметров маршрутизации",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }
}
