using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Jil;
using Newtonsoft.Json;

partial class MainClass
{

	public static string json = "{\"name\":{\"first\":\"Robert\",\"middle\":\"\",\"last\":\"Smith\"},\"age\":25,\"DOB\":\"-\",\"hobbies\":[\"running\",\"coding\",\"-\"],\"education\":{\"highschool\":\"N\\/A\",\"college\":\"Yale\"}}";

	static void Main()
	{
		var data = JsonParser.Parse(json);
		var output = JsonSerializer.Serialize(data);

		Thread.Sleep(1000);

		Stopwatch sp = new Stopwatch();

		Console.Write("JsonParser: ");
		sp.Start();
		for (int i = 0; i < 1000; i++)
		{
			JsonParser.Parse(json);
		}
		sp.Stop();
		Console.WriteLine(sp.ElapsedMilliseconds);
		

		Console.Write("Newtonsoft: ");
		sp.Restart();
		for (int i = 0; i < 1000; i++)
		{
			// Newtonsoft
			JsonConvert.DeserializeObject(json);
		}
		sp.Stop();
		Console.WriteLine(sp.ElapsedMilliseconds);

		Console.Write("Jil: ");
		sp.Restart();
		for (int i = 0; i < 1000; i++)
		{
			// Jil
			JSON.DeserializeDynamic(json);
		}
		sp.Stop();
		Console.WriteLine(sp.ElapsedMilliseconds);


		Console.ReadLine();
	}
}