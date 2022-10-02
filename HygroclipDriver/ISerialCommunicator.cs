using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HygroclipDriver
{
    public interface ISerialComms
    {
        void Send(byte[] bytes);

        event EventHandler<byte[]>? ReceivedBytes;

        void Halt();
    }
}
