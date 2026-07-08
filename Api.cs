using CodeEffects.Demo.Asp.AdaptiveSource;
using CodeEffects.Rule.Common.Models;
using CodeEffects.Rule.Editor;
using CodeEffects.Rule.Editor.Client;
using CodeEffects.Rule.Editor.Models;
using CodeEffects.Rule.Engine;
using Microsoft.AspNetCore.Mvc;

namespace CodeEffects.Demo.Asp;

public static class Api
{
	public static void MapEndpoints(this WebApplication app)
	{
		// Returns client settings for the editor
		app.MapGet("/api/settings", () =>
		{
			var settings = new Settings();
			var editor = GetControl(settings);

			// Set all evaluation type rules to be used by the
			// editor as bool properties (i.e. reusable rules) in menus
			editor.ContextMenuRules = [.. RuleStorage.Storage
				.Where(r => r.Value.Eval)
				.Select(r => new MenuItem(r.Key, r.Value.Name))];

			// Load all available rules into the Rules menu of the Toolbar (if Toolbar is used on the client)
			editor.ToolBarRules = [.. RuleStorage.Storage.Select(r => new MenuItem(r.Key, r.Value.Name))];

			// Set UI settings for the editor (Help String, etc)
			settings.GlobalData = editor.GetGlobalData();

			// Set the source-related settings for the editor
			settings.EditorData = editor.GetEditorData();

			return Results.Ok(settings);
		});

		app.MapPost("/api/menu", (MenuRequest req) =>
		{
			var editor = GetControl(new Settings());
			var data = editor.GetAdaptiveData(req.Type, req.Context);   // Load rule context into the editor
			return Results.Ok(data);
		});

		// Loads a rule by its ID
		app.MapGet("/api/load/{ruleId}", (string ruleId) =>
		{
			RuleStorage.Storage.TryGetValue(ruleId, out var value);

			var editor = GetControl(new Settings());
			editor.LoadRuleXml(value?.Xml);

			return Results.Ok(editor.GetRuleData());
		});

		app.MapPost("/api/save", ([FromBody] string rule) =>
		{
			var editor = GetControl(new Settings());
			editor.LoadRuleData(rule);

			var response = new Response();

			if(editor.IsEmpty)
			{
				response.IsRuleEmpty = true;
			}
			else if(!editor.IsValid)
			{
				response.IsRuleValid = false;

				// Load invalid elements into the editor
				response.ClientInvalidData = editor.GetInvalidData();
			}
			else
			{
				RuleStorage.Storage[editor.Rule.ID] = new AdaptiveSource.Rule
				{
					Eval = editor.Rule.IsEvalType.HasValue && editor.Rule.IsEvalType.Value,
					Name = editor.Rule.Name,
					Xml = editor.GetRuleXml()
				};

				response.Output = editor.Rule.ID;
			}

			return Results.Ok(response);
		});

		app.MapPost("/api/delete", ([FromBody] string ruleId) =>
		{
			RuleStorage.Storage.TryRemove(ruleId, out _);
		});

		app.MapPost("/api/evaluate", ([FromBody] string data) =>
		{
			var editor = GetControl(new Settings());
			editor.RuleNameIsRequired = false; // No need to validate the rule name
			editor.LoadRuleData(data);   // Load rule data into the editor

			var response = new Response();

			if(editor.IsEmpty)
			{
				response.IsRuleEmpty = true;
				response.Output = "The rule is empty";
			}
			else if(!editor.IsValid)
			{
				response.IsRuleValid = false;
				response.Output = "The rule is invalid";

				// Load invalid rule elements into the editor
				response.ClientInvalidData = editor.GetInvalidData();
			}
			else
			{
				// Init the source object with some test data
				var patient = new Patient
				{
					FirstName = "John",
					LastName = "Smith",
					DOB = new DateTime(1990, 04, 22),
					Gender = Gender.Male,
					EducationID = "Two",
					IsActive = true,
					Salary = 100000,
					TotalInvoiced = 2500,
					TotalPaid = 800,
					Home = new Address
					{
						Street = "123 Main Dr",
						City = "Atlanta",
						State = State.Georgia,
						Zip = "30530"
					},
					Work = new Address
					{
						Street = "999 Imaginary Str",
						City = "Roswel",
						State = State.Georgia,
						Zip = "30075"
					},
					Nicknames = new List<string> { "Jonny", "Dude" },
					Visits = new List<Visit> { new Visit { Date = new DateTime(2026, 02, 15), DoctorID = "xyz", ID = 101, Type = VisitType.CheckUp } },
					Hospitals = new List<Hospital> { new Hospital { ID = 202, ManagerID = 29, Name = "Northside Hospital" } }
				};

				// Get the rule XML
				var ruleXml = editor.GetRuleXml();

				// At this point, the evaluator no longer depends on the editor and can reside in an entirely separate
				// project or application. It only requires the XML representation of your rule(s) and the data object
				// against which the rules will be evaluated. In this demo, the editor is used solely as a convenient
				// way to generate and provide the rule XML.

				// Create an instanve of the Evaluator
				// This compiles your rule into IL
				var ev = new Evaluator<Patient>(ruleXml);

				// Evaluate the rule
				bool success = ev.Evaluate(patient);

				// Output the evaluation result (all action methods set the Patient.Output value)
				response.Output = success ?
					"The rule evaluated to TRUE. " + patient.Output :
					"The rule evaluated to FALSE. " + patient.Output;
			}

			return Results.Ok(response);
		});
	}

	private static Control GetControl(Settings settings)
	{
		var editor = new Control();

		// Set the editor to use the Adaptive Source
		editor.MenuProvider = new MenuProvider(typeof(Patient));
		editor.EvaluationMode = settings.Mode;

		return editor;
	}
}

public class Settings
{
	public string? GlobalData { get; set; }
	public string? EditorData { get; set; }

	public EvaluationMode Mode { get; set; } = EvaluationMode.Execution;
}

public class MenuRequest
{
	public RuleSectionType Type { get; set; } = RuleSectionType.Conditions;
	public Context? Context { get; set; }
}

public class Response
{
	public bool IsRuleEmpty { get; set; } = false;
	public bool IsRuleValid { get; set; } = true;

	public string? Output { get; set; }
	public string? ClientInvalidData { get; set; }
}