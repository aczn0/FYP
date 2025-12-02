using Godot;
using System;

public partial class scoreTracking : RichTextLabel
{
	// Called every frame
	// Display current score on GUI
	public override void _Process(double delta){
		SetText("SCORE: " + Global.Instance.score);
	}
}
