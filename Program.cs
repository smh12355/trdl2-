using System;
using DelaunatorSharp;
using Numpy;
using static System.Net.Mime.MediaTypeNames;

double[][] test_points = new double[][]
{
new double[] { 5,4, 3 },
new double[] { 1, 1.1, 2 },
new double[] { 1, 3, 4 },
new double[] { 1, 2, 5 }
};
double[][] test_point = new double[][]
{
new double[] { 2.5,2 },
};
double[][] test_points2 = new double[][]
{
new double[] { 1, 2, 3},
new double[] { 3, 4, 2},
new double[] { 5, 6, 4 },
new double[] { -2, 4, 5 },
new double[] { -1, 2, 6 }
};
double[][] test_point2 = new double[][]
{
new double[] { 0,2.5 },
};
double[][] bad_point = new double[][]
{
new double[] { -2.5, 0, 3},
new double[] { 0, 5, 2},
new double[] { 0.5, 6, 4 },
new double[] { -5, -5, 5 },
};
double[][,] search_triangles(double[][] points)
{
    IPoint[] ipoints = new IPoint[points.Length];// массив для триангуляции делоне
    for (int i = 0; i < points.Length; i++)
    {
        ipoints[i] = new Point(points[i][0], points[i][1]);
    }
    try
    {
        Delaunator dn1 = new Delaunator(ipoints);
    }
    catch
    {
        Console.WriteLine("Для данных точек невозможно построить треугольники делоне");
        double[][,] triangles1 = new double[1][,];
        return triangles1;
    }
    Delaunator dn = new Delaunator(ipoints);
    double[][,] triangles = new double[dn.GetTriangles().ToArray().Length][,];
    for (int i = 0; i < dn.GetTriangles().ToArray().Length; i++)
    {
        //triangles[i] = new double[,] { { (double)dn.GetTriangles().ToList()[i].Points.ToList()[0].X, (double)dn.GetTriangles().ToList()[i].Points.ToList()[0].Y }, { (double)dn.GetTriangles().ToList()[i].Points.ToList()[1].X, (double)dn.GetTriangles().ToList()[i].Points.ToList()[1].Y }, { (double)dn.GetTriangles().ToList()[i].Points.ToList()[2].X, (double)dn.GetTriangles().ToList()[i].Points.ToList()[2].Y } };
        triangles[i] = new double[,] { { points[dn.Triangles[0 + 3 * i]][0], points[dn.Triangles[0 + 3 * i]][1], points[dn.Triangles[0 + 3 * i]][2] }, { points[dn.Triangles[1 + 3 * i]][0], points[dn.Triangles[1 + 3 * i]][1], points[dn.Triangles[1 + 3 * i]][2] }, { points[dn.Triangles[2 + 3 * i]][0], points[dn.Triangles[2 + 3 * i]][1], points[dn.Triangles[2 + 3 * i]][2] } };
    }

    return triangles;
}

