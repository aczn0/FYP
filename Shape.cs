using Godot;
using System;

// class contains a lot of redundant code that could be better, not sure how to do it in current Godot and C# version as collections dont work - will revisit later 
// Class contains all shape coordinates and their rotation coordinates
public partial class Shape : Node
{

	public Shape(){}

	public Vector2I[] getI0Coords(){
		Vector2I[] coords = {new Vector2I(0,0), new Vector2I(1,0), new Vector2I(2,0), new Vector2I(3,0)};
		return coords;
	}

	public Vector2I[] getI90Coords(){
		Vector2I[] coords = {new Vector2I(1,0), new Vector2I(1,1), new Vector2I(1,2), new Vector2I(1,3)};
		return coords;
	}

	public Vector2I[] getI180Coords(){
		Vector2I[] coords = {new Vector2I(0,1), new Vector2I(1,1), new Vector2I(2,1), new Vector2I(3,1)};
		return coords;
	}

	public Vector2I[] getI270Coords(){
		Vector2I[] coords = {new Vector2I(1,0), new Vector2I(1,1), new Vector2I(1,2), new Vector2I(1,3)};
		return coords;
	}

	public Vector2I[] getL0Coords(){
		Vector2I[] coords = {new Vector2I(2,0), new Vector2I(0,1), new Vector2I(1,1), new Vector2I(2,1)};
		return coords;
	}

	public Vector2I[] getL90Coords(){
		Vector2I[] coords = {new Vector2I(1,0), new Vector2I(1,1), new Vector2I(1,2), new Vector2I(2,2)};
		return coords;
	}

	public Vector2I[] getL180Coords(){
		Vector2I[] coords = {new Vector2I(0,1), new Vector2I(1,1), new Vector2I(2,1), new Vector2I(0,2)};
		return coords;
	}

	public Vector2I[] getL270Coords(){
		Vector2I[] coords = {new Vector2I(0,0), new Vector2I(1,0), new Vector2I(1,1), new Vector2I(1,2)};
		return coords;
	}

	public Vector2I[] getJ0Coords(){
		Vector2I[] coords = {new Vector2I(0,0), new Vector2I(0,1), new Vector2I(1,1), new Vector2I(2,1)};
		return coords;
	}

	public Vector2I[] getJ90Coords(){
		Vector2I[] coords = {new Vector2I(1,0), new Vector2I(2,0), new Vector2I(1,1), new Vector2I(1,2)};
		return coords;
	}

	public Vector2I[] getJ180Coords(){
		Vector2I[] coords = {new Vector2I(0,1), new Vector2I(1,1), new Vector2I(2,1), new Vector2I(2,2)};
		return coords;
	}

	public Vector2I[] getJ270Coords(){
		Vector2I[] coords = {new Vector2I(1,0), new Vector2I(1,1), new Vector2I(0,2), new Vector2I(1,2)};
		return coords;
	}

	public Vector2I[] getSquareCoords(){
		Vector2I[] coords = {new Vector2I(0,0), new Vector2I(1,0), new Vector2I(0,1), new Vector2I(1,1)};
		return coords;
	}

	public Vector2I[] getS0Coords(){
		Vector2I[] coords = {new Vector2I(1,0), new Vector2I(2,0), new Vector2I(0,1), new Vector2I(1,1)};
		return coords;
	}

	public Vector2I[] getS90Coords(){
		Vector2I[] coords = {new Vector2I(1,0), new Vector2I(1,1), new Vector2I(2,1), new Vector2I(2,2)};
		return coords;
	}

	public Vector2I[] getS180Coords(){
		Vector2I[] coords = {new Vector2I(1,1), new Vector2I(2,1), new Vector2I(0,2), new Vector2I(1,2)};
		return coords;
	}

	public Vector2I[] getS270Coords(){
		Vector2I[] coords = {new Vector2I(0,0), new Vector2I(0,1), new Vector2I(1,1), new Vector2I(1,2)};
		return coords;
	}

	public Vector2I[] getT0Coords(){
		Vector2I[] coords = {new Vector2I(1,0), new Vector2I(0,1), new Vector2I(1,1), new Vector2I(2,1)};
		return coords;
	}

	public Vector2I[] getT90Coords(){
		Vector2I[] coords = {new Vector2I(1,0), new Vector2I(1,1), new Vector2I(2,1), new Vector2I(1,2)};
		return coords;
	}

	public Vector2I[] getT180Coords(){
		Vector2I[] coords = {new Vector2I(0,1), new Vector2I(1,1), new Vector2I(2,1), new Vector2I(1,2)};
		return coords;
	}

	public Vector2I[] getT270Coords(){
		Vector2I[] coords = {new Vector2I(1,0), new Vector2I(0,1), new Vector2I(1,1), new Vector2I(1,2)};
		return coords;
	}

	public Vector2I[] getZ0Coords(){
		Vector2I[] coords = {new Vector2I(0,0), new Vector2I(1,0), new Vector2I(1,1), new Vector2I(2,1)};
		return coords;
	}

	public Vector2I[] getZ90Coords(){
		Vector2I[] coords = {new Vector2I(2,0), new Vector2I(1,1), new Vector2I(2,1), new Vector2I(1,2)};
		return coords;
	}

	public Vector2I[] getZ180Coords(){
		Vector2I[] coords = {new Vector2I(0,1), new Vector2I(1,1), new Vector2I(1,2), new Vector2I(2,2)};
		return coords;
	}

	public Vector2I[] getZ270Coords(){
		Vector2I[] coords = {new Vector2I(1,0), new Vector2I(0,1), new Vector2I(1,1), new Vector2I(0,2)};
		return coords;
	}
}
