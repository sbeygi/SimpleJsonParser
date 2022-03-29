using System.Collections.Generic;

public static class JsonParser
{
	public static Dictionary<string, object> Parse(string json)
	{
		JsonStack builder = new JsonStack();

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
