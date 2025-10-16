using Godot;
using LunyScratch;
using static LunyScratch.Blocks;

public partial class PoliceCarScratch : ScratchNode3D
{
	public override void _Ready()
	{
		GD.Print("test print");

		// blinking signal lights
		RepeatForever(
			Enable("RedLight"),
			Wait(0.16),
			Disable("RedLight"),
			Wait(0.12)
		);
		RepeatForever(
			Disable("BlueLight"),
			Wait(0.13),
			Enable("BlueLight"),
			Wait(0.17)
		);
	}
}
