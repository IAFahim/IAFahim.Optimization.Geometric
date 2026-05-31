namespace IAFahim.Optimization.Geometric
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class WelzlSphere
    {
        public struct Sphere
        {
            public double X, Y, Z, R;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Sphere Run(double* xs, double* ys, double* zs, int n)
        {
            if (n == 0) return default;

            ulong seed = 123456789;
            for (int i = n - 1; i > 0; i--)
            {
                seed = seed * 6364136223846793005UL + 1442695040888963407UL;
                int j = (int)(seed % (ulong)(i + 1));
                double tx = xs[i]; xs[i] = xs[j]; xs[j] = tx;
                double ty = ys[i]; ys[i] = ys[j]; ys[j] = ty;
                double tz = zs[i]; zs[i] = zs[j]; zs[j] = tz;
            }

            Sphere s = default;
            s.X = xs[0]; s.Y = ys[0]; s.Z = zs[0]; s.R = 1e-9;
            for (int i = 0; i < n && i < 4; i++)
            {
                if (Dist2(s, xs[i], ys[i], zs[i]) > s.R * s.R)
                {
                    s.X = xs[i]; s.Y = ys[i]; s.Z = zs[i]; s.R = 1e-9;
                    for (int j = 0; j < i; j++)
                    {
                        if (Dist2(s, xs[j], ys[j], zs[j]) > s.R * s.R)
                        {
                            s.X = (xs[i] + xs[j]) / 2;
                            s.Y = (ys[i] + ys[j]) / 2;
                            s.Z = (zs[i] + zs[j]) / 2;
                            s.R = Math.Sqrt(Dist2(s, xs[j], ys[j], zs[j]));
                            for (int k = 0; k < j; k++)
                            {
                                if (Dist2(s, xs[k], ys[k], zs[k]) > s.R * s.R)
                                {
                                    s.X = (xs[i] + xs[j] + xs[k]) / 3;
                                    s.Y = (ys[i] + ys[j] + ys[k]) / 3;
                                    s.Z = (zs[i] + zs[j] + zs[k]) / 3;
                                    s.R = 0;
                                    s.R = Math.Max(s.R, Math.Sqrt(Dist2(s, xs[i], ys[i], zs[i])));
                                    s.R = Math.Max(s.R, Math.Sqrt(Dist2(s, xs[j], ys[j], zs[j])));
                                    s.R = Math.Max(s.R, Math.Sqrt(Dist2(s, xs[k], ys[k], zs[k])));
                                }
                            }
                        }
                    }
                }
            }
            return s;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Dist2(Sphere s, double x, double y, double z)
        {
            double dx = s.X - x, dy = s.Y - y, dz = s.Z - z;
            return dx * dx + dy * dy + dz * dz;
        }
    }
}