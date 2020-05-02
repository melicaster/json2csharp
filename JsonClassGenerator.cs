// Copyright © 2010 Xamasoft

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace Xamasoft.JsonClassGenerator
{
    public class JsonClassGenerator : IJsonClassGeneratorConfig
    {
        public string Example { get; set; }
        public string TargetFolder { get; set; }
        public string Namespace { get; set; }
        public string SecondaryNamespace { get; set; }
        public bool UseProperties { get; set; }
        public bool InternalVisibility { get; set; }
        public bool ExplicitDeserialization { get; set; }
        public bool NoHelperClass { get; set; }
        public string MainClass { get; set; }
        public bool SortMemberFields { get; set; }
        public bool UsePascalCase { get; set; }
        public bool UseNestedClasses { get; set; }
        public bool ApplyObfuscationAttributes { get; set; }
        public bool SingleFile { get; set; }
        public ICodeWriter CodeWriter { get; set; }
        public TextWriter OutputStream { get; set; }
        public bool AlwaysUseNullableValues { get; set; }
        public bool ExamplesInDocumentation { get; set; }

        //private PluralizationService pluralizationService = PluralizationService.CreateService(new CultureInfo("en-us"));

        private bool used = false;
        public bool UseNamespaces { get { return Namespace != null; } }

        public void GenerateClasses()
        {
            if (CodeWriter == null) CodeWriter = new CSharpCodeWriter();
            if (ExplicitDeserialization && !(CodeWriter is CSharpCodeWriter)) throw new ArgumentException("Explicit deserialization is obsolete and is only supported by the C# provider.");

            if (used) throw new InvalidOperationException("This instance of JsonClassGenerator has already been used. Please create a new instance.");
            used = true;


            var writeToDisk = TargetFolder != null;
            if (writeToDisk && !Directory.Exists(TargetFolder)) Directory.CreateDirectory(TargetFolder);


            JObject[] examples;
            var example = Example.StartsWith("HTTP/") ? Example.Substring(Example.IndexOf("\r\n\r\n")) : Example;
            using (var sr = new StringReader(example))
            using (var reader = new JsonTextReader(sr))
            {
                var json = JToken.ReadFrom(reader);
                if (json is JArray)
                {
                    examples = ((JArray)json).Cast<JObject>().ToArray();
                }
                else if (json is JObject)
                {
                    examples = new[] { (JObject)json };
                }
                else
                {
                    throw new Exception("Sample JSON must be either a JSON array, or a JSON object.");
                }
            }


            Types = new List<JsonType>();
            Names.Add(MainClass);
            var rootType = new JsonType(this, examples[0]);
            rootType.IsRoot = true;
            rootType.AssignName(MainClass);
            GenerateClass(examples, rootType);

            if (writeToDisk)
            {

                //var parentFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //if (writeToDisk && !NoHelperClass && ExplicitDeserialization) File.WriteAllBytes(Path.Combine(TargetFolder, "JsonClassHelper.cs"), Properties.Resources.JsonClassHelper);
                //if (SingleFile)
                //{
                //    WriteClassesToFile(Path.Combine(TargetFolder, MainClass + CodeWriter.FileExtension), Types);
                //}
                //else
                //{

                //    foreach (var type in Types)
                //    {
                //        var folder = TargetFolder;
                //        if (!UseNestedClasses && !type.IsRoot && SecondaryNamespace != null)
                //        {
                //            var s = SecondaryNamespace;
                //            if (s.StartsWith(Namespace + ".")) s = s.Substring(Namespace.Length + 1);
                //            folder = Path.Combine(folder, s);
                //            Directory.CreateDirectory(folder);
                //        }
                //        WriteClassesToFile(Path.Combine(folder, (UseNestedClasses && !type.IsRoot ? MainClass + "." : string.Empty) + type.AssignedName + CodeWriter.FileExtension), new[] { type });
                //    }
                //}
            }
            else if (OutputStream != null)
            {
                WriteClassesToFile(OutputStream, Types);
            }

        }

        private void WriteClassesToFile(string path, IEnumerable<JsonType> types)
        {
            //using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            //{
            //    WriteClassesToFile(sw, types);
            //}
        }

        private void WriteClassesToFile(TextWriter sw, IEnumerable<JsonType> types)
        {
            var inNamespace = false;
            var rootNamespace = false;

            CodeWriter.WriteFileStart(this, sw);
            foreach (var type in types)
            {
                if (UseNamespaces && inNamespace && rootNamespace != type.IsRoot && SecondaryNamespace != null) { CodeWriter.WriteNamespaceEnd(this, sw, rootNamespace); inNamespace = false; }
                if (UseNamespaces && !inNamespace) { CodeWriter.WriteNamespaceStart(this, sw, type.IsRoot); inNamespace = true; rootNamespace = type.IsRoot; }
                CodeWriter.WriteClass(this, sw, type);
            }
            if (UseNamespaces && inNamespace) CodeWriter.WriteNamespaceEnd(this, sw, rootNamespace);
            CodeWriter.WriteFileEnd(this, sw);
        }


        private void GenerateClass(JObject[] examples, JsonType type)
        {
            var jsonFields = new Dictionary<string, JsonType>();
            var fieldExamples = new Dictionary<string, IList<object>>();

            var first = true;

            foreach (var obj in examples)
            {
                foreach (var prop in obj.Properties())
                {
                    JsonType fieldType;
                    var currentType = new JsonType(this, prop.Value);
                    var propName = prop.Name;
                    if (jsonFields.TryGetValue(propName, out fieldType))
                    {

                        var commonType = fieldType.GetCommonType(currentType);

                        jsonFields[propName] = commonType;
                    }
                    else
                    {
                        var commonType = currentType;
                        if (first) commonType = commonType.MaybeMakeNullable(this);
                        else commonType = commonType.GetCommonType(JsonType.GetNull(this));
                        jsonFields.Add(propName, commonType);
                        fieldExamples[propName] = new List<object>();
                    }
                    var fe = fieldExamples[propName];
                    var val = prop.Value;
                    if (val.Type == JTokenType.Null || val.Type == JTokenType.Undefined)
                    {
                        if (!fe.Contains(null))
                        {
                            fe.Insert(0, null);
                        }
                    }
                    else
                    {
                        var v = val.Type == JTokenType.Array || val.Type == JTokenType.Object ? val : val.Value<object>();
                        if (!fe.Any(x => v.Equals(x)))
                            fe.Add(v);
                    }
                }
                first = false;
            }

            if (UseNestedClasses)
            {
                foreach (var field in jsonFields)
                {
                    Names.Add(field.Key.ToLower());
                }
            }

            foreach (var field in jsonFields)
            {
                var fieldType = field.Value;
                if (fieldType.Type == JsonTypeEnum.Object)
                {
                    var subexamples = new List<JObject>(examples.Length);
                    foreach (var obj in examples)
                    {
                        JToken value;
                        if (obj.TryGetValue(field.Key, out value))
                        {
                            if (value.Type == JTokenType.Object)
                            {
                                subexamples.Add((JObject)value);
                            }
                        }
                    }

                    fieldType.AssignName(CreateUniqueClassName(field.Key));
                    GenerateClass(subexamples.ToArray(), fieldType);
                }

                if (fieldType.InternalType != null && fieldType.InternalType.Type == JsonTypeEnum.Object)
                {
                    var subexamples = new List<JObject>(examples.Length);
                    foreach (var obj in examples)
                    {
                        JToken value;
                        if (obj.TryGetValue(field.Key, out value))
                        {
                            if (value.Type == JTokenType.Array)
                            {
                                foreach (var item in (JArray)value)
                                {
                                    if (!(item is JObject)) throw new NotSupportedException("Arrays of non-objects are not supported yet.");
                                    subexamples.Add((JObject)item);
                                }

                            }
                            else if (value.Type == JTokenType.Object)
                            {
                                foreach (var item in (JObject)value)
                                {
                                    if (!(item.Value is JObject)) throw new NotSupportedException("Arrays of non-objects are not supported yet.");

                                    subexamples.Add((JObject)item.Value);
                                }
                            }
                        }
                    }

                    field.Value.InternalType.AssignName(CreateUniqueClassNameFromPlural(field.Key));
                    GenerateClass(subexamples.ToArray(), field.Value.InternalType);
                }
            }

            type.Fields = jsonFields.Select(x => new FieldInfo(this, x.Key, x.Value, UsePascalCase, fieldExamples[x.Key])).ToArray();

            Types.Add(type);

        }

        public IList<JsonType> Types { get; private set; }
        private HashSet<string> Names = new HashSet<string>();

        private string CreateUniqueClassName(string name)
        {
            name = ToTitleCase(name);

            var finalName = name;
            var i = 2;
            while (Names.Any(x => x.Equals(finalName, StringComparison.OrdinalIgnoreCase)))
            {
                finalName = name + i.ToString();
                i++;
            }

            Names.Add(finalName);
            return finalName;
        }

        private string CreateUniqueClassNameFromPlural(string plural)
        {
            plural = ToTitleCase(plural);
            //return CreateUniqueClassName(pluralizationService.Singularize(plural)); //TODO: find a replacement for Data.Plurizationservice
            return CreateUniqueClassName(plural);
        }



        internal static string ToTitleCase(string str)
        {
            var sb = new StringBuilder(str.Length);
            var flag = true;

            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(flag ? char.ToUpper(c) : c);
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }

            return sb.ToString();
        }

        public bool HasSecondaryClasses
        {
            get { return Types.Count > 1; }
        }

        public static readonly string[] FileHeader = new[] {
            "Generated by Xamasoft JSON Class Generator",
            "http://www.xamasoft.com/json-class-generator"
        };
    }

    public interface IJsonClassGeneratorConfig
    {
        string Namespace { get; set; }
        string SecondaryNamespace { get; set; }
        bool UseProperties { get; set; }
        bool InternalVisibility { get; set; }
        bool ExplicitDeserialization { get; set; }
        bool NoHelperClass { get; set; }
        string MainClass { get; set; }
        bool SortMemberFields { get; set; }
        bool UsePascalCase { get; set; }
        bool UseNestedClasses { get; set; }
        bool ApplyObfuscationAttributes { get; set; }
        bool SingleFile { get; set; }
        ICodeWriter CodeWriter { get; set; }
        bool HasSecondaryClasses { get; }
        bool AlwaysUseNullableValues { get; set; }
        bool UseNamespaces { get; }
        bool ExamplesInDocumentation { get; set; }
    }

    public interface ICodeWriter
    {
        string FileExtension { get; }
        string DisplayName { get; }
        string GetTypeName(JsonType type, IJsonClassGeneratorConfig config);
        void WriteClass(IJsonClassGeneratorConfig config, TextWriter sw, JsonType type);
        void WriteFileStart(IJsonClassGeneratorConfig config, TextWriter sw);
        void WriteFileEnd(IJsonClassGeneratorConfig config, TextWriter sw);
        void WriteNamespaceStart(IJsonClassGeneratorConfig config, TextWriter sw, bool root);
        void WriteNamespaceEnd(IJsonClassGeneratorConfig config, TextWriter sw, bool root);
    }

    public class JsonType
    {


        private JsonType(IJsonClassGeneratorConfig generator)
        {
            this.generator = generator;
        }

        public JsonType(IJsonClassGeneratorConfig generator, JToken token)
            : this(generator)
        {

            Type = GetFirstTypeEnum(token);

            if (Type == JsonTypeEnum.Array)
            {
                var array = (JArray)token;
                InternalType = GetCommonType(generator, array.ToArray());
            }
        }

        internal static JsonType GetNull(IJsonClassGeneratorConfig generator)
        {
            return new JsonType(generator, JsonTypeEnum.NullableSomething);
        }

        private IJsonClassGeneratorConfig generator;

        internal JsonType(IJsonClassGeneratorConfig generator, JsonTypeEnum type)
            : this(generator)
        {
            this.Type = type;
        }


        public static JsonType GetCommonType(IJsonClassGeneratorConfig generator, JToken[] tokens)
        {

            if (tokens.Length == 0) return new JsonType(generator, JsonTypeEnum.NonConstrained);

            var common = new JsonType(generator, tokens[0]).MaybeMakeNullable(generator);

            for (int i = 1; i < tokens.Length; i++)
            {
                var current = new JsonType(generator, tokens[i]);
                common = common.GetCommonType(current);
            }

            return common;

        }

        internal JsonType MaybeMakeNullable(IJsonClassGeneratorConfig generator)
        {
            if (!generator.AlwaysUseNullableValues) return this;
            return this.GetCommonType(JsonType.GetNull(generator));
        }


        public JsonTypeEnum Type { get; private set; }
        public JsonType InternalType { get; private set; }
        public string AssignedName { get; private set; }


        public void AssignName(string name)
        {
            AssignedName = name;
        }



        public bool MustCache
        {
            get
            {
                switch (Type)
                {
                    case JsonTypeEnum.Array: return true;
                    case JsonTypeEnum.Object: return true;
                    case JsonTypeEnum.Anything: return true;
                    case JsonTypeEnum.Dictionary: return true;
                    case JsonTypeEnum.NonConstrained: return true;
                    default: return false;
                }
            }
        }

        public string GetReaderName()
        {
            if (Type == JsonTypeEnum.Anything || Type == JsonTypeEnum.NullableSomething || Type == JsonTypeEnum.NonConstrained)
            {
                return "ReadObject";
            }
            if (Type == JsonTypeEnum.Object)
            {
                return string.Format("ReadStronglyTypedObject<{0}>", AssignedName);
            }
            else if (Type == JsonTypeEnum.Array)
            {
                return string.Format("ReadArray<{0}>", InternalType.GetTypeName());
            }
            else
            {
                return string.Format("Read{0}", Enum.GetName(typeof(JsonTypeEnum), Type));
            }
        }

        public JsonType GetInnermostType()
        {
            if (Type != JsonTypeEnum.Array) throw new InvalidOperationException();
            if (InternalType.Type != JsonTypeEnum.Array) return InternalType;
            return InternalType.GetInnermostType();
        }


        public string GetTypeName()
        {
            return generator.CodeWriter.GetTypeName(this, generator);
        }

        public string GetJTokenType()
        {
            switch (Type)
            {
                case JsonTypeEnum.Boolean:
                case JsonTypeEnum.Integer:
                case JsonTypeEnum.Long:
                case JsonTypeEnum.Float:
                case JsonTypeEnum.Date:
                case JsonTypeEnum.NullableBoolean:
                case JsonTypeEnum.NullableInteger:
                case JsonTypeEnum.NullableLong:
                case JsonTypeEnum.NullableFloat:
                case JsonTypeEnum.NullableDate:
                case JsonTypeEnum.String:
                    return "JValue";
                case JsonTypeEnum.Array:
                    return "JArray";
                case JsonTypeEnum.Dictionary:
                    return "JObject";
                case JsonTypeEnum.Object:
                    return "JObject";
                default:
                    return "JToken";

            }
        }

        public JsonType GetCommonType(JsonType type2)
        {
            var commonType = GetCommonTypeEnum(this.Type, type2.Type);

            if (commonType == JsonTypeEnum.Array)
            {
                if (type2.Type == JsonTypeEnum.NullableSomething) return this;
                if (this.Type == JsonTypeEnum.NullableSomething) return type2;
                var commonInternalType = InternalType.GetCommonType(type2.InternalType).MaybeMakeNullable(generator);
                if (commonInternalType != InternalType) return new JsonType(generator, JsonTypeEnum.Array) { InternalType = commonInternalType };
            }


            //if (commonType == JsonTypeEnum.Dictionary)
            //{
            //    var commonInternalType = InternalType.GetCommonType(type2.InternalType);
            //    if (commonInternalType != InternalType) return new JsonType(JsonTypeEnum.Dictionary) { InternalType = commonInternalType };
            //}


            if (this.Type == commonType) return this;
            return new JsonType(generator, commonType).MaybeMakeNullable(generator);
        }


        private static bool IsNull(JsonTypeEnum type)
        {
            return type == JsonTypeEnum.NullableSomething;
        }



        private JsonTypeEnum GetCommonTypeEnum(JsonTypeEnum type1, JsonTypeEnum type2)
        {
            if (type1 == JsonTypeEnum.NonConstrained) return type2;
            if (type2 == JsonTypeEnum.NonConstrained) return type1;

            switch (type1)
            {
                case JsonTypeEnum.Boolean:
                    if (IsNull(type2)) return JsonTypeEnum.NullableBoolean;
                    if (type2 == JsonTypeEnum.Boolean) return type1;
                    break;
                case JsonTypeEnum.NullableBoolean:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Boolean) return type1;
                    break;
                case JsonTypeEnum.Integer:
                    if (IsNull(type2)) return JsonTypeEnum.NullableInteger;
                    if (type2 == JsonTypeEnum.Float) return JsonTypeEnum.Float;
                    if (type2 == JsonTypeEnum.Long) return JsonTypeEnum.Long;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    break;
                case JsonTypeEnum.NullableInteger:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Float) return JsonTypeEnum.NullableFloat;
                    if (type2 == JsonTypeEnum.Long) return JsonTypeEnum.NullableLong;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    break;
                case JsonTypeEnum.Float:
                    if (IsNull(type2)) return JsonTypeEnum.NullableFloat;
                    if (type2 == JsonTypeEnum.Float) return type1;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    if (type2 == JsonTypeEnum.Long) return type1;
                    break;
                case JsonTypeEnum.NullableFloat:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Float) return type1;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    if (type2 == JsonTypeEnum.Long) return type1;
                    break;
                case JsonTypeEnum.Long:
                    if (IsNull(type2)) return JsonTypeEnum.NullableLong;
                    if (type2 == JsonTypeEnum.Float) return JsonTypeEnum.Float;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    break;
                case JsonTypeEnum.NullableLong:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Float) return JsonTypeEnum.NullableFloat;
                    if (type2 == JsonTypeEnum.Integer) return type1;
                    if (type2 == JsonTypeEnum.Long) return type1;
                    break;
                case JsonTypeEnum.Date:
                    if (IsNull(type2)) return JsonTypeEnum.NullableDate;
                    if (type2 == JsonTypeEnum.Date) return JsonTypeEnum.Date;
                    break;
                case JsonTypeEnum.NullableDate:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Date) return type1;
                    break;
                case JsonTypeEnum.NullableSomething:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.String) return JsonTypeEnum.String;
                    if (type2 == JsonTypeEnum.Integer) return JsonTypeEnum.NullableInteger;
                    if (type2 == JsonTypeEnum.Float) return JsonTypeEnum.NullableFloat;
                    if (type2 == JsonTypeEnum.Long) return JsonTypeEnum.NullableLong;
                    if (type2 == JsonTypeEnum.Boolean) return JsonTypeEnum.NullableBoolean;
                    if (type2 == JsonTypeEnum.Date) return JsonTypeEnum.NullableDate;
                    if (type2 == JsonTypeEnum.Array) return JsonTypeEnum.Array;
                    if (type2 == JsonTypeEnum.Object) return JsonTypeEnum.Object;
                    break;
                case JsonTypeEnum.Object:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Object) return type1;
                    if (type2 == JsonTypeEnum.Dictionary) throw new ArgumentException();
                    break;
                case JsonTypeEnum.Dictionary:
                    throw new ArgumentException();
                //if (IsNull(type2)) return type1;
                //if (type2 == JsonTypeEnum.Object) return type1;
                //if (type2 == JsonTypeEnum.Dictionary) return type1;
                //  break;
                case JsonTypeEnum.Array:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.Array) return type1;
                    break;
                case JsonTypeEnum.String:
                    if (IsNull(type2)) return type1;
                    if (type2 == JsonTypeEnum.String) return type1;
                    break;
            }

            return JsonTypeEnum.Anything;

        }

        private static bool IsNull(JTokenType type)
        {
            return type == JTokenType.Null || type == JTokenType.Undefined;
        }



        private static JsonTypeEnum GetFirstTypeEnum(JToken token)
        {
            var type = token.Type;
            if (type == JTokenType.Integer)
            {
                if ((long)((JValue)token).Value < int.MaxValue) return JsonTypeEnum.Integer;
                else return JsonTypeEnum.Long;

            }
            switch (type)
            {
                case JTokenType.Array: return JsonTypeEnum.Array;
                case JTokenType.Boolean: return JsonTypeEnum.Boolean;
                case JTokenType.Float: return JsonTypeEnum.Float;
                case JTokenType.Null: return JsonTypeEnum.NullableSomething;
                case JTokenType.Undefined: return JsonTypeEnum.NullableSomething;
                case JTokenType.String: return JsonTypeEnum.String;
                case JTokenType.Object: return JsonTypeEnum.Object;
                case JTokenType.Date: return JsonTypeEnum.Date;

                default: return JsonTypeEnum.Anything;

            }
        }


        public IList<FieldInfo> Fields { get; internal set; }
        public bool IsRoot { get; internal set; }
    }

    public enum JsonTypeEnum
    {
        Anything,
        String,
        Boolean,
        Integer,
        Long,
        Float,
        Date,
        NullableInteger,
        NullableLong,
        NullableFloat,
        NullableBoolean,
        NullableDate,
        Object,
        Array,
        Dictionary,
        NullableSomething,
        NonConstrained
    }

    public class FieldInfo
    {

        public FieldInfo(IJsonClassGeneratorConfig generator, string jsonMemberName, JsonType type, bool usePascalCase, IList<object> Examples)
        {
            this.generator = generator;
            this.JsonMemberName = jsonMemberName;
            this.MemberName = jsonMemberName;
            if (usePascalCase) MemberName = JsonClassGenerator.ToTitleCase(MemberName);
            this.Type = type;
            this.Examples = Examples;
        }
        private IJsonClassGeneratorConfig generator;
        public string MemberName { get; private set; }
        public string JsonMemberName { get; private set; }
        public JsonType Type { get; private set; }
        public IList<object> Examples { get; private set; }

        public string GetGenerationCode(string jobject)
        {
            var field = this;
            if (field.Type.Type == JsonTypeEnum.Array)
            {
                var innermost = field.Type.GetInnermostType();
                return string.Format("({1})JsonClassHelper.ReadArray<{5}>(JsonClassHelper.GetJToken<JArray>({0}, \"{2}\"), JsonClassHelper.{3}, typeof({6}))",
                    jobject,
                    field.Type.GetTypeName(),
                    field.JsonMemberName,
                    innermost.GetReaderName(),
                    -1,
                    innermost.GetTypeName(),
                    field.Type.GetTypeName()
                    );
            }
            else if (field.Type.Type == JsonTypeEnum.Dictionary)
            {

                return string.Format("({1})JsonClassHelper.ReadDictionary<{2}>(JsonClassHelper.GetJToken<JObject>({0}, \"{3}\"))",
                    jobject,
                    field.Type.GetTypeName(),
                    field.Type.InternalType.GetTypeName(),
                    field.JsonMemberName,
                    field.Type.GetTypeName()
                    );
            }
            else
            {
                return string.Format("JsonClassHelper.{1}(JsonClassHelper.GetJToken<{2}>({0}, \"{3}\"))",
                    jobject,
                    field.Type.GetReaderName(),
                    field.Type.GetJTokenType(),
                    field.JsonMemberName);
            }

        }



        public string GetExamplesText()
        {
            return string.Join(", ", Examples.Take(5).Select(x => JsonConvert.SerializeObject(x)).ToArray());
        }

    }
}
