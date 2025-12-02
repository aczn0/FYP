using Godot;
using System;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
public partial class SarsaResultsToFile : Node2D
{
	public void resultsToFile(List<int> epochs, List<int> reward){
		try{
			GD.Print(epochs.ToString());
			GD.Print(reward.ToString());
			string jsonString = JsonSerializer.Serialize((epochs.ToArray(), reward.ToArray()));
			File.WriteAllText(@"C:\Users\Gabriel\PROJECT\product\sarsaresults.json", jsonString);
		}
		catch (Exception e){
			GD.Print("ERROR: Could not save Q table -> " + e.Message);
		}
	}
}
