using BenchmarkDotNet.Running;
using EntityFrameworkCoreVsDapperApp;


Console.WriteLine("-------------------------------------------");

BenchmarkRunner.Run<Benchmarks>();

Console.WriteLine("-------------------------------------------");