using System;
using System.Collections.Generic;

public static class JsonSerializer
{
	public static Dictionary<string, object> IgnoredItems = new Dictionary<string, object>();
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
					if (IgnoredItems.ContainsKey((string)item.Value))
						continue;

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
					if (IgnoredItems.ContainsKey((string)item))
						continue;

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