void triangulation(double[][,] tr, double[][] point)
{
    double a1 = 0;
    double a2 = 0;
    double a3 = 0;
    double[][] centroid_coords = new double[tr.Length][];// массив координат центроида
    try
    {
        for (int i = 0; i < tr.Length; i++)
        {
            double h = 0;
            double h1 = 0;
            for (int j = 0; j < 3; j++)
            {
                h += tr[i][j, 0];
                h1 += tr[i][j, 1];
            }
            centroid_coords[i] = new double[] { h / 3.0, h1 / 3.0 };
        }
    }
    catch
    {
        Console.WriteLine("Невозможно произвести триангуляцию, так как для данных точек не существует треугольников делоне");
        return;
    }
    for (int i = 0; i < tr.Length; i++)
    {
        a1 = (tr[i][0, 0] - point[0][0]) * (tr[i][1, 1] - tr[i][0, 1]) - (tr[i][1, 0] - tr[i][0, 0]) * (tr[i][0, 1] - point[0][1]);
        a2 = (tr[i][1, 0] - point[0][0]) * (tr[i][2, 1] - tr[i][1, 1]) - (tr[i][2, 0] - tr[i][1, 0]) * (tr[i][1, 1] - point[0][1]);
        a3 = (tr[i][2, 0] - point[0][0]) * (tr[i][1, 1] - tr[i][0, 1]) - (tr[i][0, 0] - tr[i][2, 0]) * (tr[i][2, 1] - point[0][1]);

        if ((a1 >= 0 && a2 >= 0 && a3 >= 0) || (a1 <= 0 && a2 <= 0 && a3 <= 0))
        {
            var res = np.cross(np.array(new double[] { tr[i][0, 0] - tr[i][1, 0], tr[i][0, 1] - tr[i][1, 1], tr[i][0, 2] - tr[i][1, 2] }), np.array(new double[] { tr[i][0, 0] - tr[i][2, 0], tr[i][0, 1] - tr[i][2, 1], tr[i][0, 2] - tr[i][2, 2] }));
            var qq = ((double)res[0]);
            var qq1 = ((double)res[1]);
            var qq2 = ((double)res[2]);
            var qq3 = np.dot(np.array(new double[] { ((double)res[0]), ((double)res[1]), ((double)res[2]) }), np.array(new double[] { tr[i][2, 0], tr[i][2, 1], tr[i][2, 2] }));
            var zzz = (qq3 - qq * point[0][0] - qq1 * point[0][1]) / qq2;
            Console.WriteLine($"точка x:({point[0][0]}) y:({point[0][1]}) лежит в треугольнике(включая грани) с вершинами:\n{np.array(new double[,] { { tr[i][0, 0], tr[i][0, 1], tr[i][0, 2] }, { tr[i][1, 0], tr[i][1, 1], tr[i][1, 2] }, { tr[i][2, 0], tr[i][2, 1], tr[i][2, 2] } })}");
            Console.WriteLine($"точка x:({point[0][0]}) y:({point[0][1]})\nинтерполирована к точке треугольника с координатами:\nx:({point[0][0]}) y:({point[0][1]}) и значением функции:({zzz})");
            return;
        }
    }
    var measure = Math.Sqrt(Math.Pow(point[0][0] - centroid_coords[0][0], 2) + Math.Pow(point[0][1] - centroid_coords[0][1], 2));
    int j1 = 0;
    for (int i = 1; i < tr.Length; i++)
    {
        var step = Math.Sqrt(Math.Pow(point[0][0] - centroid_coords[i][0], 2) + Math.Pow(point[0][1] - centroid_coords[i][1], 2));
        if (step < measure)
        {
            measure = step;
            j1 = i;
        }
    }
    var res1 = np.cross(np.array(new double[] { tr[j1][0, 0] - tr[j1][1, 0], tr[j1][0, 1] - tr[j1][1, 1], tr[j1][0, 2] - tr[j1][1, 2] }), np.array(new double[] { tr[j1][0, 0] - tr[j1][2, 0], tr[j1][0, 1] - tr[j1][2, 1], tr[j1][0, 2] - tr[j1][2, 2] }));
    var qqq = ((double)res1[0]);
    var qqq1 = ((double)res1[1]);
    var qqq2 = ((double)res1[2]);
    var qqq3 = np.dot(np.array(new double[] { ((double)res1[0]), ((double)res1[1]), ((double)res1[2]) }), np.array(new double[] { tr[j1][2, 0], tr[j1][2, 1], tr[j1][2, 2] }));
    var z = (qqq3 - qqq * point[0][0] - qqq1 * point[0][1]) / qqq2;
    Console.WriteLine($"точка x:({point[0][0]}) y:({point[0][1]}) лежит вне треугольников, ближайший треугольник с вершинами:\n{np.array(new double[,] { { tr[j1][0, 0], tr[j1][0, 1], tr[j1][0, 2] }, { tr[j1][1, 0], tr[j1][1, 1], tr[j1][1, 2] }, { tr[j1][2, 0], tr[j1][2, 1], tr[j1][2, 2] } })}");
    Console.WriteLine($"точка x:({point[0][0]}) y:({point[0][1]})\nинтерполирована к точке треугольника с координатами:\nx:({point[0][0]}) y:({point[0][1]}) и значением функции:({z})");
    return;
}
triangulation(search_triangles(test_points), test_point);
triangulation(search_triangles(test_points2), test_point2);
triangulation(search_triangles(bad_point), test_point);
//search_triangles(bad_point);
