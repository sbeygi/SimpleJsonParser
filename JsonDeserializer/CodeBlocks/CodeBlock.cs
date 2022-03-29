class CodeBlock
{
	private readonly string key;
	protected readonly string openBlock;
	protected readonly string closeBlock;
	protected readonly JsonWriter writer;

	public CodeBlock(JsonWriter writer)
	{
		this.writer = writer;
	}

	public CodeBlock(string openBlock, string closeBlock, JsonWriter writer)
	{
		this.openBlock = openBlock;
		this.closeBlock = closeBlock;
		this.writer = writer;

		Open();
	}

	public CodeBlock(string key, string openBlock, string closeBlock, JsonWriter writer)
	{
		this.key = key;
		this.openBlock = openBlock;
		this.closeBlock = closeBlock;
		this.writer = writer;

		Open();
	}

	public virtual void Open()
	{
		writer.WriteKey(key);
		writer.Write(openBlock);
	}

	public void Close()
	{
		writer.Write(closeBlock);
	}
}
