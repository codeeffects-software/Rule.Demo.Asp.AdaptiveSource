
Thank you for downloading Code Effects demo project.

ASP.NET Core | .NET 8.0 | Business Rules

NuGet packages:

- https://www.nuget.org/packages/CodeEffects.Rule.Engine
- https://www.nuget.org/packages/CodeEffects.Rule.Editor

npm package:

- https://www.npmjs.com/package/codeeffects

Right-click the /Dependencies/npm folder and select Restore Packages before building and running the project.
Files from the npm "codeeffects" package are automatically copied by MSBuild to the /wwwroot folder during the build process.
See the .csproj file for details.

Remarks:

The /wwwroot/index.html file declares the rule management and evaluation script that uses end-points declared in the /Api.cs file to save, load, update, delete, and evaluate business rules.
Rules are stored in temporary in-memory storage using the Api.Storage dictionary.
The Toolbar and HelpString are enabled by default. Default settings are configured in the /Api.cs Settings constructor.

This demo project uses the Adaptive Source model. For details about how Adaptive Source works and how menus are generated dynamically at runtime, see the Adaptive Source documentation at https://codeeffects.com/decision-automation/business-rule-adaptive-source

The /MenuProvider.cs class implements the IMenuProvider interface and uses the /Patient.cs class as the data source for generating menu items. The MenuProvider is assigned to the Rule Editor in the GetControl() method of /Api.cs.

Resources:

- Adaptive Source:				https://codeeffects.com/decision-automation/business-rule-adaptive-source
- Live demo:					https://codeeffects.com/business-rules-engine-demo
- Implementation:				https://codeeffects.com/decision-automation/business-rule-implementation
- How to obtain Code Effects:	https://codeeffects.com/decision-automation/how-to-obtain-business-rules-engine
- Code Effects Editions:		https://codeeffects.com/content/business-rules-engine-editions
- Support:						https://codeeffects.com/support

© 2009 - 2026 Code Effects Software, LLC