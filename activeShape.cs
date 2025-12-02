using Godot;
using System;
using System.Collections.Generic;

public partial class activeShape : TileMapLayer
{

	// Board attributes
	int COLS = 10;
	int ROWS = 20;
	int score;
	private BoardMatrix board;
	bool gameOver = false;


	// Drawing and shape attributes	
	private Shape shapes;
	private Vector2I currentPos;
	private Vector2I[] activePiece;
	private Vector2I[] previousPieceDrawPos;
	private EraseQueue eraseQueue;
	private Vector2I prevDrawPos;
	private int currentRotation;
	public Vector2I[] currentShapeCoords;
	public Vector2I[] nextShapeCoords;

	public Vector2I currentShapeAtlasPos;
	public int nextShapeID;
	public int currentShapeID;

	// Movement attributes
	private int speed;
	private int gravity; // the speed at which a piece falls, directly proportional with game time
	private int steps; // only for use when moving active piece downwards
	private bool inPlay;
	
	// AI related attributes
	private TetrisSolver solver;
	public int stepsToDestination;
	private ReceiveFromPython stream;

	private QLearning QLAgent;
	private Sarsa SARSA;
	public int prevAction = 0;
	public int currentEpoch = 0;
	public int maxEpochs = 20;
	public string prevState = "";

	// Analytics related attributes
	public List<int> rewards = new List<int>();
	public List<int> epochs = new List<int>();
	private QResultsToFile QResults;
	private SarsaResultsToFile SARSAResults;

