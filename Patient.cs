using CodeEffects.Rule.Common.Attributes;
using CodeEffects.Rule.Common.Models;

namespace CodeEffects.Demo.Asp.AdaptiveSource
{
	[Data("Education", typeof(Utils), "Schools")]
	[ExternalMethod(typeof(Utils), "ExternalMethod")]
	[ExternalAction(typeof(Utils), "ExternalAction")]
	public class Patient
	{
		[Field(DisplayName = "Patient ID", Min = 0, Max = 10000000, AllowCalculations = false, IncludeInCalculations = false)]
		public int ID { get; set; } = 0;

		[Field(DisplayName = "First Name", Max = 200)]
		public string? FirstName { get; set; }

		[Field(DisplayName = "Last Name", Max = 200, Prompt = true)]
		public string? LastName { get; set; }

		[Field(DisplayName = "Date of Birth", DateTimeFormat = "MMM dd, yyyy")]
		public DateTime DOB { get; set; } = DateTime.MinValue;

		public Gender Gender { get; set; } = Gender.Undefined;

		[Field(DisplayName = "Education", DataSourceName = "Education", Description = "Patient's Education", ValueInputType = ValueInputType.User)]
		public string? EducationID { get; set; }

		[Field(DisplayName = "Is Active")]
		public bool IsActive { get; set; } = false;

		[Field(Min = 0, Max = 500000, IncludeInCalculations = true, AllowCalculations = true)]
		public decimal Salary { get; set; } = 0;

		[Field(Min = 0, Max = 1000000, IncludeInCalculations = true, AllowCalculations = true)]
		public decimal TotalInvoiced { get; set; } = 0;

		[Field(Min = 0, Max = 1000000, IncludeInCalculations = true, AllowCalculations = true)]
		public decimal TotalPaid { get; set; } = 0;

		public Address Home { get; set; } = new Address();
		public Address Work { get; set; } = new Address();

		public List<string> Nicknames { get; set; } = new List<string>();

		public ICollection<Visit> Visits { get; set; } = new List<Visit>();
		public ICollection<Hospital> Hospitals { get; set; } = new List<Hospital>();

		[ExcludeFromEvaluation]
		public string? Output { get; set; }

		[Method(DisplayName = "Full Name")]
		public string FullName()
		{
			return LastName + ", " + FirstName;
		}

		[Method(DisplayName = "Get AI Probability", IncludeInCalculations = true)]
		[return: Return(Min = 0, Max = 1, AllowCalculations = true)]
		public decimal GetAiProbability([Parameter(Prompt = true)] string prompt)
		{
			// Get your AI implementation that returns some probability percentage here
			return 0.75m;
		}

		[Method(DisplayName = "Get Total Debt", IncludeInCalculations = true)]
		[return: Return(Min = -10000000, Max = 10000000, AllowCalculations = true)]
		public static decimal CalculateTotalDebt(decimal invoiced, decimal paid)
		{
			return invoiced - paid;
		}

		public void Accept(string message)
		{
			this.IsActive = true;
			this.Output = message;
		}

		public void Decline(string message)
		{
			this.IsActive = false;
			this.Output = message;
		}
	}

	public enum Gender
	{
		Undefined = 0, Male = 1, Female = 2
	}

	public enum State
	{
		Undefined = 0,
		Georgia = 1,

		[EnumItem("North Carolina")]
		NorthCarolina = 2
	}

	public enum VisitType
	{
		Undefined = 0,

		[EnumItem("Check Up")]
		CheckUp = 1,

		Emergency = 4
	}

	public class Address
	{
		[Parent("Home", "Home Street")]
		[Parent("Work", "Work Street")]
		[Field(Prompt = true)]
		public string? Street { get; set; }

		[Parent("Home", "Home City")]
		[Parent("Work", "Work City")]
		public string? City { get; set; }

		[Parent("Home", "Home Zip")]
		[Parent("Work", "Work Zip")]
		public string? Zip { get; set; }

		[Parent("Home", "Home State")]
		[Parent("Work", "Work State")]
		public State State { get; set; } = State.Undefined;
	}

	[Data("Doctors", typeof(Utils), "Doctors")]
	public class Visit
	{
		[Field(DisplayName = "ID", Min = 0, Max = 100000, AllowCalculations = false, IncludeInCalculations = false)]
		public int ID { get; set; } = 0;

		[Field(DisplayName = "Doctor", DataSourceName = "Doctors", ValueInputType = ValueInputType.User)]
		public string? DoctorID { get; set; }

		[Field(DisplayName = "Date", DateTimeFormat = "MMM dd, yyyy")]
		public DateTime Date { get; set; } = DateTime.MinValue;

		public VisitType Type { get; set; } = VisitType.Undefined;

		[Method(DisplayName = "Test Method")]
		public VisitType TestMethod([Parameter(DataSourceName = "Doctors")] string doctorID, VisitType type)
		{
			if(type == VisitType.Undefined && doctorID == "abc") return VisitType.Emergency;
			else return VisitType.CheckUp;
		}
	}

	[Data("Managers", typeof(Utils), "Managers")]
	public class Hospital
	{
		[Field(Min = 0, Max = 100000, AllowCalculations = false, IncludeInCalculations = false)]
		public int ID { get; set; } = 0;

		[Field(Max = 100)]
		public string? Name { get; set; }

		[Field(DisplayName = "Manager", DataSourceName = "Managers", ValueInputType = ValueInputType.User)]
		public int ManagerID { get; set; }
	}
}