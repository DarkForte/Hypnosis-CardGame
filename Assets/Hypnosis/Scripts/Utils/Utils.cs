using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Utils
{
    public static Random randomGenerator = new Random();
    public static void Swap<T>(ref T a, ref T b)
    {
        T t = a;
        a = b;
        b = t;
        return;
    }
}