	// Called when the node enters the scene tree for the first time.
	// Initiliase all values needed at startup
	public override void _Ready()
	{
		// Reset global score
		Global.Instance.score = 0;

		// Get and ready all required nodes
		shapes = GetNode<Shape>("GamePieces");
		shapes._Ready();

		board = GetNode<BoardMatrix>("BoardMatrix");
		board._Ready();
		board.printBoard();
		

		eraseQueue = GetNode<EraseQueue>("EraseQueue");
		eraseQueue._Ready();

		solver = GetNode<TetrisSolver>("TetrisSolver");
		solver._Ready();

		QLAgent = GetNode<QLearning>("QLearning");
		QLAgent._Ready();

		SARSA = GetNode<Sarsa>("SARSA");
		SARSA._Ready();
		prevState = board.ToString();

		QResults = GetNode<QResultsToFile>("QResultsToFile");
		SARSAResults = GetNode<SarsaResultsToFile>("SARSAResultsToFile");

		// Initliase Solver variables

		stepsToDestination = 0;

		// Placeholder values to prevent eraseShape from removing shapes premptively
		setPrevDrawPiece(shapes.getI0Coords());
		setPrevDrawPos(new Vector2I(0,0));
		setCurrentPos(new Vector2I(5,1));

		// Decide which shape to play first
		currentShapeID = decideNextShape();
		currentShapeCoords = getShapeCoords(currentShapeID);

		// Set beginning fall speed
		setSpeed(1);
		setGravity(100);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// Main game loop 
	public override void _Process(double delta)
	{
		// inPlay = true;
		bool isAiPlaying = true;
		if (isAiPlaying){
			train("Q");
		}
		else{		
			// Check if piece can be placed then generate new shape to display
			if (isPlaceablePiece(currentShapeCoords)){
				placeShape(currentShapeCoords);
				processFullRows();
				decideNextPieces();
				setPrevDrawPos(new Vector2I(0,0));
				setCurrentPos(new Vector2I(5,1));
				inPlay = false;
			}
			drawFromMatrix();
			eraseShape(getPrevDrawPiece());

			Vector2I movementVector = new Vector2I(0,0);
			setSteps(getSteps() + 1);

			if (getSteps() > getGravity()){
				movementVector = new Vector2I(0,1);
				setSteps(0);
			}
			if (Input.IsActionJustPressed("ui_left")){
				movementVector = new Vector2I(-1,0);
			}
			if (Input.IsActionJustPressed("ui_right")){
				movementVector = new Vector2I(1,0);
			}
			if (Input.IsActionJustPressed("ui_up")){
				rotateShape();
				GD.Print("Current rotation" + currentRotation);
				GD.Print("Current shape ID" + currentShapeID);
				currentShapeCoords = getShapeCoords(currentShapeID);
			}
			if (Input.IsActionJustPressed("ui_select")){
				setCurrentPos(new Vector2I(getCurrentPos().X, ROWS-1));
				movementVector = new Vector2I(getCurrentPos().X, ROWS-1);
			}

			moveGamePiece(movementVector, currentShapeCoords);
		}
	}
	
	public bool groundTimeUp(){ return true;}

	// Version of the game controlled entirely by an algorithm, contains mostly the same code as _Process 
	public void tetrisSolver(){
		// Check if piece can be placed then generate new shape to display
		if (isPlaceablePiece(currentShapeCoords)){
			placeShape(currentShapeCoords);
			processFullRows();
			decideNextPieces();
			setPrevDrawPos(new Vector2I(0,0));
			setCurrentPos(new Vector2I(5,1));
			inPlay = false;
		} 
		drawFromMatrix();
		eraseShape(getPrevDrawPiece());

		Vector2I movementVector = new Vector2I(0,0);
		// Get next coordinate to move to
		Vector2I nextPos = solver.findNext(board.getBoard());
		// Calculate horizontal distance between current position and next
		stepsToDestination = solver.findPathX(getCurrentPos(), nextPos);

		setSteps(getSteps() + 1);
		bool moveTaken = false;
		if (getSteps() > getGravity()){
			movementVector = new Vector2I(0,1);
			setSteps(0);
		}
	
		if (stepsToDestination != 0){
			if (nextPos.X < getCurrentPos().X){
				movementVector = new Vector2I(-1, 1);
			} else if (nextPos.X > getCurrentPos().X) {
				movementVector = new Vector2I(1, 1);
			}
		}

		moveGamePiece(movementVector, currentShapeCoords);
	}

	// Q Learning interface, mostly same code as _Process adapted to work with Q Learning
	public void QLearningInterface(){
		clearVisualBoard();
		Vector2I prevpos = getCurrentPos();
		if (checkGameOver(currentShapeCoords)){
			GD.Print("\n");
			GD.Print("Game Over");
			GD.Print("Pevious pos before game over" + prevpos);
			GD.Print("Previous shape coords before game over" + currentShapeCoords);
			GD.Print("\n");
			gameOver = true;
		} 
		int curReward = 0;

		if (isPlaceablePiece(currentShapeCoords)){
			placeShape(currentShapeCoords);
			curReward = processFullRows();
			decideNextPieces();
			setPrevDrawPos(new Vector2I(0,0));
			setCurrentPos(new Vector2I(5,1));
			inPlay = false;
		} 
		drawFromMatrix();
		eraseShape(getPrevDrawPiece());

		Vector2I movementVector = new Vector2I(0,0);

		if (gameOver){
			curReward = -100;
		}
		int chosenAction = QLAgent.chooseAction(board.ToString());
		
		QLAgent.update(prevAction, board.ToString(), curReward);

		setSteps(getSteps() + 1);
		bool moveTaken = false;
		if (getSteps() > getGravity()){
			movementVector = new Vector2I(0,1);
			setSteps(0);
		}
		if (getSteps() >= 80){
			if (chosenAction == 0) {
				movementVector += new Vector2I(-1, 0);
			} else if (chosenAction == 1){
				movementVector += new Vector2I(1, 0);
			} else if (chosenAction == 2){
				rotateShape();
				currentShapeCoords = getShapeCoords(currentShapeID);
			} else if (chosenAction == 3){
				// hard drop here
			}			
		}
		prevAction = chosenAction;
		rewards.Add(curReward);
		epochs.Add(currentEpoch);
		moveGamePiece(movementVector, currentShapeCoords);
	}
	
	// SARSA interface, mostly same code as _Process adapted to work with SARSA
	public void SARSAInterface(){
		clearVisualBoard();
		Vector2I prevpos = getCurrentPos();
		if (checkGameOver(currentShapeCoords)){
			GD.Print("\n");
			GD.Print("Game Over");
			GD.Print("Pevious pos before game over" + prevpos);
			GD.Print("Previous shape coords before game over" + currentShapeCoords);
			GD.Print("\n");
			gameOver = true;
		} 
		int curReward = 0;
		if (isPlaceablePiece(currentShapeCoords)){
			placeShape(currentShapeCoords);
			curReward = processFullRows();
			decideNextPieces();
			setPrevDrawPos(new Vector2I(0,0));
			setCurrentPos(new Vector2I(5,1));
			inPlay = false;
		} 
		drawFromMatrix();
		eraseShape(getPrevDrawPiece());

		Vector2I movementVector = new Vector2I(0,0);

		if (gameOver){
			curReward = -100;
		}
		int chosenAction = SARSA.chooseAction(board.ToString());
		
		SARSA.update(prevAction, prevState, curReward, board.ToString(), chosenAction);

		setSteps(getSteps() + 1);
		bool moveTaken = false;
		if (getSteps() > getGravity()){
			movementVector = new Vector2I(0,1);
			setSteps(0);
		}
		if (getSteps() >= 120){
			if (chosenAction == 0) {
				movementVector += new Vector2I(-1, 0);
			} else if (chosenAction == 1){
				movementVector += new Vector2I(1, 0);
			} else if (chosenAction == 2){
				rotateShape();
				currentShapeCoords = getShapeCoords(currentShapeID);
			} else if (chosenAction == 3){
				// hard drop here
			}			
		}
		prevAction = chosenAction;
		prevState = board.ToString();
		rewards.Add(curReward);
		epochs.Add(currentEpoch);
		moveGamePiece(movementVector, currentShapeCoords);
	}
	
	// training loop, runs training episode then saves Q-Table to json
	public void train(string model){
		if (model == "Q"){
			if (gameOver){
				GD.Print("epoch finished");
				QLAgent.saveQ();
				board.initialiseBoard();
				gameOver = false;
				currentEpoch++;

				if (currentEpoch >= maxEpochs){
					GD.Print("training finished");
					QResults.resultsToFile(epochs, rewards);
					GetTree().Quit();
				}
			} else {
				QLearningInterface();
			}
		} else if (model == "SARSA"){
			if (gameOver){
				GD.Print("epoch finished");
				SARSA.saveQ();
				board.initialiseBoard();
				gameOver = false;
				currentEpoch++;

				if (currentEpoch >= maxEpochs){
					GD.Print("training finished");
					SARSAResults.resultsToFile(epochs, rewards);
					GetTree().Quit();
				}
			} else {
				SARSAInterface();
			}
		}
	}

	// Draw all filled rows from Matrix to Display game board
	public void drawFromMatrix(){
		int[,] temp = board.getBoard();
		for (int i = 0; i < 22; i++){
			for (int j = 0; j < 12; j++){
				if(temp[i, j] == 1){
					SetCell(new Vector2I(j, i), 0, currentShapeAtlasPos);
				}
			}
		}
	}
	
	// Clears all items from board
	public void clearVisualBoard(){
		int[,] temp = board.getBoard();
		for (int i = 0; i < 22; i++){
			for (int j = 0; j < 12; j++){
				if(temp[i, j] == 0){
					EraseCell(new Vector2I(j, i));
				}
			}
		}
	}

	// Generates the next two pieces, the second one is to be displayed on the GUI
	public void decideNextPieces(){
		resetRotation();
		currentShapeID = decideNextShape();
		currentShapeCoords = getShapeCoords(currentShapeID);
		nextShapeID = decideNextShape();
		nextShapeCoords = getShapeCoords(nextShapeID);
		switch(nextShapeID){
			case 0:
				Global.Instance.nextPiece = "I";
				currentShapeAtlasPos = new Vector2I(0,0);
				break;
			case 1:
				Global.Instance.nextPiece = "L";
				currentShapeAtlasPos = new Vector2I(0,0);
				break;
			case 2:
				Global.Instance.nextPiece = "J";
				currentShapeAtlasPos = new Vector2I(0,0);
				break;
			case 3:
				Global.Instance.nextPiece = "C";
				currentShapeAtlasPos = new Vector2I(0,0);
				break;
			case 4:
				Global.Instance.nextPiece = "S";
				currentShapeAtlasPos = new Vector2I(0,0);
				break;
			case 5:
				Global.Instance.nextPiece = "T";
				currentShapeAtlasPos = new Vector2I(0,0);
				break;
			case 6:
				Global.Instance.nextPiece = "Z";
				currentShapeAtlasPos = new Vector2I(0,0);
				break;
		}
	}

	// Piece movement handler - check whether move is valid then draw shape
	public void moveGamePiece(Vector2I movementVector, Vector2I[] newShape){
		if (checkValidMove(movementVector, newShape)){
			int[] coloursList = {0,2,4,6,8,10,12};
			drawShape(newShape, getCurrentPos(), currentShapeAtlasPos, movementVector);
		}
	}

	// Checks game over condition
	public bool checkGameOver(Vector2I[] currentShape){
		for (int i = 0; i < 4; i++){
			// if piece it out of board boundaries then game over
			if(!inBounds(currentShape[i] + getCurrentPos())){
				setPrevDrawPos(new Vector2I(0,0));
				setCurrentPos(new Vector2I(5,1));
			}
			// if piece is at top of the board or if new piece position overlaps existing piece then game over
			if ((currentShape[i] + getCurrentPos()).Y < 1 || board.getBoard()[currentShape[i].Y + getCurrentPos().Y, currentShape[i].X + getCurrentPos().X] == 1){
				return true;
			}
		}
		return false;
	}

	// Draw whole shape and change values accordingly
	public void drawShape(Vector2I[] shape, Vector2I pos, Vector2I atlasPos, Vector2I moveVect){
		// Set Previous Draw Piece and Pos for use in eraseShape
		setPrevDrawPiece(shape);
		setPrevDrawPos(pos);
		// Change current position
		setCurrentPos(getCurrentPos() + moveVect);
		for (int i = 0; i < 4; i++){
			SetCell(shape[i] + getCurrentPos(), 0, atlasPos);
		}
	}

	// Erase previous drawn piece
	public void eraseShape(Vector2I[] previousDrawPiece){
		for (int i = 0; i < 4; i++){
			EraseCell(previousDrawPiece[i] + getPrevDrawPos());
		}
	}

	// Go through EraseQueue and remove all stored positions
	public void clearQueue(){
		for (int i = 0; i < eraseQueue.getSize(); i++){
			EraseCell(eraseQueue.get(i));
			eraseQueue.dequeue();
		}
	}

	// Generate shapeID to get the position coordinates of the next Shape
	// 0 = i, 1 = l, 2 = j, 3 = c, 4 = s, 5 = t, z = 6
	public int decideNextShape(){
		Random random = new Random();
		int randomInt = random.Next(6);
		return randomInt;
	}

	// Reflect shape placement on display board to BoardMatrix
	public void placeShape(Vector2I[] currentShape){
		for (int i = 0; i < 4; i++){
			board.fillEmptySpot(currentShape[i] + getCurrentPos());
		}
		board.printBoard();
	}

	// Check if next move in valid - all squares are within the boundaries of the board
	public bool checkValidMove(Vector2I moveVect, Vector2I[] currentShape){
		for (int i = 0; i < 4; i++){
			if (!inBounds(currentShape[i] + getCurrentPos() + moveVect)){
				return false;
			}
		}
		return true;
	}

	// Check if single position is within board boundaries
	public bool inBounds(Vector2I pos){
		if (pos.X < 1 || pos.X > COLS || pos.Y < 0 || pos.Y > ROWS){
			return false;
		}
		return true;
	}

	// Check if coordinate can be placed (touching bottom of board)
	public bool isPlaceable(Vector2I pos, bool isTimeUp){
		bool timeUp = true; // checking that amount of time before placement is exceeded
		if (pos.Y == 20 && timeUp){
			return true;
		}
		return false;
	}

	// Check if entire piece can be placed - includes logic for stacking pieces
	public bool isPlaceablePiece(Vector2I[] coordinates){
		int[,] copy = board.getBoard();
		for (int i = 0; i < 4; i++){
			Vector2I temp = coordinates[i] + getCurrentPos();
			if (isPlaceable(temp, true) || copy[temp.Y + 1, temp.X] == 1){
				return true;
			}
		}
		return false;
	}

	// Process rows in the BoardMatrix that are full
	public int processFullRows(){
		int points = board.clearRows();
		List<int> eraseRows = board.getRowsToClear();

		// Iterate through eraseRows and remove each from display board
		foreach (int row in eraseRows){
			for (int i = 0; i < 12; i++){
				EraseCell(new Vector2I(i, row));
			}
		}
		for (int i = 20; i >= 0; i--){
			for (int j = 0; j < 12; j++){
				if (board.getBoard()[i, j] == 0){
					EraseCell(new Vector2I(i, j));
				}
			}
		}
		// According to how many rows cleared, increase score
		increaseScore(points);
		return points;
	}

	// Container for all piece coordinates and their rotated coordinates - call to get specific shape according to shapeID
	public Vector2I[] getShapeCoords(int shapeID){
		int rotation = getCurrentRotation();
		Vector2I[] defaultCoords = {new Vector2I(0,0), new Vector2I(0,0), new Vector2I(0,0), new Vector2I(0,0)};
		switch(shapeID){
			case 0:
				if(rotation == 0){
					return shapes.getI0Coords();
				} else if (rotation == 90){
					return shapes.getI90Coords();
				} else if (rotation == 180){
					return shapes.getI180Coords();
				} else if (rotation == 270){
					return shapes.getI270Coords();
				}
				break;
			case 1:
				switch(rotation){
					case 0:
						return shapes.getL0Coords();
						break;
					case 90:
						return shapes.getL90Coords();
						break;
					case 180:
						return shapes.getL180Coords();
						break;
					case 270:
						return shapes.getL270Coords();
						break;
				}
				break;
			case 2:
				switch(rotation){
					case 0:
						return shapes.getJ0Coords();
						break;
					case 90:
						return shapes.getJ90Coords();
						break;
					case 180:
						return shapes.getJ180Coords();
						break;
					case 270:
						return shapes.getJ270Coords();
						break;
				}
				break;
			case 3:
				return shapes.getSquareCoords();
				break;
			case 4:
				switch(rotation){
					case 0:
						return shapes.getS0Coords();
						break;
					case 90:
						return shapes.getS90Coords();
						break;
					case 180:
						return shapes.getS180Coords();
						break;
					case 270:
						return shapes.getS270Coords();
						break;
				}
				break;
			case 5:
				switch(rotation){
					case 0:
						return shapes.getT0Coords();
						break;
					case 90:
						return shapes.getT90Coords();
						break;
					case 180:
						return shapes.getT180Coords();
						break;
					case 270:
						return shapes.getT270Coords();
						break;
				}
				break;
			case 6:
				switch(rotation){
					case 0:
						return shapes.getZ0Coords();
						break;
					case 90:
						return shapes.getZ90Coords();
						break;
					case 180:
						return shapes.getZ180Coords();
						break;
					case 270:
						return shapes.getZ270Coords();
						break;
				}
				break;
		}
		return defaultCoords;
	}


	// Setters and Getters
	public int getSpeed(){
		return speed;
	}

	public void setSpeed(int newSpeed){
		speed = newSpeed;
	}

	public int getGravity(){
		return gravity;
	}

	public void setGravity(int newGravity){
		gravity = newGravity;
	}

	public int getSteps(){
		return steps;
	}

	public void setSteps(int newSteps){
		steps = newSteps;
	}
	
	public Vector2I getCurrentPos(){
		return currentPos;
	}

	public void setCurrentPos(Vector2I newPos){
		currentPos = newPos;
	}

	public Vector2I[] getActivePiece(){
		return activePiece;
	}

	public Vector2I[] getPrevDrawPiece(){
		return previousPieceDrawPos;
	}

	public void setPrevDrawPiece(Vector2I[] piece){
		previousPieceDrawPos = piece;
	}
	
	public Vector2I getPrevDrawPos(){
		return prevDrawPos;
	}

	public void setPrevDrawPos(Vector2I newPrevDrawPos){
		prevDrawPos = newPrevDrawPos;
	}

	public int getScore(){
		return score;
	}

	// Increment local and global score
	public void increaseScore(int scorePoints){
		score += scorePoints;
		Global.Instance.score += scorePoints;
	}

	public int getCurrentRotation(){
		return currentRotation;
	}

	// only for use when getting a new shape
	public void resetRotation(){
		currentRotation = 0;
	}

	// Rotate shape
	// if > 270 loop back to original rotation
	public void rotateShape(){
		currentRotation = (currentRotation + 90 ) % 360;
	}
}
