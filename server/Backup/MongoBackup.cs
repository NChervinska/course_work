using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace server.Backup
{
    public class MongoBackup
    {
        private string _backupComand = "mongodump --uri mongodb+srv://Anastasiia:qaz123@pethotels.d3hrb.mongodb.net";
        private string _restoreCommand = "mongorestore --uri mongodb+srv://Anastasiia:qaz123@pethotels.d3hrb.mongodb.net";
        private async Task ExecuteCommand(string command)
        {
            ProcessStartInfo ProcessInfo;

            ProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + command);
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.UseShellExecute = true;

            await Task.Run(() => Process.Start(ProcessInfo));
        }
        public async Task BackupDatabase()
        {
            await ExecuteCommand(_backupComand);

        }

        public async Task RestoreDatabase()
        {
            await ExecuteCommand(_restoreCommand);
        }
    }
}
