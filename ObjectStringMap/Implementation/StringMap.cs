﻿using ObjectStringMapping.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ObjectStringMap.Implementation
{
    public class StringMap<TObject> : 
        IStringMap<TObject>
        where TObject : new()
    {
        readonly Regex NodePattern = new Regex(
            @"\{(?<Name>[^}]+?)(:(?<Format>[^}]+)){0,1}}",
            RegexOptions.Compiled);

        public StringMap(string mapSource)
        {
            MapSource = mapSource;
        }

        public static implicit operator StringMap<TObject>(string mapSource)
        {
            return new StringMap<TObject>(mapSource);
        }

        public static implicit operator string(StringMap<TObject> objectStringMap)
        {
            return objectStringMap?.MapSource;
        }

        public string MapSource { get; }

        public TObject Map(string str)
        {
            var pattern = new StringBuilder();

            var nodeMatches = NodePattern.Matches(MapSource).Cast<Match>();

            var index = 0;

            var formats = new Dictionary<string, string>();

            foreach (var nodeMatch in nodeMatches)
            {
                if (nodeMatch.Index > index)
                {
                    pattern.Append(Regex.Escape(MapSource.Substring(index, nodeMatch.Index - index)));
                }

                var nodeName = nodeMatch.Groups["Name"].Value;

                var nodeFormat = nodeMatch.Groups["Format"].Value;

                if (!string.IsNullOrWhiteSpace(nodeFormat))
                {
                    formats.Add(nodeName, nodeFormat);
                }

                var nodePattern = ResolvePattern(nodeName);

                pattern.Append(nodePattern);

                index = nodeMatch.Index + nodeMatch.Length;
            }

            if (index < MapSource.Length)
            {
                pattern.Append(Regex.Escape(MapSource.Substring(index)));
            }

            var match = Regex.Match(str, pattern.ToString());

            var obj = default(TObject);

            var thisGroup = match.Groups["this"];

            var format = default(string);

            if (thisGroup.Success)
            {
                formats.TryGetValue("this", out format);

                obj = (TObject)TypeString(
                    typeof(TObject),
                    thisGroup.Value,
                    format);
            }
            else
            {
                obj = new TObject();

                foreach (var property in typeof(TObject).GetProperties())
                {
                    var name = match.Groups[property.Name].Value;

                    format = formats.TryGetValue(name, out format) ? format : null;

                    var typedValue = TypeString(
                        property.PropertyType,
                        name,
                        format);

                    property.SetValue(obj, typedValue);
                }
            }

            return obj;
        }

        public string Map(
            TObject obj, 
            bool allowPartialMap = false)
        {
            var output = new StringBuilder();

            var nodeMatches = NodePattern.Matches(MapSource).Cast<Match>();

            var index = 0;

            foreach (var nodeMatch in nodeMatches)
            {
                if (nodeMatch.Index > index)
                {
                    output.Append(MapSource.Substring(index, nodeMatch.Index - index));
                }

                var name = nodeMatch.Groups["Name"].Value;

                var format = nodeMatch.Groups["Format"].Value;

                var value = ResolveValue(obj, name, format);

                if (value == null)
                {
                    if (allowPartialMap)
                    {
                        return output.ToString();
                    }
                    else
                    {
                        throw new ArgumentNullException(name);
                    }
                }

                output.Append(value);

                index = nodeMatch.Index + nodeMatch.Length;
            }

            if (index < MapSource.Length)
            {
                output.Append(MapSource.Substring(index));
            }

            return output.ToString();
        }

        static string ResolveValue(
            TObject obj, 
            string name,
            string format)
        {
            var value = default(object);

            if(name == "this")
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
            if (type.Equals(typeof(Guid)) || type.Equals(typeof(Guid?)))
            {
                return Guid.Parse(stringValue);
            }
            else if((type.Equals(typeof(DateTime)) || type.Equals(typeof(DateTime?))) &&
                !string.IsNullOrWhiteSpace(format))
            {
                return DateTime.ParseExact(stringValue, format, null);
            }
            else
            {
                return Convert.ChangeType(stringValue, type);
            }
        }
}
}