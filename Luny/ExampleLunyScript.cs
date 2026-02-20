using LunyScript;

/// <summary>
/// Example LunyScript demonstrating the block system and Step 2 debug/profiling features.
/// This script will automatically bind to any GameObject named "ExampleLunyScript" in the scene.
/// </summary>
public sealed class ExampleLunyScript : Script
{
	public override void Build(ScriptContext context)
	{
		// Set up variables
		Var["Health"].Set(100);
		Var["Name"].Set("Player1");
		GVar["GameScore"].Set(0);

		GVar["Boolean_True"].Set(true);
		GVar["Boolean_False"].Set(false);
		GVar["String_Value"].Set("this is a string");
		GVar["Number_Value"].Set(1234567890);

		// Multi-block sequence demonstrating debug breakpoint
		On.FrameUpdate(Var["Health"].Dec(), Var["LocalScore"].Inc());

		// Demonstrate global variables with variable change tracking
		// In debug builds, Variables.OnVariableChanged events will fire
		On.Heartbeat(GVar["GameScore"].Inc());

		// Note: To enable debug features, build with DEBUG, LUNY_DEBUG, or LUNYSCRIPT_DEBUG defined
		// In release builds, all DebugLog() and DebugBreak() calls have zero overhead

		// To enable execution tracing: context.DebugHooks.EnableTracing = true
		// To enable internal logging: LunyLogger.EnableInternalLogging = true
		// To access profiler: context.BlockProfiler.TakeSnapshot()
	}
}
