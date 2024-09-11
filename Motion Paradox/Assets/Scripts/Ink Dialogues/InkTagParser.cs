using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class InkTagParser
{
	// Indexer.
	public string this[InkTagKey key]
	{
		get { return _tags[key]; }
	}

	// Private fields.
	private Dictionary<InkTagKey, string> _tags;
	private char _delimiter;

	public InkTagParser(char delimiter)
	{
		_tags = new Dictionary<InkTagKey, string>()
		{
			[InkTagKey.Speaker] = "",
			[InkTagKey.Portrait] = "",
			[InkTagKey.Layout] = ""
		};

		_delimiter = delimiter;
	}

	public bool TryUpdateTags(IList<string> tagList)
	{
		bool anyDifferent = false;

		foreach (string tag in tagList)
		{
			string[] tagContent = tag.Trim().Split(_delimiter);

			if (tagContent.Length != 2)
			{
				Debug.LogWarning($"Failed to split tag '{tag}' into key-value pairs, probably an invalid delimiter was provided," +
									"or the tag was incorrectly formatted!");
				continue;
			}

			string key = tagContent[0].Trim();

			if (Enum.TryParse(key, ignoreCase: true, out InkTagKey tagKey))
			{
				string value = tagContent[1].Trim();
				
				if (!_tags[tagKey].Equals(value))
				{
					_tags[tagKey] = value;
					anyDifferent = true;
				}
			}
			else
			{
				Debug.LogWarning($"Failed to parse tag '{tag}', the provided key '{key}' was invalid, ignoring...");
			}
		}
		
		return anyDifferent;
	}
}

public enum InkTagKey
{
	Speaker,
	Portrait,
	Layout
}