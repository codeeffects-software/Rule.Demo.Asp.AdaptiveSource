using CodeEffects.Rule.Editor.Client;
using CodeEffects.Rule.Editor.Models;

namespace CodeEffects.Demo.Asp.AdaptiveSource;

public class Menu : IMenu
{
	public string? CacheID { get; set; }

	public ICollection<Field> Fields { get; set; } = [];
	public ICollection<Function> Functions { get; set; } = [];
}