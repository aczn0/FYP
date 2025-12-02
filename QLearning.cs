using Godot;
using System;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;

public partial class QLearning : Node
{
	private int states;
	private int[] actions = {0, 1, 2, 3}; // actions represented by integers for simplicity's sake (0 = left, 1 = right, 2 = rotate, 3 = hard drop)
	private float learningRate;
	private float[,] qTable;

	private Dictionary<string, float[]> qDict = new Dictionary<string, float[]>();

	private string qTableFilePath = @"C:\Users\Gabriel\PROJECT\product\QTable.json";

	public Random rand = new Random();

	// hyperparameters
	float alpha = 0.7f; // learning rate
	double epsilon = 0.1; // exploration chance
	float gamma = 0.9f; // discount factor

	public override void _Ready(){
		initialise(true);
	}

	// initiliase new variables and load q table from file if useTableFromFile true
	public void initialise(bool useTableFromFile){
		this.states = 0;

		qTable = new float[states, actions.Length];

		if (useTableFromFile){
			loadQ();
		}
	}

	// choose new best action for current state
	public int chooseAction(string state){
		// if random number is less than exploration rate, then explore (dont choose best action)
		if (rand.NextDouble() < epsilon){
			return rand.Next(actions.Length);
		}
		
		if (!qDict.ContainsKey(state)){
			float[] initQVals = new float[actions.Length];
			for (int i = 0; i < actions.Length; i++){ // assign random small q values to the new state --> more exploration possible 
				initQVals[i] = 0.01f * (float)rand.NextDouble(); // multiply by 0.01 to prevent values from becoming too large
			}
			qDict.Add(state, initQVals);
		}

		float largestQValue = 0f; // arbitrary small value
		int choice = 0;

		// find largest q value in q table -> choose next best action
		for (int i = 0; i < actions.Length; i++){
			if (qDict[state][i] > largestQValue){
				largestQValue = qDict[state][i];
				choice = i;
			}
		}

		return choice;
	}

	// update q value for state-action pair in q-table
	public float update(int action, string state, int curReward){ 
		float largestQValue = 0.0001f; // arbitrary small value
		int choice = 0;

		for (int i = 0; i < actions.Length; i++){
			if (qDict[state][i] > largestQValue){
				largestQValue = qDict[state][i];
				choice = i;
			}
		}
		// GD.Print("Current reward" + curReward);
		float qValue = qDict[state][action] + alpha * (curReward + (gamma * largestQValue) - qDict[state][action]);
		qDict[state][choice] = qValue;
		saveQ();
		return qValue;
	}

	public void saveQ(){ // saves q table to file
		try{
			string jsonString = JsonSerializer.Serialize(qDict);
			File.WriteAllText(getFilePath(), jsonString);
		}
		catch (Exception e){
			GD.Print("ERROR: Could not save Q table -> " + e.Message);
		}
	} 

	public void loadQ(){ // loads q table from file
		try{
			string jsonString = File.ReadAllText(getFilePath());
			qDict = JsonSerializer.Deserialize<Dictionary<string, float[]>>(jsonString);
		}
		catch (Exception e){
			GD.Print("ERROR: Could not load Q table -> " + e.Message);
		}
	} 

	public string getFilePath(){
		return qTableFilePath;
	}

	public string getDict(){
		string stringDict = "";
		foreach (KeyValuePair<string, float[]> kvp in qDict)
		{
			stringDict += ("Key = ", kvp.Key, ", Value = ", kvp.Value," \n");
		}
		return stringDict;
	}
}
