class NamedObjectCodeBlock : CodeBlock
{
	private readonly string name;


	public NamedObjectCodeBlock(string name, JsonWriter writer) : base(name, "{", "}", writer)
	{

	}
}
