using Luny;

/// <summary>
/// Example LunyScript that demonstrates the basic structure.
/// This script will automatically bind to any GameObject named "ExampleLunyScript" in the scene.
/// </summary>
public class ExampleLunyScript : LunyScript.LunyScript
{
    public override void Build()
    {
        // TODO: In future phases, users will construct sequences here:
        // When.KeyPressed("Space").Do(() => Audio.Play("Jump"));
        // When.Collision().With("Enemy").Do(() => Variables["Health"] = 0);
        // For now, this is just a placeholder demonstrating the clean API
        LunyLogger.LogInfo($"====> {nameof(ExampleLunyScript)} runs OnStartup !!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    }
}
