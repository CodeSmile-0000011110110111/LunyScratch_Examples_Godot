using Luny;
using System;

/// <summary>
/// Example LunyScript demonstrating the block system and Step 2 debug/profiling features.
/// This script will automatically bind to any GameObject named "ExampleLunyScript" in the scene.
/// </summary>
public sealed class ExampleLunyScript : LunyScript.LunyScript
{
	public override void Build()
	{
		// Set up variables
		LocalVars["Health"] = 100;
		LocalVars["Name"] = "Player1";
		GlobalVars["GameScore"] = 0;

		GlobalVars["Boolean_True"] = true;
		GlobalVars["Boolean_False"] = false;
		GlobalVars["String_Value"] = "this is a string";
		GlobalVars["Number_Value"] = 1234567890;

		// Demonstrate Log vs DebugLog
		// DebugLog() is completely stripped in release builds
		//OnUpdate(DebugLog("Debug-only log - stripped in release"));

		// Log() appears in both debug and release builds
		// OnUpdate(Log("ExampleLunyScript Update tick - always logs"));
		// OnFixedStep(Log("ExampleLunyScript FixedStep tick"));
		// OnLateUpdate(Log("ExampleLunyScript LateUpdate tick"));

		// Multi-block sequence demonstrating debug breakpoint
		When.Self.Updates(
			//Log("Multi-block sequence start"),
			Method.Run(() =>
			{
				var health = LocalVars.Get<Int32>("Health");
				LocalVars["Health"] = health - 1;

				var score = LocalVars.Get<Int32>("LocalScore");
				LocalVars["LocalScore"] = ++score;
			})
			//DebugBreak("sequence breakpoint"),
			//EditorPausePlayer("PAUSE PLAYER"),
			//Log("Multi-block sequence end")
		);

		When.Self.Updates(Method.Run(context => LunyLogger.LogInfo(context.ToString())));

		// Demonstrate global variables with variable change tracking
		// In debug builds, Variables.OnVariableChanged events will fire
		When.Self.Steps(Method.Run(() =>
		{
			var score = GlobalVars.Get<Int32>("GameScore");
			GlobalVars["GameScore"] = score + 1;
		}));

		// Note: To enable debug features, build with DEBUG, LUNY_DEBUG, or LUNYSCRIPT_DEBUG defined
		// In release builds, all DebugLog() and DebugBreak() calls have zero overhead

		// To enable execution tracing: context.DebugHooks.EnableTracing = true
		// To enable internal logging: LunyLogger.EnableInternalLogging = true
		// To access profiler: context.BlockProfiler.TakeSnapshot()
	}
}
