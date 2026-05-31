namespace IAFahim.Optimization.Geometric
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinEnclosingBall
    {
        public struct Circle { public double X, Y, R; }

        public static Circle Welzl(double* xs, double* ys, int n, int* p)
        {
            if (n == 0) return new Circle { X = 0, Y = 0, R = 0 };
            if (n == 1) return new Circle { X = xs[0], Y = ys[0], R = 0 };
            Shuffle(n, p); Circle c = new Circle { X = xs[p[0]], Y = ys[p[0]], R = 0 };
            for (int i = 1; i < n; i++) if (!Contains(c, xs[p[i]], ys[p[i]])) c = Solve2(xs, ys, i, p);
            return c;
        }

        private static Circle Solve2(double* xs, double* ys, int i, int* p)
        {
            Circle c = Construct(xs[p[i]], ys[p[i]], xs[p[0]], ys[p[0]]);
            for (int j = 1; j < i; j++) if (!Contains(c, xs[p[j]], ys[p[j]])) c = Solve3(xs, ys, i, j, p);
            return c;
        }

        private static Circle Solve3(double* xs, double* ys, int i, int j, int* p)
        {
            Circle c = Construct(xs[p[i]], ys[p[i]], xs[p[j]], ys[p[j]]);
            for (int k = 0; k < j; k++) if (!Contains(c, xs[p[k]], ys[p[k]])) c = Construct(xs[p[i]], ys[p[i]], xs[p[j]], ys[p[j]], xs[p[k]], ys[p[k]]);
            return c;
        }

        private static void Shuffle(int n, int* p)
        {
            ulong seed = 123456789;
            for (int i = 0; i < n; i++) p[i] = i;
            for (int i = n - 1; i > 0; i--) { seed = seed * 6364136223846793005UL + 1442695040888963407UL; int j = (int)(seed % (ulong)(i + 1)); int t = p[i]; p[i] = p[j]; p[j] = t; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Contains(Circle c, double px, double py) { double dx = px - c.X, dy = py - c.Y; return dx * dx + dy * dy <= c.R * c.R + 1e-9; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Circle Construct(double x1, double y1, double x2, double y2) { double cx = (x1 + x2) / 2, cy = (y1 + y2) / 2; return new Circle { X = cx, Y = cy, R = Math.Sqrt(DistSq(x1, y1, cx, cy)) }; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Circle Construct(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            double bx = x2 - x1, by = y2 - y1, cx = x3 - x1, cy = y3 - y1, B = bx * bx + by * by, C = cx * cx + cy * cy, D = bx * cy - by * cx;
            if (Math.Abs(D) < 1e-9) return ConstructLinear(x1, y1, x2, y2, x3, y3);
            double x = (cy * B - by * C) / (2 * D), y = (bx * C - cx * B) / (2 * D);
            return new Circle { X = x1 + x, Y = y1 + y, R = Math.Sqrt(x * x + y * y) };
        }

        private static Circle ConstructLinear(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            Circle c12 = Construct(x1, y1, x2, y2), c13 = Construct(x1, y1, x3, y3), c23 = Construct(x2, y2, x3, y3), best = c12;
            if (c13.R > best.R) best = c13; if (c23.R > best.R) best = c23; return best;
        }

        private static double DistSq(double x1, double y1, double x2, double y2) => (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
    }
}
