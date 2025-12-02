using Godot;
using System;
using System.Collections.Generic;

public partial class BoardMatrix : Node2D
{

	// 0 = empty space
	// 1 = filled space
	// 2 = border
	private int[,] gameBoard;

	// exclusively for sending drawing data to the activeShape class
	private List<int> currentRowsToClear;
	private List<int> movedDownRowsToClear;

	public override void _Ready(){
		initialiseBoard();
	}

	// creates the borders of the board
	public void initialiseBoard(){
		int[,] board = { 
						{2,2,2,2,2,2,2,2,2,2,2,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,0,0,0,0,0,0,0,0,0,0,2},
						{2,2,2,2,2,2,2,2,2,2,2,2} };
		setBoard(board);
	}

	// Print current board state
	public void printBoard(){
		int[,] temp = getBoard();
		for (int i = 0; i < 22; i++){
			string boardRow = "";
			for (int j = 0; j < 12; j++){
				boardRow = boardRow + temp[i,j].ToString();
			}
			GD.Print(boardRow);
		}
	}

	// Fill one position in the matrix from a given coordinate
	public void fillEmptySpot(Vector2I coordinate){
		int[,] temp = getBoard();
		temp[coordinate.Y, coordinate.X] = 1;
		setBoard(temp);
	}

	// Find all full rows and clear 
	// Return score 
	public int clearRows(){
		int[,] temp = getBoard();
		var rowsToClear = new List<int>();
		int score = 0;
		bool fullRowFound = false;
		// Iterate through board and count how many filled spots in a row
		for (int i = 0; i < 22; i++){
			int fullCount = 0;
			for (int j = 0; j < 12; j++){
				if (temp[i, j] == 1){
					fullCount += 1;
				}
			}
			// If number of filled spots is equal to row size then add to list
			if (fullCount == 10){
				rowsToClear.Add(i);
				fullRowFound = true;
			}
		}

		// Set all filled rows back to empty
		setRowsToClear(rowsToClear);
		foreach (int row in rowsToClear){
			for (int k = 1; k < 11; k++){
				temp[row, k] = 0;
			}
			score += 1;
		}
		if (fullRowFound){
			temp = moveRowsDown(temp);
		}
		GD.Print(temp);
		// Set board to new state
		setBoard(temp);
	
		return score;
	}

	// method to shift rows down following a  cleared line
	public int[,] moveRowsDown(int[,] board){
		for (int i = 20; i >= 0; i--){
			for (int j = 0; j < 12; j++){
				if (board[i, j] == 1 && board[i+1, j] == 0){
					GD.Print("Moving down");
					board[i, j] = 0;
					board[i+1, j] = 1;
				}
			}
		}
		return board;
	}

	// turn board into an int array
	public int[] flatten(){
		int size = getBoard().GetLength(0) * getBoard().GetLength(1);
		int[] res = new int[size];

		int write = 0;
		for (int i = 0; i <= getBoard().GetUpperBound(0); i++){
			for (int j = 0; j <= getBoard().GetUpperBound(1); j++){
				res[write] = getBoard()[i, j];
				write++;
			}
		}

		return res;
	}

	// method to stringify flattened board
	public override string ToString()
	{
		int[] flattenedBoard = flatten();
		string res = String.Join(",", flattenedBoard);
		return res;
	}

	// Setters and Getters
	public int[,] getBoard(){
		return gameBoard;
	}

	// private because only to be used within this class
	private void setBoard(int[,] newBoard){
		gameBoard = newBoard;
	}

	// private because only to be used within this class
	private void setRowsToClear(List<int> newList){
		currentRowsToClear = newList;
	}

	public List<int> getRowsToClear(){
		return currentRowsToClear;
	}

	public void setMovedDownRowsToClear(List<int> newList){
		movedDownRowsToClear = newList;
	}

	public List<int> getMovedDownRowsToClear(){
		return movedDownRowsToClear;
	}
}
