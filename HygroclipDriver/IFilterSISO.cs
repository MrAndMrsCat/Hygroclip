using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HygroclipDriver
{
    public interface IFilterSISO<T>
    {
        T Input(T value);

        T Value { get; }
    }
}
