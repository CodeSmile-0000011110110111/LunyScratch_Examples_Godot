using Godot;
using LunyScratch;
using System;
using static LunyScratch.Blocks;

namespace LunyScratch_Examples.scratches
{
	public sealed partial class CompanionCubeScratch : ScratchRigidbody3D
	{
		[Export] private Single _minVelocityForSound = 5f;

		protected override void OnScratchReady()
		{
			var progressVar = GlobalVariables["Progress"];
			var counterVar = Variables["Counter"];

			// increment counter to be able to hit the ball again
			RepeatForever(AddVariable(counterVar, 5), Wait(1));

			Run(Disable("Lights"));

			When(CollisionEnter(tag: "Police"),
				// play bump sound unconditionally and make cube glow
				PlaySound(), Enable("Lights"),
				// count down from current progress value to spawn more cube instances the longer the game progresses
				RepeatWhileTrue(() =>
					{
						if (counterVar.Number > progressVar.Number)
							counterVar.Set(Math.Clamp(progressVar.Number, 1, 50));
						counterVar.Subtract(1);
						return counterVar.Number >= 0;
					}, CreateInstance("scenes/hiteffect"), Wait(1 / 60f),
					CreateInstance("scenes/hiteffect"), Wait(1 / 60f),
					CreateInstance("scenes/hiteffect")),
				Wait(1), Disable("Lights"));

			// play sound when ball bumps into anything
			When(CollisionEnter(),
				If(IsVelocityGreater(_minVelocityForSound),
					PlaySound()));
		}
	}
}
