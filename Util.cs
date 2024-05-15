using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pixeljam
{
    public class Util
    {
        public static readonly double c4 = (2 * Math.PI) / 3;
        public static float easeOutElastic(double x)
        {
            return (float)(x == 0 ? 0 : (x == 1 ? 1 : Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75) * c4) + 1));
        }

        public static float easeOutExpo(double x)
        {
            return (float) (x == 1 ? 1 : 1 - Math.Pow(2, -10 * x));
        }

        public static float easeInOutQuad(double x)
        {
            return (float)(x < 0.5 ? 2 * x * x : 1 - Math.Pow(-2 * x + 2, 2) / 2);
        }
    }
}
