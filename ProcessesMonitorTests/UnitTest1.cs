using ProcessesMonitor;
using System.Diagnostics;

namespace ProcessesMonitorTests
{
    public class ProgramTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ParseAndValidateArgsTest()
        {
            Assert.DoesNotThrow(() =>
            Program.ParseAndValidateArgs(new string[] { "notepad", "5", "1" }));

            Assert.DoesNotThrow(() =>
            Program.ParseAndValidateArgs(new string[] { "chrome", "10", "2" }));

            Assert.Throws<Exception>(() =>
            Program.ParseAndValidateArgs(new string[] {}));

            Assert.Throws<Exception>(() =>
            Program.ParseAndValidateArgs(new string[] { "x"}));

            Assert.Throws<Exception>(() =>
            Program.ParseAndValidateArgs(new string[] { "x", "y", "z"}));

            Assert.Throws<Exception>(() =>
            Program.ParseAndValidateArgs(new string[] { "x", "4.2", "1" }));


            Assert.Pass();
        }
    }

    public class ProcessesMonitorTests 
    {
        string processName = "notepad";
        private int maxLifetime = 0;
        private int monitoringFrequency = 0;
        ProcessesMonitor.ProcessesMonitor monitor;

        [SetUp]
        public void Setup()
        {
            monitor = new ProcessesMonitor.ProcessesMonitor(processName, maxLifetime, monitoringFrequency);
        }

        [Test]
        public void MonitorTest()
        {
            var process = Process.Start(processName);
            monitor.Start();

            Assert.IsFalse(process.HasExited);

            Assert.Pass();
        }
    }
}