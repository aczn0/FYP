using Godot;
using System;
using System.Collections.Generic;

public partial class TetrisSolver : Node2D
{
	// Get the index for all non empty lines
	public List<int> getNonEmptyLines(int[,] currentBoard){
		var nonEmptyRows = new List<int>();
		
		bool nonEmptyFound = false;
		for (int i = 0; i < 22; i++){
			for (int j = 0; j < 12; j++){
				if (currentBoard[i, j] == 1){
					nonEmptyRows.Add(i);
					nonEmptyFound = true;						
				} 
			}
		}
		// If no non empty lines are found add only 0 to the list
		if (!nonEmptyFound){
			nonEmptyRows.Add(0);
			return nonEmptyRows;
		}
		return nonEmptyRows;
	}

	// Get the lengths of non empty lines
	public List<int> getLineLengths(int[,] currentBoard, List<int> nonEmptyRows){
		var lineLengths = new List<int>();

		foreach(int row in nonEmptyRows){
			int numOfFullSpots = 0;
			for(int i = 0; i < 12; i++){
				if (currentBoard[row, i] == 1){
					numOfFullSpots += 1;
				}
			}
			lineLengths.Add(numOfFullSpots);
		}
		return lineLengths;
	}

	// Check if the retrieved row can be reached
	public bool isRowAvailable(int[,] currentBoard, int rowNum){
		for (int i = 0; i < 12; i++){
			// Check that the coordinates before aren't out of bounds
			if (rowNum > 4){
				// If 3 blocks above are empty then assume row is available
				if (currentBoard[rowNum-1, i] == 0 && currentBoard[rowNum-2, i] == 0 && currentBoard[rowNum-3, i] == 0){
					return true;
				}
			}
		}
		return false;
	}

	// Find the next closest empty space in a non-empty row
	public Vector2I findNext(int[,] currentBoard){
		var nonEmptyRows = getNonEmptyLines(currentBoard);

		if (nonEmptyRows[0] == 0){
			// If nothing placed return starting position
			return new Vector2I(5,1);
		} else{
			int xCoordinate = 0;
			for (int i = 0; i < 12; i++){
				if (currentBoard[nonEmptyRows[0], i] == 0 && isRowAvailable(currentBoard, nonEmptyRows[0])){
					xCoordinate = i;
				}
			}

			return new Vector2I(xCoordinate, nonEmptyRows[0]);
		}
		// Search through non empty row and find empty spaces
		// algorithm will always choose the last empty space in the row
	}

	// Find the next best location to place a shape in
	public Vector2I findBest(int[,] currentBoard){
		var nonEmptyRows = getNonEmptyLines(currentBoard);
		List<int> trimRows = new List<int>();

		// Trim nonEmptyRows of all non-available rows
		foreach(int row in nonEmptyRows){
			if (isRowAvailable(currentBoard, row)){
				trimRows.Add(row);
			}
		}

		List<int> rowLengths = getLineLengths(currentBoard, trimRows);

		int currentLargest = 0;
		foreach(int len in rowLengths){
			if (len > currentLargest){
				currentLargest = len;
			}
		}
		return new Vector2I(0,0);
	}

	// Calculate horizontal distance between two points 
	// Return number of steps to get to other point
	public int findPathX(Vector2I point1, Vector2I point2){
		return point1.X - point2.X;
	}
}
