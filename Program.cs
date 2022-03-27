using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

class MainClass
{

    public static string xst = "{\"name\":{\"first\":\"Robert\",\"middle\":\"\",\"last\":\"Smith\"},\"age\":25,\"DOB\":\"-\",\"hobbies\":[55,[66],\"running\",\"coding\",\"-\"],\"education\":{\"highschool\":\"N\\/A\",\"college\":\"Yale\"}}";
    public static string xs = "{\"name\":{\"first\":\"Robert\",\"middle\":\"\",\"last\":\"Smith\"},\"age\":25,\"DOB\":\"-\",\"hobbies\":[\"running\",\"coding\",\"-\"],\"education\":{\"highschool\":\"N\\/A\",\"college\":\"Yale\",\"se\":[{\"a\":1},{\"b\":2}]}}";
    public static string x = "{\"i\":[{\"a\":1},{\"b\":2}]}";
    static void Main()
    {

        var data = JsonParser.Parse(x);
        var output = JsonSerializer.Serialize(data);

        //output = JsonConvert.SerializeObject(data, Formatting.Indented);
        Console.WriteLine(output);
        Console.ReadLine();
    }

    public static class JsonSerializer
    {
        static Dictionary<Type, TypesCache> typesCache = new Dictionary<Type, TypesCache>();
        enum TypesCache { DictionaryStringObject, Int, String, ListOfObject };

        static JsonSerializer()
        {
            typesCache.Add(typeof(Dictionary<string, object>), TypesCache.DictionaryStringObject);
            typesCache.Add(typeof(int), TypesCache.Int);
            typesCache.Add(typeof(string), TypesCache.String);
            typesCache.Add(typeof(List<object>), TypesCache.ListOfObject);
        }

        public static string Serialize(Dictionary<string, object> data)
        {
            JsonWriter writer = new JsonWriter();
            SerializeInternal(data, writer);

            return writer.Build();
        }

        private static void SerializeInternal(Dictionary<string, object> data, JsonWriter writer)
        {
            Stack<CodeBlock> stack = new Stack<CodeBlock>();

            foreach (var item in data)
            {
                switch (typesCache[item.Value.GetType()])
                {
                    case TypesCache.DictionaryStringObject:
                        if (stack.Count > 0)
                            writer.WriteSeparator();

                        stack.Push(new NamedObjectCodeBlock(item.Key, writer));

                        SerializeInternal((Dictionary<string, object>)item.Value, writer);
                        stack.Peek().Close();
                        break;
                    case TypesCache.ListOfObject:
                        if (stack.Count > 0)
                            writer.WriteSeparator();

                        stack.Push(new NamedArrayCodeBlock(item.Key, writer));

                        SerializeInternal((List<object>)item.Value, writer);
                        stack.Peek().Close();
                        break;
                    case TypesCache.Int:
                        if (stack.Count > 0)
                            writer.WriteSeparator();

                        stack.Push(new NumberLiteral(item.Key, (int)item.Value, writer));
                        break;
                    case TypesCache.String:
                        if (stack.Count > 0)
                            writer.WriteSeparator();

                        stack.Push(new StringLiteral(item.Key, (string)item.Value, writer));
                        break;
                    default:
                        break;
                }

            }
        }

