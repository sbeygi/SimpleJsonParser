using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

partial class MainClass
{

	public static string x = "{\"name\":{\"first\":\"Robert\",\"middle\":\"\",\"last\":\"Smith\"},\"age\":25,\"DOB\":\"-\",\"hobbies\":[\"running\",\"coding\",\"-\"],\"education\":{\"highschool\":\"N\\/A\",\"college\":\"Yale\"}}";
	public static string xs = "{\"name\":{\"first\":\"Robert\",\"middle\":\"\",\"last\":\"Smith\"},\"age\":25,\"DOB\":\"-\",\"hobbies\":[\"running\",\"coding\",\"-\"],\"education\":{\"highschool\":\"N\\/A\",\"college\":\"Yale\",\"se\":[{\"a\":1},{\"b\":2}]}}";
	public static string xh = "{\"i\":[{\"a\":1},{\"b\":2}]}";
	public static string json = "";
	static void Main()
	{
		WebRequest request = WebRequest.Create("https://coderbyte.com/api/challenges/json/json-cleaning");
		using (WebResponse response = request.GetResponse())
		{
			using (StreamReader reader = new StreamReader(response.GetResponseStream()))
			{
				json = reader.ReadToEnd();
			}
		}

		JsonSerializer.IgnoredItems.Add("N/A", "");
		JsonSerializer.IgnoredItems.Add("-", "");
		JsonSerializer.IgnoredItems.Add("", "");

		var data = JsonParser.Parse(json);
		var output = JsonSerializer.Serialize(data);

		//output = JsonConvert.SerializeObject(data, Formatting.Indented);
		Console.WriteLine(output);
		Console.ReadLine();
	}
}