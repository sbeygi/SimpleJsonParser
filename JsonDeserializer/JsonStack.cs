using System;
using System.Collections.Generic;


public class JsonStack
{
	Stack<object> refs = new Stack<object>();
	Stack<ElementType> refTypes = new Stack<ElementType>();
	public Dictionary<string, object> root = new Dictionary<string, object>();

	string lastItemKey = "";
	StackState stackState = StackState.PushingKey;

	Dictionary<string, object> lastDictRef { get { return (Dictionary<string, object>)refs.Peek(); } }
	List<object> lastListRef { get { return (List<object>)refs.Peek(); } }
	ElementType lastRefType { get { return refTypes.Peek(); } }

	public JsonStack()
	{
		refs.Push(root);
		refTypes.Push(ElementType.Object);
	}

	internal void Push(object value)
	{
		switch (stackState)
		{
			case StackState.PushingValue:
				switch (lastRefType)
				{
					case ElementType.Object:
						lastDictRef[lastItemKey] = value;
						break;
					case ElementType.Array:
						lastListRef.Add(value);
						break;
					default:
						break;
				}
				break;
			default:
				lastItemKey = (string)value;
				break;
		}
	}

	public void AddElement(string key, ElementType type)
	{
		Object elm = null;

		switch (type)
		{
			case ElementType.Object:
				elm = new Dictionary<string, object>();
				break;
			case ElementType.Array:
				elm = new List<object>();
				break;
			default:
				break;
		}

		switch (lastRefType)
		{
			case ElementType.Array:
				lastListRef.Add(elm);
				break;
			default:
				lastDictRef.Add(key, elm);
				break;
		}

		refs.Push(elm);
		refTypes.Push(type);
	}

	public void OpenValue()
	{
		stackState = StackState.PushingValue;
	}

	public void CloseValue()
	{
		if (lastRefType == ElementType.Object)
			stackState = StackState.PushingKey;
		else
			stackState = StackState.PushingValue;
	}

	public void OpenArray()
	{
		AddElement(lastItemKey, ElementType.Array);
		stackState = StackState.PushingValue;
	}

	public void CloseArray()
	{
		PopStack();
	}

	public void OpenObject()
	{
		AddElement(lastItemKey, ElementType.Object);
		stackState = StackState.PushingKey;
	}

	public void CloseObject()
	{
		PopStack();
	}

	public void PopStack()
	{
		refs.Pop();
		refTypes.Pop();
	}
}
