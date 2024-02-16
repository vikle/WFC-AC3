using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class RandomExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int NextArrayElementIndex(this Random random, IReadOnlyList<double> weights)
    {
        int count = weights.Count;
        if (count == 1) return 0;

        double tmp_sum = 0d;

        for (int i = 0; i < count; i++)
        {
            tmp_sum += weights[i];
        }
            
        double threshold = (random.NextDouble() * tmp_sum);
        tmp_sum = 0d;

        for (int i = 0; i < count; i++)
        {
            tmp_sum += weights[i];
            if (threshold < tmp_sum) return i;
        }

        return (count - 1);
    }
};
