using Object.Build.Implementation;
using ObjectStringMapping.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ObjectStringMap.Implementation
{
    public class StringMap<TObject> : 
        IStringMap<TObject>
    {
        const string NameGroupKey = "Name";

        const string FormatGroupKey = "Format";

        const string ThisKeyword = "this";

        static readonly Regex NodePattern = new Regex(
            @"\{(?<Name>[^}]+?)(:(?<Format>[^}]+)){0,1}}",
            RegexOptions.Compiled);

        readonly Lazy<ParseInfo> _parseInfo;

        public StringMap(string source)
        {
            Source = source;

            _parseInfo = new Lazy<ParseInfo>(() => ResolveParseInfo(source));
        }

        public static implicit operator StringMap<TObject>(string source)
        {
            return new StringMap<TObject>(source);
        }

        public static implicit operator string(StringMap<TObject> stringMap)
        {
            return stringMap?.Source;
        }

        public string Source { get; }

        public override string ToString()
        {
            return Source;
        }

        public TObject Map(string str)
        {
            var parseInfo = _parseInfo.Value;

            var match = parseInfo.Regex.Match(str);

            if (!match.Success)
            {
                return default(TObject);
            }

            var thisGroup = match.Groups[ThisKeyword];

            var format = default(string);

            var obj = default(TObject);

            if (thisGroup.Success)
            {
                parseInfo.Formats.TryGetValue(ThisKeyword, out format);

                var typedValue = TypeString(
                    typeof(TObject),
                    thisGroup.Value,
                    format);

                if(typedValue == null)
                {
                    return default(TObject);
                }

                obj = (TObject)typedValue;
            }
            else
            {
                var builder = new Builder<TObject>();

                foreach (var property in typeof(TObject).GetProperties())
                {
                    var name = property.Name;

                    var value = match.Groups[name].Value;

                    format = parseInfo.Formats.TryGetValue(name, out format) ? format : null;

                    var typedValue = TypeString(
                        property.PropertyType,
                        value,
                        format);

                    if(typedValue == null)
                    {
                        return default(TObject);
                    }

                    builder.Set(property.Name, typedValue);
                }

                obj = builder.Build();
            }

            return obj;
        }

        public string Map(
            TObject obj, 
            bool allowPartialMap = false)
        {
            var output = new StringBuilder();

            var nodeMatches = NodePattern.Matches(Source).Cast<Match>();

            var index = 0;

            foreach (var nodeMatch in nodeMatches)
            {
                if (nodeMatch.Index > index)
                {
                    output.Append(Source.Substring(index, nodeMatch.Index - index));
                }

                var name = nodeMatch.Groups[NameGroupKey].Value;

                var format = nodeMatch.Groups[FormatGroupKey].Value;

                var value = ResolveValue(obj, name, format);

                if (value == null)
                {
                    if (allowPartialMap)
                    {
                        return output.ToString();
                    }
                    else
                    {
                        throw new ArgumentNullException(
                            name, 
                            $"A required value was null while mapping {typeof(TObject).Name} to a string.");
                    }
                }

                output.Append(value);

                index = nodeMatch.Index + nodeMatch.Length;
            }

            if (index < Source.Length)
            {
                output.Append(Source.Substring(index));
            }

            return output.ToString();
        }

        public bool IsMatch(string str)
        {
            return _parseInfo.Value.Regex.IsMatch(str);
        }

        static string ResolveValue(
            TObject obj, 
            string name,
            string format)
        {
            var value = default(object);

            if(name == ThisKeyword)
            {
                value = obj;
            }
            else
            {
                value = typeof(TObject)
                    .GetProperty(name)
                    .GetValue(obj);
            }

            if (value == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(format))
            {
                var formattable = value as IFormattable;

                if (formattable == null)
                {
                    throw new ArgumentException($"{name} has a format but is not IFormattable.");
                }

                return formattable.ToString(format, null);
            }

            if(value is Guid)
            {
                return ((Guid)value).ToString("N");
            }

            return value.ToString();
        }

        static string ResolvePattern(string name)
        {
            return $"(?<{name}>.*)";
        }

        static object TypeString(
            Type type, 
            string stringValue,
            string format)
        {
            try
            {
                if (type.Equals(typeof(Guid)) || type.Equals(typeof(Guid?)))
                {
                    return Guid.Parse(stringValue);
                }
                else if ((type.Equals(typeof(DateTime)) || type.Equals(typeof(DateTime?))) &&
                    !string.IsNullOrWhiteSpace(format))
                {
                    return DateTime.ParseExact(stringValue, format, null);
                }
                else
                {
                    if (type.IsGenericType &&
                        type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        type = type.GetGenericArguments().Single();
                    }

                    return Convert.ChangeType(stringValue, type);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        static ParseInfo ResolveParseInfo(string source)
        {
            var pattern = new StringBuilder();

            var nodeMatches = NodePattern.Matches(source).Cast<Match>();

            var index = 0;

            var formats = new Dictionary<string, string>();

            foreach (var nodeMatch in nodeMatches)
            {
                if (nodeMatch.Index > index)
                {
                    pattern.Append(Regex.Escape(source.Substring(index, nodeMatch.Index - index)));
                }

                var nodeName = nodeMatch.Groups[NameGroupKey].Value;

                var nodeFormat = nodeMatch.Groups[FormatGroupKey].Value;

                if (!string.IsNullOrWhiteSpace(nodeFormat))
                {
                    formats.Add(nodeName, nodeFormat);
                }

                var nodePattern = ResolvePattern(nodeName);

                pattern.Append(nodePattern);

                index = nodeMatch.Index + nodeMatch.Length;
            }

            if (index < source.Length)
            {
                pattern.Append(Regex.Escape(source.Substring(index)));
            }

            var regex = new Regex(
                pattern.ToString(),
                RegexOptions.Compiled);

            return new ParseInfo(formats, regex);
        }

        class ParseInfo
        {
            public ParseInfo(
                Dictionary<string, string> formats,
                Regex regex)
            {
                Formats = formats;

                Regex = regex;
            }

            public Dictionary<string, string> Formats { get; }

            public Regex Regex { get; }
        }
    }
}
