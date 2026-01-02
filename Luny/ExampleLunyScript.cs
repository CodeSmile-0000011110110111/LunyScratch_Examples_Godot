/// <summary>
/// Example LunyScript demonstrating the block system and Step 2 debug/profiling features.
/// This script will automatically bind to any GameObject named "ExampleLunyScript" in the scene.
/// </summary>
public sealed class ExampleLunyScript : LunyScript.LunyScript
{
	public override void Build()
	{
		// Set up variables
		LocalVariables["Health"] = 100;
		LocalVariables["Name"] = "Player1";
		GlobalVariables["GameScore"] = 0;

		GlobalVariables["Boolean_True"] = true;
		GlobalVariables["Boolean_False"] = false;
		GlobalVariables["String_Value"] = "this is a string";
		GlobalVariables["Number_Value"] = 1234567890;

		// Demonstrate Log vs DebugLog
		// DebugLog() is completely stripped in release builds
		//OnUpdate(DebugLog("Debug-only log - stripped in release"));

		// Log() appears in both debug and release builds
		// OnUpdate(Log("ExampleLunyScript Update tick - always logs"));
		// OnFixedStep(Log("ExampleLunyScript FixedStep tick"));
		// OnLateUpdate(Log("ExampleLunyScript LateUpdate tick"));

		// Multi-block sequence demonstrating debug breakpoint
		When.EveryFrame(
			//Log("Multi-block sequence start"),
			Run(() =>
			{
				var health = LocalVariables.Get<int>("Health");
				LocalVariables["Health"] = health - 1;

				var score = LocalVariables.Get<int>("LocalScore");
				LocalVariables["LocalScore"] = ++score;
			})
			//DebugBreak("sequence breakpoint"),
			//EditorPausePlayer("PAUSE PLAYER"),
			//Log("Multi-block sequence end")
		);

		// Demonstrate global variables with variable change tracking
		// In debug builds, Variables.OnVariableChanged events will fire
		When.EveryFixedStep(Run(() =>
		{
			var score = GlobalVariables.Get<int>("GameScore");
			GlobalVariables["GameScore"] = score + 1;
		}));

		// Note: To enable debug features, build with DEBUG, LUNY_DEBUG, or LUNYSCRIPT_DEBUG defined
		// In release builds, all DebugLog() and DebugBreak() calls have zero overhead

		// To enable execution tracing: context.DebugHooks.EnableTracing = true
		// To enable internal logging: LunyLogger.EnableInternalLogging = true
		// To access profiler: context.BlockProfiler.TakeSnapshot()
	}
}
