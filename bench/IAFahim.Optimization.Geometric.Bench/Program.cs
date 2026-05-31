public class Program { public static void Main(string[] args) => BenchmarkDotNet.Running.BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args); }
