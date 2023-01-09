using Serilog;
using Serilog.Events;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace HomeAutomationServer.Data;

/// <summary> Generic XML Serializer with exception handling & logging </summary>
/// <typeparam name="T"> Is a type of configuration class has the attribute [Serializable()]</typeparam>
public class XMLConfigurationSerializer<T> where T : new()
{
    private static XmlSerializer Serializer { get; } = new(typeof(T));
    private static void Log(LogEventLevel logLevel, string message) => Serilog.Log.Logger.Write(logLevel, message);
    private static void Log(Exception exception) => Serilog.Log.Logger.Write(LogEventLevel.Error, $"Exception: {exception.Message}");

    /// <summary> Serialize generic to XML file with pretty printing </summary>
    public static void Serialize(T configuration, string outputPath)
    {
        Log(LogEventLevel.Debug, $"Saving {typeof(T)} to path: {outputPath}");

        try
        {
            File.WriteAllText(outputPath, Serialize(configuration));
        }
        catch (Exception ex)
        {
            Log(LogEventLevel.Error, $"Failed to save {typeof(T)} to path: {outputPath}");
            Log(ex);
        }
    }

    private static string Serialize(T configuration)
    {
        var sb = new StringBuilder();

        XmlWriterSettings settings = new()
        {
            Indent = true,
            OmitXmlDeclaration = true,
            NewLineOnAttributes = true,
        };

        using var writer = XmlWriter.Create(sb, settings);

        Serializer.Serialize(writer, configuration);

        return sb.ToString();
    }

    /// <summary> Deserialize generic to XML from file </summary>
    public static bool TryDeserialize(string inputPath, out T configuration)
    {
        Log(LogEventLevel.Debug, $"Reading {typeof(T)} from path : {inputPath}");

        try
        {
            using StreamReader reader = File.OpenText(inputPath);

            if (Serializer.Deserialize(reader) is T config)
            {
                configuration = config;
                return true;
            }
        }
        catch (Exception ex)
        {
            Log(LogEventLevel.Error, $"Failed to read {typeof(T)} from path: {inputPath}");
            Log(ex);
        }

        configuration = new();
        return false;
    }

    /// <summary> For printing to more human readable format (e.g. console) </summary>
    public static string Print(T configuration)
    {
        StringBuilder sb = new();

        sb.AppendLine($"--- {typeof(T)} ---");
        sb.AppendLine("");

        string xmlString = "";

        try
        {
            xmlString = Serialize(configuration);

            foreach (string line in xmlString.Split('\n')[1..^1]) // skip first and last lines
            {
                sb.AppendLine(PrettyXMLLine(line));
            }
        }
        catch (Exception ex)
        {
            Log(LogEventLevel.Error, "Failed to pretty print config");
            Log(ex);
            return xmlString;
        }

        return sb.ToString().Trim('\n');
    }


    /// <summary> Pretty print, remove angle brackets and end of attributes </summary>
    private static string PrettyXMLLine(string line)
    {
        string result = line;

        // remove end tag
        int endTagIndex = result.IndexOf("</");
        if (endTagIndex > -1) result = result[0..endTagIndex];

        // empty-element tag
        result = result.Replace("/>", "");

        // cleanup start tag
        int startTagIndex = result.IndexOf("<");
        if (startTagIndex > -1)
        {
            result = result[0..startTagIndex] + " " + result[(startTagIndex + 1)..^0];

            startTagIndex = result.IndexOf(">");
            if (startTagIndex > -1)
            {
                result = result[0..startTagIndex] + ": " + result[(startTagIndex + 1)..^0];
            }
        }

        return result;
    }
}
