using System.Xml.Linq;
using CodeEffects.Rule.Common.Models;
using CodeEffects.Rule.Editor.Client;
using CodeEffects.Rule.Editor.Models;
using CodeEffects.Rule.Editor.Utils;

namespace CodeEffects.Demo.Asp.AdaptiveSource;

public sealed class MenuProvider : IMenuProvider
{
	private ICollection<Type> types = null!;

	private Lazy<Index<Field>> fields = null!;
	private Lazy<Index<Function>> inRuleMethods = null!;
	private Lazy<Index<Function>> actionMethods = null!;
	private Lazy<Index<DataSource>> dataSources = null!;

	public string SourceObjectTypeOrAliasName { get; }

	public MenuProvider(Type sourceObject)
	{
		if(sourceObject == null)
			throw new ArgumentNullException(nameof(sourceObject));

		this.SourceObjectTypeOrAliasName = Source.GetName(sourceObject, TypeNameKind.AssemblyQualifiedName);
		this.Initialize(sourceObject);
	}

	public IMenu GetFieldsAndMethods(Context context)
	{
		var fieldNames = new List<string>();
		var methodNames = new List<string>();

		// Get the name of the last field on rule's context
		var field = context.Conditions.LastOrDefault(c => c.Name != default)?.Name ?? "Patient.";
		string cache = null!;

		if(field == "Patient.Visits" || field.StartsWith("Visit."))
		{
			cache = "Visit Fields";

			fieldNames.Add("Visit.ID");
			fieldNames.Add("Visit.DoctorID");
			fieldNames.Add("Visit.Date");
			fieldNames.Add("Visit.Type");

			methodNames.Add("Visit.TestMethod");
		}
		else if(field == "Patient.Hospitals" || field.Contains("Hospital."))
		{
			cache = "Hospital Fields";

			fieldNames.Add("Hospital.ID");
			fieldNames.Add("Hospital.Name");
			fieldNames.Add("Hospital.ManagerID");
		}
		else
		{
			cache = "Patient Fields";

			fieldNames.Add("Patient.ID");
			fieldNames.Add("Patient.FirstName");
			fieldNames.Add("Patient.LastName");
			fieldNames.Add("Patient.DOB");
			fieldNames.Add("Patient.IsActive");
			fieldNames.Add("Patient.Gender");
			fieldNames.Add("Patient.Salary");
			fieldNames.Add("Patient.TotalInvoiced");
			fieldNames.Add("Patient.TotalPaid");
			fieldNames.Add("Patient.EducationID");

			fieldNames.Add("Patient.Home.Street");
			fieldNames.Add("Patient.Home.City");
			fieldNames.Add("Patient.Home.Zip");
			fieldNames.Add("Patient.Home.State");

			fieldNames.Add("Patient.Work.Street");
			fieldNames.Add("Patient.Work.City");
			fieldNames.Add("Patient.Work.Zip");
			fieldNames.Add("Patient.Work.State");

			fieldNames.Add("Patient.Nicknames");
			fieldNames.Add("Patient.Visits");
			fieldNames.Add("Patient.Hospitals");

			methodNames.Add("Patient.FullName");
			methodNames.Add("Patient.GetAiProbability");
			methodNames.Add("Patient.CalculateTotalDebt");
			methodNames.Add("Patient.ExternalMethod");
		}

		var menu = new Menu { CacheID = cache };

		if(methodNames.Count > 0) menu.Functions = Select(Pick(true), methodNames);
		menu.Fields = Select(fields.Value, fieldNames);

		return menu;
	}
	public IMenu GetSettersAndActions(Context context)
	{
		var actionNames = new List<string>
			{
				"Patient.Accept",
				"Patient.Decline",
				"Patient.ExternalAction"
			};

		var setterNames = new List<string>
			{
				"Patient.FirstName",
				"Patient.LastName",
				"Patient.DOB",
				"Patient.IsActive",
				"Patient.Gender",
				"Patient.Salary",
				"Patient.TotalInvoiced",
				"Patient.TotalPaid",
				"Patient.EducationID",

				"Patient.Home.Street",
				"Patient.Home.City",
				"Patient.Home.Zip",
				"Patient.Home.State",

				"Patient.Work.Street",
				"Patient.Work.City",
				"Patient.Work.Zip",
				"Patient.Work.State",

				"Patient.Nicknames"
			};

		var menu = new Menu { CacheID = "Patient Actions" };

		if(actionNames.Count > 0) menu.Functions = Select(Pick(false), actionNames);
		menu.Fields = Select(fields.Value, setterNames);

		return menu;
	}

