using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using UI.Properties;

namespace UI
{
    internal static class Deploy
    {
        public static void CreateProtocolEntries()
        {
            var key = Registry.ClassesRoot.CreateSubKey("exproto", RegistryKeyPermissionCheck.ReadWriteSubTree);

            key.SetValue("URL Protocol", "");
            key.SetValue("", "URL:exproto protocol");
            var command = key.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");

            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dir = Path.Combine(appdata, "exproto");
            var path = Path.Combine(dir, "exproto.bat");
            command.SetValue("", $@"""{path}"" ""%1""");

            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(path, Resources.Execute);
        }
    }
}
