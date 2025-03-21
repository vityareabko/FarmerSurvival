using Unity.IL2CPP.CompilerServices;
using static Fraktalia.Core.Mathematics.Fmath;

namespace Fraktalia.Core.Mathematics
{
    /// <summary>
    /// A static class containing noise functions.
    /// </summary>
    [FIl2CppEagerStaticClassConstruction]
    public static partial class Fnoise
    {
        // Modulo 289 without a division (only multiplications)
        static float  mod289(float x)  { return x - floor(x * (1.0f / 289.0f)) * 289.0f; }
        static Ffloat2 mod289(Ffloat2 x) { return x - floor(x * (1.0f / 289.0f)) * 289.0f; }
        static Ffloat3 mod289(Ffloat3 x) { return x - floor(x * (1.0f / 289.0f)) * 289.0f; }
        static Ffloat4 mod289(Ffloat4 x) { return x - floor(x * (1.0f / 289.0f)) * 289.0f; }

        // Modulo 7 without a division
        static Ffloat3 mod7(Ffloat3 x) { return x - floor(x * (1.0f / 7.0f)) * 7.0f; }
        static Ffloat4 mod7(Ffloat4 x) { return x - floor(x * (1.0f / 7.0f)) * 7.0f; }

        // Permutation polynomial: (34x^2 + x) math.mod 289
        static float  permute(float x)  { return mod289((34.0f * x + 1.0f) * x); }
        static Ffloat3 permute(Ffloat3 x) { return mod289((34.0f * x + 1.0f) * x); }
        static Ffloat4 permute(Ffloat4 x) { return mod289((34.0f * x + 1.0f) * x); }

        static float  taylorInvSqrt(float r)  { return 1.79284291400159f - 0.85373472095314f * r; }
        static Ffloat4 taylorInvSqrt(Ffloat4 r) { return 1.79284291400159f - 0.85373472095314f * r; }

        static Ffloat2 fade(Ffloat2 t) { return t*t*t*(t*(t*6.0f-15.0f)+10.0f); }
        static Ffloat3 fade(Ffloat3 t) { return t*t*t*(t*(t*6.0f-15.0f)+10.0f); }
        static Ffloat4 fade(Ffloat4 t) { return t*t*t*(t*(t*6.0f-15.0f)+10.0f); }

        static Ffloat4 grad4(float j, Ffloat4 ip)
        {
            Ffloat4 ones = float4(1.0f, 1.0f, 1.0f, -1.0f);
            Ffloat3 pxyz = floor(frac(float3(j) * ip.xyz) * 7.0f) * ip.z - 1.0f;
            float  pw   = 1.5f - dot(abs(pxyz), ones.xyz);
            Ffloat4 p = float4(pxyz, pw);
            Ffloat4 s = float4(p < 0.0f);
            p.xyz = p.xyz + (s.xyz*2.0f - 1.0f) * s.www;
            return p;
        }

        // Hashed 2-D gradients with an extra rotation.
        // (The constant 0.0243902439 is 1/41)
        static Ffloat2 rgrad2(Ffloat2 p, float rot)
        {
            // For more isotropic gradients, math.sin/math.cos can be used instead.
            float u = permute(permute(p.x) + p.y) * 0.0243902439f + rot; // Rotate by shift
            u = frac(u) * 6.28318530718f; // 2*pi
            return float2(cos(u), sin(u));
        }
    }
}
