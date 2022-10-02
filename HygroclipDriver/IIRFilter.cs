using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HygroclipDriver
{
    public class IIRFilter : IFilterSISO<double>
    {
        public IIRFilter(double alpha)
        {
            if (alpha < 0 || alpha > 1) throw new ArgumentOutOfRangeException(nameof(alpha));

            Alpha = alpha;
        }

        public double Input(double value)
        {
            return double.IsNaN(Value) 
                ? Value = value 
                :  Value = Alpha * value + (1 - Alpha) * Value;
        }

        public double Value { get; private set; } = double.NaN;
        public double Alpha { get; private set; }
    }
}
