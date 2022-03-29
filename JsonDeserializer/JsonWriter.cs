using System;
using System.Collections.Generic;

class JsonWriter
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
