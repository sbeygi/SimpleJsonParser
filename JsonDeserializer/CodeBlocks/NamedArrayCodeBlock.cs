class NamedArrayCodeBlock : CodeBlock
{
	public NamedArrayCodeBlock(string name, JsonWriter writer) : base(name, "[", "]", writer)
	{
	}
}
