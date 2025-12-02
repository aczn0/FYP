using Godot;
using System;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;

public partial class QResultsToFile : Node2D
{

	public void resultsToFile(List<int> epochs, List<int> reward){
		try{
			string jsonString = JsonSerializer.Serialize((epochs.ToArray(), reward.ToArray()));
			File.WriteAllText(@"C:\Users\Gabriel\PROJECT\product\qresults.json", jsonString);
		}
		catch (Exception e){
			GD.Print("ERROR: Could not save Q table -> " + e.Message);
		}
	}
}
