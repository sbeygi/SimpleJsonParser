class NumberLiteral : CodeBlock
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
