using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AlloyEngine
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    internal sealed class KeyValueObjectAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal sealed class KeyFieldAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal sealed class KeyValueAttribute : Attribute { }

    internal static class YamlSerializer
    {
        public static string Serialize(object obj) => new Serializer().Run(obj);
        public static object Deserialize(string yaml, Type t) => new Deserializer(yaml).Run(t);
        public static object Deserialize<T>(string yaml) => new Deserializer(yaml).Run(typeof(T));

        // ================================================================
        // SERIALIZER
        // ================================================================
        private sealed class Serializer
        {
            private readonly StringBuilder _sb = new();
            private const int SpacesPerLevel = 2;

            public string Run(object obj)
            {
                SerializeValue(obj, 0);
                return _sb.ToString().TrimEnd();
            }

            private void SerializeValue(object value, int level)
            {
                if (value == null) { WriteLine("null", level); return; }

                var type = value.GetType();

                if (IsSimple(type) || type.IsEnum)
                {
                    WriteScalar(value, level);
                    return;
                }

                if (IsDictionary(type))
                {
                    SerializeDictionary((IDictionary)value, level);
                    return;
                }

                if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
                {
                    SerializeSequence((IEnumerable)value, level);
                    return;
                }

                if (IsKeyValueObject(type))
                {
                    SerializeKeyValueObject(value, level);
                    return;
                }

                SerializeObject(value, level);
            }

            private void SerializeObject(object obj, int level)
            {
                var type = obj.GetType();
                var members = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                  .Cast<MemberInfo>()
                                  .Concat(type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                              .Where(p => p.CanRead));

                foreach (var m in members)
                {
                    var val = m switch
                    {
                        FieldInfo f => f.GetValue(obj),
                        PropertyInfo p => p.GetValue(obj),
                        _ => null
                    };

                    Indent(level);
                    _sb.Append(m.Name).Append(": ");
                    if (val == null || IsSimple(val.GetType()) || val.GetType().IsEnum)
                        WriteScalarInline(val);
                    else
                    {
                        _sb.AppendLine();
                        SerializeValue(val, level + 1);
                    }
                }
            }

            private void SerializeKeyValueObject(object obj, int level)
            {
                var type = obj.GetType();
                var keyFld = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                 .FirstOrDefault(f => f.GetCustomAttribute<KeyFieldAttribute>() != null);
                var valFld = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                 .FirstOrDefault(f => f.GetCustomAttribute<KeyValueAttribute>() != null);

                if (keyFld == null || valFld == null)
                    throw new InvalidOperationException($"Type {type.Name} missing KeyField/KeyValue");

                var key = keyFld.GetValue(obj);
                var val = valFld.GetValue(obj);

                Indent(level);
                _sb.Append(Convert.ToString(key, CultureInfo.InvariantCulture)).Append(": ");
                if (val == null || IsSimple(val.GetType()) || val.GetType().IsEnum)
                    WriteScalarInline(val);
                else
                {
                    _sb.AppendLine();
                    SerializeValue(val, level + 1);
                }
            }

            private void SerializeDictionary(IDictionary dict, int level)
            {
                foreach (DictionaryEntry e in dict)
                {
                    Indent(level);
                    _sb.Append(Convert.ToString(e.Key, CultureInfo.InvariantCulture)).Append(": ");
                    var val = e.Value;
                    if (val == null || IsSimple(val.GetType()) || val.GetType().IsEnum)
                        WriteScalarInline(val);
                    else
                    {
                        _sb.AppendLine();
                        SerializeValue(val, level + 1);
                    }
                }
            }

            private void SerializeSequence(IEnumerable seq, int level)
            {
                foreach (var item in seq)
                {
                    Indent(level);
                    _sb.Append("- ");
                    if (item == null || IsSimple(item.GetType()) || item.GetType().IsEnum)
                        WriteScalarInline(item);
                    else
                    {
                        _sb.AppendLine();
                        SerializeValue(item, level + 1);
                    }
                }
            }

            private void WriteScalar(object value, int level)
            {
                Indent(level);
                WriteScalarInline(value);
                _sb.AppendLine();
            }

            private void WriteScalarInline(object value)
            {
                if (value == null) { _sb.Append("null"); return; }
                if (value is string s)
                {
                    if (s.Contains('\n'))
                    {
                        _sb.AppendLine("|");
                        foreach (var line in s.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
                        {
                            Indent(CurrentLevel() + 1);
                            _sb.AppendLine(line);
                        }
                    }
                    else
                    {
                        AppendEscaped(s);
                    }
                }
                else if (value is bool b) _sb.Append(b ? "true" : "false");
                else if (value is DateTime dt) _sb.Append(dt.ToString("o"));
                else _sb.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
            }

            private void AppendEscaped(string s)
            {
                if (s.All(c => c >= 32 && c <= 126 && c != '"' && c != '\\')) { _sb.Append(s); return; }
                _sb.Append('"');
                foreach (var c in s)
                {
                    switch (c)
                    {
                        case '"': _sb.Append("\\\""); break;
                        case '\\': _sb.Append("\\\\"); break;
                        case '\n': _sb.Append("\\n"); break;
                        case '\r': _sb.Append("\\r"); break;
                        case '\t': _sb.Append("\\t"); break;
                        default: _sb.Append(c); break;
                    }
                }
                _sb.Append('"');
            }

            private void WriteLine(string text, int level) { Indent(level); _sb.AppendLine(text); }
            private void Indent(int level) => _sb.Append(' ', level * SpacesPerLevel);
            private int CurrentLevel() => 0;

            private static bool IsSimple(Type t) =>
                t.IsPrimitive || t == typeof(string) || t == typeof(decimal) ||
                t == typeof(DateTime) || t == typeof(Guid) || t == typeof(Version) || t == typeof(TimeSpan);

            private static bool IsDictionary(Type t) =>
                typeof(IDictionary).IsAssignableFrom(t) ||
                (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>));

            private static bool IsKeyValueObject(Type t) =>
                t.GetCustomAttribute<KeyValueObjectAttribute>() != null;
        }

        // ================================================================
        // DESERIALIZER
        // ================================================================
        private sealed class Deserializer
        {
            private readonly string[] _lines;
            private int _pos;
            private const int SpacesPerLevel = 2;

            public Deserializer(string yaml)
            {
                if (string.IsNullOrWhiteSpace(yaml)) throw new ArgumentException("YAML is empty");
                _lines = yaml.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
            }

            public object Run(Type targetType) => DeserializeValue(targetType, 0);

            private object DeserializeValue(Type type, int indent)
            {
                if (_pos >= _lines.Length) throw new InvalidOperationException("Unexpected end of YAML");

                string llp = _lines[_pos];
                int actual = CountIndent(llp);
                if (actual < indent) return null;
                if (actual > indent) throw new InvalidOperationException("Indentation too deep");

                string trimmed = llp.Trim();
                if (trimmed == "null") { _pos++; return null; }

                if (IsSimple(type)) { _pos++; return ParseSimple(type, trimmed); }
                if (type.IsEnum) { _pos++; return Enum.Parse(type, trimmed, false); }

                if (IsKeyValueObject(type))
                {
                    var (keyStr, valObj) = ParseKeyValueLine(llp, indent);
                    _pos++;

                    var obj = Activator.CreateInstance(type);
                    var keyFld = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                     .First(f => f.GetCustomAttribute<KeyFieldAttribute>() != null);
                    var valFld = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                     .First(f => f.GetCustomAttribute<KeyValueAttribute>() != null);

                    keyFld.SetValue(obj, Convert.ChangeType(keyStr, keyFld.FieldType, CultureInfo.InvariantCulture));
                    valFld.SetValue(obj, valObj);
                    return obj;
                }

                if (IsDictionary(type))
                {
                    var (keyType, valueType) = GetDictionaryTypes(type);
                    var dict = (IDictionary)Activator.CreateInstance(type);

                    while (_pos < _lines.Length && CountIndent(_lines[_pos]) >= indent + SpacesPerLevel)
                    {
                        var (kStr, vObj) = ParseDictionaryEntry(indent + SpacesPerLevel, valueType);
                        var keyObj = Convert.ChangeType(kStr, keyType, CultureInfo.InvariantCulture);
                        dict[keyObj] = vObj;
                    }
                    return dict;
                }

                if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
                {
                    var elementType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];
                    var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

                    while (_pos < _lines.Length && CountIndent(_lines[_pos]) >= indent + SpacesPerLevel)
                    {
                        if (!llp.TrimStart().StartsWith("-")) break;
                        var item = ParseListItem(indent + SpacesPerLevel, elementType);
                        list.Add(item);
                    }

                    if (type.IsArray)
                    {
                        var arr = Array.CreateInstance(elementType, list.Count);
                        list.CopyTo(arr, 0);
                        return arr;
                    }
                    return list;
                }

                // Regular object
                var instance = Activator.CreateInstance(type);
                while (_pos < _lines.Length && CountIndent(_lines[_pos]) >= indent + SpacesPerLevel)
                {
                    var (name, value) = ParseProperty(indent + SpacesPerLevel);
                    var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field != null) { field.SetValue(instance, value); continue; }

                    var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (prop != null && prop.CanWrite) prop.SetValue(instance, value);
                }
                return instance;
            }

            private (string key, object value) ParseKeyValueLine(string llp, int indent)
            {
                int colonIdx = llp.IndexOf(':', CountIndent(llp));
                if (colonIdx < 0) throw new InvalidOperationException("Missing ':' in KeyValue line");

                string key = llp.Substring(CountIndent(llp), colonIdx - CountIndent(llp)).Trim();
                string rest = llp.Substring(colonIdx + 1).Trim();

                if (string.IsNullOrWhiteSpace(rest))
                {
                    _pos++;
                    var valueType = typeof(object); // will be resolved by caller
                    return (key, DeserializeValue(valueType, indent + SpacesPerLevel));
                }

                return (key, ParseSimpleScalar(rest));
            }

            private (string key, object value) ParseDictionaryEntry(int indent, Type valueType)
            {
                string llp = _lines[_pos];
                int actual = CountIndent(llp);
                if (actual < indent) throw new InvalidOperationException("Bad indent");

                int colonIdx = llp.IndexOf(':', actual);
                if (colonIdx < 0) throw new InvalidOperationException("Missing ':' in dict entry");

                string key = llp.Substring(actual, colonIdx - actual).Trim();
                string rest = llp.Substring(colonIdx + 1).Trim();

                object val;
                if (string.IsNullOrWhiteSpace(rest))
                {
                    _pos++;
                    val = DeserializeValue(valueType, indent);
                }
                else
                {
                    val = ParseSimpleScalar(rest);
                    _pos++;
                }
                return (key, val);
            }

            private object ParseListItem(int indent, Type elementType)
            {
                string llp = _lines[_pos];
                if (!llp.TrimStart().StartsWith("- "))
                    throw new InvalidOperationException("Expected '-' for list item");

                int dashPos = llp.IndexOf('-');
                string afterDash = llp.Substring(dashPos + 1).Trim();

                if (string.IsNullOrWhiteSpace(afterDash))
                {
                    _pos++;
                    return DeserializeValue(elementType, indent);
                }

                _pos++;
                return ParseSimpleScalar(afterDash);
            }

            private (string name, object value) ParseProperty(int indent)
            {
                string llp = _lines[_pos];
                int actual = CountIndent(llp);
                if (actual < indent) throw new InvalidOperationException("Bad indent in property");

                int colonIdx = llp.IndexOf(':', actual);
                if (colonIdx < 0) throw new InvalidOperationException("Missing ':' in property");

                string name = llp.Substring(actual, colonIdx - actual).Trim();
                string rest = llp.Substring(colonIdx + 1).Trim();

                object val;
                if (string.IsNullOrWhiteSpace(rest))
                {
                    _pos++;
                    val = DeserializeValue(typeof(object), indent);
                }
                else
                {
                    val = ParseSimpleScalar(rest);
                    _pos++;
                }
                return (name, val);
            }

            private object ParseSimpleScalar(string s)
            {
                if (s == "null") return null;
                if (s == "true") return true;
                if (s == "false") return false;

                if (s.StartsWith("|"))
                {
                    var sb = new StringBuilder();
                    int baseIndent = CountIndent(_lines[_pos]);
                    _pos++;

                    while (_pos < _lines.Length)
                    {
                        string llp = _lines[_pos];
                        int ind = CountIndent(llp);
                        if (ind <= baseIndent) break;
                        sb.AppendLine(llp.Substring(baseIndent));
                        _pos++;
                    }

                    if (sb.Length > 0) sb.Length -= Environment.NewLine.Length;
                    return sb.ToString();
                }

                if (int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out int i)) return i;
                if (long.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out long l)) return l;
                if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double d)) return d;
                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal m)) return m;
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime dt)) return dt;
                if (Guid.TryParse(s, out Guid g)) return g;
                if (TimeSpan.TryParse(s, CultureInfo.InvariantCulture, out TimeSpan ts)) return ts;

                if ((s.StartsWith("\"") && s.EndsWith("\"")) || (s.StartsWith("'") && s.EndsWith("'")))
                    s = s.Substring(1, s.Length - 2);

                return s;
            }

            private object ParseSimple(Type type, string value)
            {
                if (type == typeof(string)) return value;
                if (type == typeof(bool)) return bool.Parse(value);
                if (type == typeof(int)) return int.Parse(value, CultureInfo.InvariantCulture);
                if (type == typeof(long)) return long.Parse(value, CultureInfo.InvariantCulture);
                if (type == typeof(float)) return float.Parse(value, CultureInfo.InvariantCulture);
                if (type == typeof(double)) return double.Parse(value, CultureInfo.InvariantCulture);
                if (type == typeof(decimal)) return decimal.Parse(value, CultureInfo.InvariantCulture);
                if (type == typeof(DateTime)) return DateTime.Parse(value, CultureInfo.InvariantCulture);
                if (type == typeof(Guid)) return Guid.Parse(value);
                if (type == typeof(Version)) return new Version(value);
                if (type == typeof(TimeSpan)) return TimeSpan.Parse(value, CultureInfo.InvariantCulture);
                throw new NotSupportedException($"Simple type {type} not supported");
            }

            private static int CountIndent(string s)
            {
                int i = 0;
                while (i < s.Length && s[i] == ' ') i++;
                return i;
            }

            private static (Type Key, Type Value) GetDictionaryTypes(Type t)
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    return (t.GenericTypeArguments[0], t.GenericTypeArguments[1]);

                var iface = t.GetInterfaces()
                             .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
                return iface != null
                    ? (iface.GenericTypeArguments[0], iface.GenericTypeArguments[1])
                    : (typeof(object), typeof(object));
            }

            private static bool IsSimple(Type t) =>
                t.IsPrimitive || t == typeof(string) || t == typeof(decimal) ||
                t == typeof(DateTime) || t == typeof(Guid) || t == typeof(Version) || t == typeof(TimeSpan);

            private static bool IsDictionary(Type t) =>
                typeof(IDictionary).IsAssignableFrom(t) ||
                (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>));

            private static bool IsKeyValueObject(Type t) =>
                t.GetCustomAttribute<KeyValueObjectAttribute>() != null;
        }
    }
}
