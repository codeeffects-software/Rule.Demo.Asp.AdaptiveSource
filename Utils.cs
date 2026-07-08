using CodeEffects.Rule.Common.Attributes;
using CodeEffects.Rule.Editor.Client;

namespace CodeEffects.Demo.Asp.AdaptiveSource;

public class Utils
{
	[Method(DisplayName = "External Method")]
	public bool ExternalMethod(string str)
	{
		return str != default && str.Length > 0;
	}

	[Action(DisplayName = "External Action")]
	public void ExternalAction(string test)
	{
		// Action logic goes here
	}

	public static List<DataSourceItem> Schools()
	{
		var schools = new List<DataSourceItem>();

		schools.Add(new DataSourceItem("One", "School #1"));
		schools.Add(new DataSourceItem("Two", "School #2"));

		return schools;
	}

	public static List<DataSourceItem> Managers()
	{
		var managers = new List<DataSourceItem>();

		managers.Add(new DataSourceItem(22, "Elvira Clayton"));
		managers.Add(new DataSourceItem(29, "Jack Smith"));
		managers.Add(new DataSourceItem(54, "Michael Jackson"));

		return managers;
	}

	public static List<DataSourceItem> Doctors()
	{
		var doctors = new List<DataSourceItem>();

		doctors.Add(new DataSourceItem("abc", "Derek Brown"));
		doctors.Add(new DataSourceItem("xyz", "Lilia Johnson"));
		doctors.Add(new DataSourceItem("opr", "Sam Jefferson"));

		return doctors;
	}
}