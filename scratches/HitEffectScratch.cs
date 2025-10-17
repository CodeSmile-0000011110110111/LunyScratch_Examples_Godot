using Godot;
using LunyScratch;
using System;
using static LunyScratch.Blocks;

namespace LunyScratch_Examples.scratches
{
	public sealed partial class HitEffectScratch : ScratchRigidbody3D
	{
		[Export] private Double _timeToLiveInSeconds = 3;
		[Export] private Double _minVelocityForSound = 3;

		protected override void OnScratchReady()
		{
			Run(Wait(_timeToLiveInSeconds), DestroySelf());

			var globalTimeout = GlobalVariables["MiniCubeSoundTimeout"];
			When(CollisionEnter(),
				If(AND(IsVariableLessThan(globalTimeout, 0), IsVelocityGreater(_minVelocityForSound)),
					PlaySound(), SetVariable(globalTimeout, 0)));
		}
	}
}
