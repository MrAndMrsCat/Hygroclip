using System.Linq;

namespace HygroclipBlazorServer
{
    public class CommandLineParser
    {
        public static Dictionary<string, string[]> GetCommandLineArgs(string[] args)
        {
            Dictionary<string, string[]> result = new ();

            string? currentArg = null;

            foreach (string item in args)
            {
                if (item[0] == '-')
                {
                    currentArg = item[1..];
                    result[currentArg] = Array.Empty<string>();
                }
                else if (currentArg is not null)
                {
                    result[currentArg] = result[currentArg].Concat(new string[] { item }).ToArray();
                }
            }

            return result;
        }

    }
}
