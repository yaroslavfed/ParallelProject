using System.Diagnostics;

namespace ParallelProj;

internal class Program
{
    static void TimeMetric(string label, Action act)
    {
        var sw = new Stopwatch();
        sw.Start();
        act();
        sw.Stop();
        Console.WriteLine($"{label} : {sw.Elapsed}");
    }

    static void Main(string[] args)
    {
        var n = 3;

        var A = new[,]
        {
            { 1, 1, 1 },
            { 3, 3, 3 },
            { 5, 5, 5 }
        };
        var B = new[,]
        {
            { 2, 2, 2 },
            { 4, 4, 4 },
            { 6, 6, 6 }
        };

        var C = new int[n, n];

        TimeMetric("Matrix multiplication", () =>
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C[i, j] = 0;
                    for (int k = 0; k < n; k++)
                        C[i, j] += A[i, k] * B[k, j];
                }
            }
        });

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                Console.Write($"{C[i, j]} ");
            Console.WriteLine();
        }
    }
}