using System.Collections.Concurrent;

namespace CodeEffects.Demo.Asp.AdaptiveSource
{
	public class RuleStorage
	{
		// Simple in-memory rule storage for the sake of this demo
		public static readonly ConcurrentDictionary<string, Rule> Storage = new();
	}
}