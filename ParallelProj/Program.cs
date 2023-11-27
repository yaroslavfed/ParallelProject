using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace ParallelProj;

internal class Program
{
    private static readonly ILogger s_logger =
        LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<Program>();

    static async Task Main(string[] args)
    {
        int n = 5000, m = 5000;
        s_logger.LogInformation($"Initialize rows & cols [n = {n}, m = {m}]");

        double[][] A = await MatrixCreate(n, m);

        double[][] B = await MatrixCreate(m, n);

        double[][] C = default!;

        await TimeMetric("Matrix multiplication single thread", async () => { C = await MultiplyMatrixSingle(A, B); });
        // PrintMatrix(C, "Matrix multiplication single thread");

        await TimeMetric("Matrix multiplication parallel TPL", async () => { C = await MultiplyMatrixParallelTPL(A, B); });
        // PrintMatrix(C, "Matrix multiplication parallel TPL");
    }

    static Task PrintMatrix(double[][] matrix, string name)
    {
        int Rows = matrix.Length;
        int Cols = matrix[0].Length;

        var sb = new StringBuilder();
        sb.Append($" {name} |");

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
                sb.Append($" {matrix[i][j]} ");
            sb.Append("|");
        }

        s_logger.LogInformation(sb.ToString());

        return Task.CompletedTask;
    }

    static async Task TimeMetric(string label, Func<Task> act)
    {
        var sw = new Stopwatch();
        sw.Start();
        await act();
        sw.Stop();
        s_logger.LogInformation($"{label} : {sw.Elapsed}");
    }

    static async Task<double[][]> MatrixCreate(int rows, int cols)
    {
        double[][] result = new double[rows][];
        for (int i = 0; i < rows; ++i)
        {
            result[i] = new double[cols];
            for (int j = 0; j < cols; j++)
            {
                result[i][j] = i * j + 1;
            }
        }

        return result;
    }

    static async Task<double[][]> MultiplyMatrixSingle(double[][] matrixA,
        double[][] matrixB)
    {
        int aRows = matrixA.Length;
        int aCols = matrixA[0].Length;
        int bRows = matrixB.Length;
        int bCols = matrixB[0].Length;

        if (aCols != bRows)
            throw new Exception("Non conformable");

        double[][] result = await MatrixCreate(aRows, bCols);

        for (int i = 0; i < aRows; ++i)
            for (int j = 0; j < bCols; ++j)
                for (int k = 0; k < aCols; ++k)
                    result[i][j] += matrixA[i][k] * matrixB[k][j];

        return result;
    }

    static async Task<double[][]> MultiplyMatrixParallelTPL(double[][] matrixA,
        double[][] matrixB)
    {
        int aRows = matrixA.Length;
        int aCols = matrixA[0].Length;
        int bRows = matrixB.Length;
        int bCols = matrixB[0].Length;

        if (aCols != bRows)
            throw new Exception("Non conformable");

        double[][] result = await MatrixCreate(aRows, bCols);

        Parallel.For(0, aRows, i =>
            {
                for (int j = 0; j < bCols; ++j)
                    for (int k = 0; k < aCols; ++k)
                        result[i][j] += matrixA[i][k] * matrixB[k][j];
            }
        );
        return result;
    }
}