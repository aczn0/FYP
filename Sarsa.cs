using Godot;
using System;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;

public partial class Sarsa : Node {
	private int states;
	private int[] actions = {0, 1, 2, 3}; // actions represented by integers for simplicity's sake (0 = left, 1 = right, 2 = rotate, 3 = hard drop)
	private float learningRate;
	private float[,] qTable;

	private Dictionary<string, float[]> qDict = new Dictionary<string, float[]>();

	private string qTableFilePath = @"C:\Users\Gabriel\PROJECT\product\SARSATable.json";

	public Random rand = new Random();

	// hyperparameters
	float alpha = 0.1f; // learning rate
	double epsilon = 0.4; // exploration rate
	float gamma = 0.9f; // discount factor

	public override void _Ready(){
		initialise(true);
	}

	// initiliase new variables and load q table from file if useTableFromFile true
	public void initialise(bool useTableFromFile){
		this.states = 0;

		if (useTableFromFile){
			loadQ();
		}
	}

	// choose best action for current state
	public int chooseAction(string state){
		// if random number is less than exploration rate, then explore (dont choose best action)
		if (rand.NextDouble() < epsilon){
			return rand.Next(actions.Length);
		}
		
		if (!qDict.ContainsKey(state)){
			qDict.Add(state, new float[actions.Length]);
		}

		float largestQValue = qDict[state][0]; 
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

	// update q value in q table for current state-action pair
	public float update(int action, string state, int curReward, string next_state, int next_action){ 
		float qValue =  qDict[state][action] + alpha * (curReward + gamma * qDict[next_state][next_action] - qDict[state][action]);
		qDict[state][action] = qValue;
		saveQ();
		return qValue;
	}

	public void saveQ(){// saves q table to file
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
