class StringLiteral : CodeBlock
{
	private readonly string key;
	private readonly string value;

	public StringLiteral(string value, JsonWriter writer) : base(writer)
	{
		this.value = value;

		Open();
	}

	public StringLiteral(string key, string value, JsonWriter writer) : base(writer)
	{
		this.key = key;
		this.value = value;

		Open();
	}

	public override void Open()
	{
		writer.WriteKey(key);
		writer.WriteString(value);
	}
}
