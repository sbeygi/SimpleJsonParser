using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

class MainClass
{

    public static string x = "{\"name\":{\"first\":\"Robert\",\"middle\":\"\",\"last\":\"Smith\"},\"age\":25,\"DOB\":\"-\",\"hobbies\":[55,\"running\",\"coding\",\"-\"],\"education\":{\"highschool\":\"N\\/A\",\"college\":\"Yale\"}}";
    public static string xf = "{\"name\":{\"first\":\"Robert\",\"middle\":\"\",\"last\":\"Smith\"},\"age\":25,\"DOB\":\"-\",\"hobbies\":[\"running\",\"coding\",\"-\"],\"education\":{\"highschool\":\"N\\/A\",\"college\":\"Yale\",\"se\":[{\"a\":1},{\"b\":2}]}}";
    public static string xg = "{\"i\":[{\"a\":1},{\"b\":2}]}";
    static void Main()
    {
        ModelBuilder builder = new ModelBuilder();

        for (int i = 0; i < x.Length;)
        {
            switch (x[i])
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
                    string value = JsonReader.ReadString(x, ref i);
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
                    int num = JsonReader.ReadNumber(x, ref i);
                    builder.Push(num);
                    continue;
                default:
                    break;
            }

            i++;
        }

        var output = JsonConvert.SerializeObject(builder.root, Formatting.Indented);
        Console.WriteLine(output);
        Console.ReadLine();
    }

    public static class JsonReader
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

    public class ModelBuilder
    {
        Stack<Dictionary<string, object>> refs = new Stack<Dictionary<string, object>>();
        Stack<ElementType> refTypes = new Stack<ElementType>();
        public Dictionary<string, object> root = new Dictionary<string, object>();

        string lastItemKey = "root";
        StackState stackState = StackState.PushingKey;

        Dictionary<string, object> lastRef { get { return refs.Peek(); } }
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
                            lastRef[lastItemKey] = value;
                            break;
                        case ElementType.Array:
                            lastRef[lastRef.Count.ToString()] = value;
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

        public Dictionary<string, object> AddElement(string key, ElementType type)
        {
            Dictionary<string, object> elm = new Dictionary<string, object>();
            if (lastRefType == ElementType.Array)
                lastRef.Add("__"+lastRef.Count.ToString(), elm);
            else
                lastRef.Add(key, elm);

            refs.Push(elm);
            refTypes.Push(type);

            return elm;
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