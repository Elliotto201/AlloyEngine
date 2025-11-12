using AlloyEngine;

namespace AlloyEditor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "yaml_tests_output.txt";
            using (var sw = new StreamWriter(path))
            {
                var simple = new SimpleClass { Number = 42, Text = "Hello\nWorld", Flag = true };
                var nested = new NestedClass { Inner = simple, Rate = 3.14, Timestamp = DateTime.Now };
                var complex = new ComplexClass
                {
                    NestedList = new List<NestedClass> { nested, nested },
                    NamedItems = new Dictionary<string, SimpleClass> { { "A", simple }, { "B", simple } },
                    KeyValPair = new MyKeyValue { Key = "MyKey", Value = 123 },
                    AppVersion = new Version(1, 2, 3, 4)
                };

                sw.WriteLine("--- SimpleClass ---");
                string simpleYaml = YamlSerializer.Serialize(simple);
                sw.WriteLine(simpleYaml);

                sw.WriteLine("--- NestedClass ---");
                string nestedYaml = YamlSerializer.Serialize(nested);
                sw.WriteLine(nestedYaml);

                sw.WriteLine("--- ComplexClass ---");
                string complexYaml = YamlSerializer.Serialize(complex);
                sw.WriteLine(complexYaml);

                // Deserialize back to check
                var deserializedSimple = YamlSerializer.Deserialize<SimpleClass>(simpleYaml);
                var deserializedNested = YamlSerializer.Deserialize<NestedClass>(nestedYaml);
                var deserializedComplex = YamlSerializer.Deserialize<ComplexClass>(complexYaml);
            }

            Console.WriteLine($"YAML test output written to {Path.GetFullPath(path)}");
        }
    }
}

[KeyValueObject]
public struct MyKeyValue
{
    [KeyField] public string Key;
    [KeyValue] public int Value;
}

public class SimpleClass
{
    public int Number;
    public string Text;
    public bool Flag;
}

public class NestedClass
{
    public SimpleClass Inner;
    public double Rate;
    public DateTime Timestamp;
}

public class ComplexClass
{
    public List<NestedClass> NestedList;
    public Dictionary<string, SimpleClass> NamedItems;
    public MyKeyValue KeyValPair;
    public Version AppVersion;
}