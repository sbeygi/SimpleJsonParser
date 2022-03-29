static class JsonReader
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
