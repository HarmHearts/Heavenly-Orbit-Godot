using Godot;
using System;

public partial class CounterRotate : Node2D
{
	[Export]
	Node2D parent;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.Transform = new Transform2D(-parent.Transform.Rotation, this.Transform.Origin);
	}
}
