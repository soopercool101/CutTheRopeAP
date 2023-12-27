using System;
using System.Collections;
using CutTheRope.ios;

internal class DynamicArrayEnumerator : IEnumerator
{
	public NSObject[] _map;

	private int _highestIndex;

	private int position = -1;

	object IEnumerator.Current
	{
		get
		{
			return Current;
		}
	}

	public NSObject Current
	{
		get
		{
			try
			{
				return _map[position];
			}
			catch (IndexOutOfRangeException)
			{
				throw new InvalidOperationException();
			}
		}
	}

	public DynamicArrayEnumerator(NSObject[] list, int highestIndex)
	{
		_map = list;
		_highestIndex = highestIndex;
	}

	public bool MoveNext()
	{
		position++;
		return position < _highestIndex + 1;
	}

	public void Reset()
	{
		position = -1;
	}
}