        private static void SerializeInternal(List<object> data, JsonWriter writer)
        {
            Stack<CodeBlock> stack = new Stack<CodeBlock>();

            foreach (var item in data)
            {
                switch (typesCache[item.GetType()])
                {
                    case TypesCache.DictionaryStringObject:
                        if (stack.Count > 0)
                            writer.WriteSeparator();

                        stack.Push(new ObjectCodeBlock(writer));

                        SerializeInternal((Dictionary<string, object>)item, writer);
                        stack.Peek().Close();
                        break;
                    case TypesCache.ListOfObject:
                        if (stack.Count > 0)
                            writer.WriteSeparator();

                        stack.Push(new ArrayCodeBlock(writer));

                        SerializeInternal((List<object>)item, writer);
                        stack.Peek().Close();
                        break;
                    case TypesCache.Int:
                        if (stack.Count > 0)
                            writer.WriteSeparator();

                        stack.Push(new NumberLiteral((int)item, writer));
                        break;
                    case TypesCache.String:
                        if (stack.Count > 0)
                            writer.WriteSeparator();

                        stack.Push(new StringLiteral((string)item, writer));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private class JsonWriter
    {
        private List<String> chunks = new List<string>();

        public JsonWriter()
        {

        }

        public void Write(string chunk)
        {
            chunks.Add(chunk);
        }

        public void WriteInt(int num)
        {
            chunks.Add(num.ToString());
        }

        public void WriteString(string str)
        {
            chunks.Add("\"");
            chunks.Add(str.ToString());
            chunks.Add("\"");
        }

        public void WriteKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            chunks.Add("\"");
            chunks.Add(key);
            chunks.Add("\":");
        }

        public void WriteSeparator()
        {
            chunks.Add(",");
        }

        public string Build()
        {
            return string.Join("", chunks);
        }
    }

    private class CodeBlock
    {
        private readonly string key;
        protected readonly string openBlock;
        protected readonly string closeBlock;
        protected readonly JsonWriter writer;

        public CodeBlock(JsonWriter writer)
        {
            this.writer = writer;
        }

        public CodeBlock(string openBlock, string closeBlock, JsonWriter writer)
        {
            this.openBlock = openBlock;
            this.closeBlock = closeBlock;
            this.writer = writer;

            Open();
        }

        public CodeBlock(string key, string openBlock, string closeBlock, JsonWriter writer)
        {
            this.key = key;
            this.openBlock = openBlock;
            this.closeBlock = closeBlock;
            this.writer = writer;

            Open();
        }

        public virtual void Open()
        {
            writer.WriteKey(key);
            writer.Write(openBlock);
        }

        public void Close()
        {
            writer.Write(closeBlock);
        }
    }

    private class ObjectCodeBlock : CodeBlock
    {
        public ObjectCodeBlock(JsonWriter writer) : base("{", "}", writer)
        {
        }
    }

    private class NamedObjectCodeBlock : CodeBlock
    {
        private readonly string name;


        public NamedObjectCodeBlock(string name, JsonWriter writer) : base(name, "{", "}", writer)
        {
            
        }
    }

    private class ArrayCodeBlock : CodeBlock
    {
        public ArrayCodeBlock(JsonWriter writer) : base("[", "]", writer)
        {
        }
    }

    private class NamedArrayCodeBlock : CodeBlock
    {
        public NamedArrayCodeBlock(string name, JsonWriter writer) : base(name, "[", "]", writer)
        {
        }
    }

    private class NumberLiteral : CodeBlock
    {
        private readonly string key;
        private readonly int value;

        public NumberLiteral(int value, JsonWriter writer) : base(writer)
        {
            this.value = value;

            Open();
        }

        public NumberLiteral(string key, int value, JsonWriter writer) : base(writer)
        {
            this.key = key;
            this.value = value;

            Open();
        }

        public override void Open()
        {
            writer.WriteKey(key);
            writer.WriteInt(value);
        }
    }

    private class StringLiteral : CodeBlock
    {
        private readonly string key;
        private readonly string value;

        public StringLiteral(string value, JsonWriter writer) : base(writer)
        {
            this.value = value;

            Open();
        }

        public StringLiteral(string key, string value, JsonWriter writer) : base(writer)
        {
            this.key = key;
            this.value = value;

            Open();
        }

        public override void Open()
        {
            writer.WriteKey(key);
            writer.WriteString(value);
        }
    }

    public static class JsonParser
    {
        public static Dictionary<string, object> Parse(string json)
        {
            ModelBuilder builder = new ModelBuilder();

            for (int i = 0; i < json.Length;)
            {
                switch (json[i])
                {
                    case '{':
                        builder.OpenObject();
                        break;
                    case '}':
                        builder.CloseObject();
                        break;
                    case ':':
                        builder.OpenValue();
                        break;
                    case ',':
                        builder.CloseValue();
                        break;
                    case '[':
                        builder.OpenArray();
                        break;
                    case ']':
                        builder.CloseArray();
                        break;
                    case '"':
                        string value = JsonReader.ReadString(json, ref i);
                        builder.Push(value);
                        continue;
                    case '-':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        int num = JsonReader.ReadNumber(json, ref i);
                        builder.Push(num);
                        continue;
                    default:
                        break;
                }

                i++;
            }

            return builder.root;
        }
    }

    private static class JsonReader
    {
        public static string ReadString(string json, ref int index)
        {
            string result = "";
            int qoutes = 0;

            while (index < json.Length && qoutes < 2)
            {
                switch (json[index])
                {
                    case '"':
                        qoutes++;
                        break;
                    case '\\':
                        break;
                    default:
                        result += json[index];
                        break;
                }

                index++;
            }

            return result;
        }

        public static int ReadNumber(string json, ref int index)
        {
            int result = 0;
            int multiplier = 10;
            int sign = 1;

            if (json[index] == '-')
            {
                sign = -1;
                index++;
            }

            while (index < json.Length && json[index] >= '0' && json[index] <= '9')
            {
                result = (result * multiplier) + (json[index] - 48) * sign;
                index++;
            }

            return result;
        }
    }

    private class ModelBuilder
    {
        Stack<object> refs = new Stack<object>();
        Stack<ElementType> refTypes = new Stack<ElementType>();
        public Dictionary<string, object> root = new Dictionary<string, object>();

        string lastItemKey = "";
        StackState stackState = StackState.PushingKey;

        Dictionary<string, object> lastDictRef { get { return (Dictionary<string, object>)refs.Peek(); } }
        List<object> lastListRef { get { return (List<object>)refs.Peek(); } }
        ElementType lastRefType { get { return refTypes.Peek(); } }

        public ModelBuilder()
        {
            refs.Push(root);
            refTypes.Push(ElementType.Object);
        }

        internal void Push(object value)
        {
            switch (stackState)
            {
                case StackState.PushingValue:
                    switch (lastRefType)
                    {
                        case ElementType.Object:
                            lastDictRef[lastItemKey] = value;
                            break;
                        case ElementType.Array:
                            lastListRef.Add(value);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    lastItemKey = (string)value;
                    break;
            }
        }

        public void AddElement(string key, ElementType type)
        {
            Object elm = null;

            switch (type)
            {
                case ElementType.Object:
                    elm = new Dictionary<string, object>();
                    break;
                case ElementType.Array:
                    elm = new List<object>();
                    break;
                default:
                    break;
            }

            switch (lastRefType)
            {
                case ElementType.Array:
                    lastListRef.Add(elm);
                    break;
                default:
                    lastDictRef.Add(key, elm);
                    break;
            }

            refs.Push(elm);
            refTypes.Push(type);
        }

        public void OpenValue()
        {
            stackState = StackState.PushingValue;
        }

        public void CloseValue()
        {
            if (lastRefType == ElementType.Object)
                stackState = StackState.PushingKey;
            else
                stackState = StackState.PushingValue;
        }

        public void OpenArray()
        {
            AddElement(lastItemKey, ElementType.Array);
            stackState = StackState.PushingValue;
        }

        public void CloseArray()
        {
            PopStack();
        }

        public void OpenObject()
        {
            AddElement(lastItemKey, ElementType.Object);
            stackState = StackState.PushingKey;
        }

        public void CloseObject()
        {
            PopStack();
        }

        public void PopStack()
        {
            refs.Pop();
            refTypes.Pop();
        }
    }

    public enum ElementType
    {
        Object, Array
    }

    public enum StackState
    {
        PushingValue, PushingKey
    }
}