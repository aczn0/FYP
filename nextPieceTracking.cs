using Godot;
using System;

public partial class nextPieceTracking : RichTextLabel
{
	// Called every frame
	// Display next piece on GUI
	public override void _Process(double delta){
		SetText("Next Piece: " + Global.Instance.nextPiece);
	}
}
