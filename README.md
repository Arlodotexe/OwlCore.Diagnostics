# OwlCore.Diagnostics [![Version](https://img.shields.io/nuget/v/OwlCore.Diagnostics.svg)](https://www.nuget.org/packages/OwlCore.Diagnostics)

Supplemental diagnostic tooling for dotnet.

## Featuring:
- A simple static Logger for consolidating outputting one or more log streams. 
- ProcessHelpers for interacting with executable binaries.

## Install

Published releases are available on [NuGet](https://www.nuget.org/packages/OwlCore.Diagnostics). To install, run the following command in the [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console).

    PM> Install-Package OwlCore.Diagnostics
    
Or using [dotnet](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet)

    > dotnet add package OwlCore.Diagnostics

## Usage

### `Logger` 

```cs
var startTime = DateTime.Now;

// Logging output routing //
////////////////////////////
Logger.MessageReceived += Logger_MessageReceived;

void Logger_MessageReceived(object? sender, LoggerMessageEventArgs e) => Console.WriteLine(FormatLoggerMessage(e));
void Logger_MessageReceived(object? sender, LoggerMessageEventArgs e) => Debug.WriteLine(FormatLoggerMessage(e));

string FormatLoggerMessage(LoggerMessageEventArgs e) => $"+{Math.Round((DateTime.Now - startTime).TotalMilliseconds)}ms {Path.GetFileNameWithoutExtension(e.CallerFilePath)} {e.CallerMemberName}  [{e.Level}] {e.Exception} {e.Message}";

// Additional logging input routing //
//////////////////////////////////////
// Normal unhandled application exceptions
AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => Logger.LogError(e.ExceptionObject?.ToString() ?? "Error message not found", e.ExceptionObject as Exception);

// First chance exception log all exceptions, even if caught.
AppDomain.CurrentDomain.FirstChanceException += (object? sender, FirstChanceExceptionEventArgs e) => Logger.LogError(e.Exception?.ToString() ?? "Error message not found", e.Exception);

// Unobserved tasks (_ = SomethingAsync()) can suppress exceptions, but can be observed and routed nonetheless.
TaskScheduler.UnobservedTaskException += (object? sender, UnobservedTaskExceptionEventArgs e) => Logger.LogError(e.Exception?.ToString() ?? "Error message not found", e.Exception);

// Logging invocation //
////////////////////////
// Logs the general flow of the application. Can be used for interactive investigation during development.
Logger.LogInformation(information);

// Logs when the current flow of execution is stopped due to a failure. These should indicate a failure in the current activity, not an application-wide failure.
Logger.LogError(message, exception);

// Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the application execution to stop.
Logger.LogWarning(message);

// Logs an extremely detailed message. May contain sensitive application data. These messages should never be enabled in a production environment.
Logger.LogTrace(message);

// Logs an unrecoverable application or system crash, or a catastrophic failure that requires immediate attention.
Logger.LogCritical(message);
```

### `ProcessHelpers`

```cs
// Collect both output and error
var (output, error) = await ProcessHelpers.RunExecutable(new SystemFile("/path/to/binary"), arguments: $"-id \"{id}\" -port {portNumber}");

// Throwing instead of returning error
var (output, _) = await ProcessHelpers.RunExecutable(new SystemFile("/path/to/binary"), arguments: $"-id \"{id}\" -port {portNumber}", throwOnError: true);
```

## Financing

We accept donations [here](https://github.com/sponsors/Arlodotexe) and [here](https://www.patreon.com/arlodotexe), and we do not have any active bug bounties.

## Versioning

Version numbering follows the Semantic versioning approach. However, if the major version is `0`, the code is considered alpha and breaking changes may occur as a minor update.

## License

All OwlCore code is licensed under the MIT License. OwlCore is licensed under the MIT License. See the [LICENSE](./src/LICENSE.txt) file for more details.
