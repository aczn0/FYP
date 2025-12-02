using Godot;
using System;

public partial class EraseQueue : Node2D
{
	private Godot.Collections.Array queue;
	private int queueSize;

	public override void _Ready(){
		queue = new Godot.Collections.Array{};
		queueSize = 0;
	}

	public Godot.Collections.Array getQueue(){
		return queue;
	}

	public Vector2I get(int index){
		Vector2I queueItem = (Vector2I)queue[index];
		return queueItem;
	}

	public void enqueue(Vector2I position){
		queue.Insert(0, position);
		queueSize += 1;
	}

	public Vector2I dequeue(){
		Vector2I returnVect = get(0);
		queue.RemoveAt(queueSize - 1);
		return returnVect;
	}

	public int getSize(){
		return queueSize;
	}
}
