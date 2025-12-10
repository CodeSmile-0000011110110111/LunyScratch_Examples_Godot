When.Collision.With("Paddle").Begins(Audio.Play("paddle_hit"));



When.Input.Action("Move").Ends(Animation.SetSpeed(0));


When.Event.Received("opened").Then(Prefab.Spawn("goldcoins"));


When.Timer.Seconds(1).Then(Variable("Timer").Decrement());


When.Variable("Timer").LessThan(0).Then(Set(0), Game.Over());




using Godot;
using LunyScript;
using System;

public partial class SoundOnBounce : LunyScript
{
	public override void _Ready()
	{
		
		When.Collision.With("Paddle").Begins(Audio.Play("paddle_hit"));
		
	}
}



When.Collision.With("Paddle").Begins(Audio.Play("paddle_hit"));