	public Field GetField(string fieldName)
	{
		fields.Value.ByName.TryGetValue(fieldName, out var field);
		return field!;
	}
	public Field GetSetter(string fieldName)
	{
		fields.Value.ByName.TryGetValue(fieldName, out var field);
		return field!;
	}
	public Field GetReusableRule(string ruleID)
	{
		if(!RuleStorage.Storage.TryGetValue(ruleID, out var value))
			throw new KeyNotFoundException($"No rule with ID '{ruleID}'.");

		if(string.IsNullOrEmpty(value.Xml)) value.Xml = Xml.GetEmptyRuleDocument().OuterXml;

		// Get the name of the rule (if it's in the doc)
		XNamespace ce = "https://codeeffects.com/schemas/rule/6";
		XDocument? xdoc = XDocument.Parse(value.Xml);

		string name = xdoc.Descendants(ce + "name").FirstOrDefault()?.Value ?? ruleID;

		var field = new Field
		{
			Value = ruleID,
			DisplayName = name,
			IsRule = true,
			Nullable = false,
			DataType = DataType.Bool,
			ValueInputType = ValueInputType.User
		};

		return field;
	}
	public Function GetMethod(string methodName)
	{
		Pick(true).ByName.TryGetValue(methodName, out var function);
		return function!;
	}
	public Function GetAction(string methodName)
	{
		Pick(false).ByName.TryGetValue(methodName, out var function);
		return function!;
	}
	public CodeEffects.Rule.Editor.Client.Enum GetEnum(string enumName)
	{
		return Source.GetEnum(Type.GetType(enumName, true), enumName);
	}
	public DataSource GetDataSource(string dataSourceName)
	{
		if(!dataSources.Value.ByName.TryGetValue(dataSourceName, out var dataSource))
			throw new Exception("Expected dynamic menu data source cannot be located: " + dataSourceName);

		switch(dataSourceName)
		{
			case "Education":

				dataSource.Items.Add(new DataSourceItem("One", "School #1"));
				dataSource.Items.Add(new DataSourceItem("Two", "School #2"));
				break;

			case "Attempts":

				dataSource.Items.Add(new DataSourceItem(11, "Attempt #1"));
				dataSource.Items.Add(new DataSourceItem(22, "Attempt #2"));
				dataSource.Items.Add(new DataSourceItem(33, "Attempt #3"));
				break;

			case "Doctors":

				dataSource.Items.Add(new DataSourceItem("1b", "Derek Brown"));
				dataSource.Items.Add(new DataSourceItem("2e", "Lilia Johnson"));
				dataSource.Items.Add(new DataSourceItem("3e", "Sam Jefferson"));
				break;

			case "Managers":

				dataSource.Items.Add(new DataSourceItem(22, "Elvira Clayton"));
				dataSource.Items.Add(new DataSourceItem(29, "Jack Smith"));
				dataSource.Items.Add(new DataSourceItem(54, "Michael Jackson"));
				break;

			default: throw new Exception("Unknown data source: " + dataSourceName);
		}

		return dataSource;
	}

	private void Initialize(Type mainType)
	{
		types = Source.GetSourceTypes(mainType);

		fields = Defer(() => BuildIndex(LoadAll<Field>(t => Source.GetFields(t, TypeNameKind.AssemblyQualifiedName)), f => f.Value));
		inRuleMethods = Defer(() => BuildIndex(LoadAll<Function>(t => Source.GetMethods(t, true, TypeNameKind.AssemblyQualifiedName)), fn => fn.Value));
		actionMethods = Defer(() => BuildIndex(LoadAll<Function>(t => Source.GetMethods(t, false, TypeNameKind.AssemblyQualifiedName)), fn => fn.Value));
		dataSources = Defer(() => BuildIndex(LoadAll<DataSource>(t => Source.GetDataSources(t)), ds => ds.Name));
	}
	private ICollection<T> LoadAll<T>(Func<Type, ICollection<T>> loader)
	{
		var all = new List<T>();

		foreach(var type in types)
			all.AddRange(loader(type));

		return all;
	}
	private Index<Function> Pick(bool inRuleMethods)
		=> (inRuleMethods ? this.inRuleMethods : this.actionMethods).Value;
	private static Lazy<T> Defer<T>(Func<T> factory)
		=> new Lazy<T>(factory, LazyThreadSafetyMode.ExecutionAndPublication);
	private static Index<T> BuildIndex<T>(ICollection<T> items, Func<T, string> keySelector)
	{
		var byName = new Dictionary<string, T>(items.Count, StringComparer.Ordinal);

		foreach(var item in items)
			byName[keySelector(item)] = item;

		return new Index<T>(items, byName);
	}
	private static ICollection<T> Select<T>(Index<T> index, IEnumerable<string> names)
	{
		if(names == null) return index.Ordered;

		var result = new List<T>();

		foreach(var n in names)
		{
			if(index.ByName.TryGetValue(n, out var item))
				result.Add(item);
		}

		return result.AsReadOnly();
	}

	private sealed class Index<T>
	{
		public ICollection<T> Ordered { get; }
		public Dictionary<string, T> ByName { get; }

		public Index(ICollection<T> ordered, Dictionary<string, T> byName)
		{
			Ordered = ordered;
			ByName = byName;
		}
	}
}