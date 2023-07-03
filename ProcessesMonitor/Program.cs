using System.Diagnostics;

namespace ProcessesMonitor
{
    public class ProcessesMonitor
    {

        private string processName;
        private int maxLifetime;
        private int monitoringFrequency;

        Thread thread;

        public ProcessesMonitor(string processName, int maxLifetime, int monitoringFrequency) 
        {
            this.processName = processName;
            this.maxLifetime = maxLifetime; 
            this.monitoringFrequency = monitoringFrequency;

            thread = new Thread(new ThreadStart(Monitor));
        }

        private void Monitor() 
        {
            Console.WriteLine(
                String.Format("[{0}] Monitoring started", processName));

            try
            {
                while (true)
                {
                    var processes = Process.GetProcessesByName(processName);

                    if (processes.Length < 1)
                    {
                        Console.WriteLine(
                            String.Format("[{0}] Process not found", processName));
                    }
                    else
                    {
                        var process = processes.First();

                        process.Refresh();

                        var now = DateTime.Now;
                        var lifetime = (now - process.StartTime).Minutes;

                        if (lifetime >= maxLifetime)
                        {

                            process.Kill();

                            Console.WriteLine(
                                String.Format("[{0}] Process terminated",
                                processName));
                        }
                        else 
                        {
                            Console.WriteLine(String.Format("[{2}] Lifetime: {0}/{1}", lifetime, maxLifetime, processName));
                        }

                    }



                    Thread.Sleep(monitoringFrequency * 1000 * 60);
                }
            }
            catch (ThreadInterruptedException e)
            {
                Console.WriteLine(
                    String.Format("[{0}] Monitoring ended", processName));
            }
        }

        public void Start()
        {
            thread.Start();
        }

        public void Stop() 
        {
            thread.Interrupt();
        }
    }

    public class Program
    {
        static public (string processName, int maxLifetime, int monitoringFrequency) 
            ParseAndValidateArgs(string[] args)
        {
            if (args.Length != 3) 
            {
                throw new Exception(
                    String.Format("Unexpected count of arguments: {0}", args.Length));
            }

            var processName = args[0];

            if (!int.TryParse(args[1], out int maxLifetime))
            {
                throw new Exception(
                    String.Format("Unexpected maxLifetime value: {0}", args[1]));
            }

            if (!int.TryParse(args[2], out int monitoringFrequency))
            {
                throw new Exception(
                    String.Format("Unexpected monitoringFrequency value: {0}", args[2]));
            }

            return (processName, maxLifetime, monitoringFrequency);
        }


        static void Main(string[] args)
        {

            string processName = "";
            int maxLifetime = 0;
            int monitoringFrequency = 0;

            try
            {
                (processName, maxLifetime, monitoringFrequency) = ParseAndValidateArgs(args);
                
            }
            catch (Exception e) 
            {
                Console.WriteLine(String.Format("Invalid arguments: \n\t{0}", e.Message));

                Console.WriteLine("Usage: monitor.exe [process name] [maximum lifetime (in minutes)] [monitoring frequency (in minutes)]");
                return;
            }

            ProcessesMonitor monitor = new ProcessesMonitor(processName, maxLifetime, monitoringFrequency);
            monitor.Start();

            while (Console.ReadKey(true).Key != ConsoleKey.Q)
            {
                continue;
            }

            monitor.Stop();
        }
    }
}