using Godot;
using System;

public partial class player : Node2D
{
	[Export] public float RotationSpeed = 1;
	[Export] public float LinearSpeed = 0;

	[Export] public float Distance = 1;
	[Export] public float defaultDistance = 24;
	[Export] public float DistancingSpeed = 0.75f;
	[Export] public float MinDistance = 0.5f;
	[Export] public float MaxDistance = 1.5f;
	private int distanceInPixels => (int)(Distance * defaultDistance);

    [Export] private NodePath moonReference;
    [Export] private NodePath sunReference;

	private Node2D moon;
	private Node2D sun;

	public bool MoonHeld;
	public bool SunHeld;

	private Vector2 movementVector;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		moon = (Node2D)GetNode(moonReference);
		sun = (Node2D)GetNode(sunReference);
	}

    public override void _Input(InputEvent @event)
    {
		if (@event.IsAction("A"))
		{
			if (@event.IsPressed() && !MoonHeld) 
			{
				MoonHeld = true;
				Lock(moon, sun);
			} else if (@event.IsReleased() && MoonHeld)
			{
				MoonHeld = false;
                Release(moon, sun);
            }
		} 
		else if (@event.IsAction("B"))
		{

		}
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		Rotation += (RotationSpeed * (float)delta);

        CalculateDistance(delta);
        SetBodyPositions();
    }

	private void CalculateDistance(double delta)
	{
        float axis = Input.GetAxis("DPadDown", "DPadUp");
        Distance = (float)Mathf.Clamp(Distance + ((axis * DistancingSpeed) * delta), MinDistance, MaxDistance);
    }

	private void SetBodyPositions()
	{
        moon.Position = new Vector2(-distanceInPixels, moon.Position.Y);
        sun.Position = new Vector2(distanceInPixels, sun.Position.Y);
    }

    public void Lock(Node2D locked, Node2D other)
    {
		/*
        if (focus == sun) sunSprite.Lock();
        if (focus == moon) moonSprite.Lock();
		*/
            
        movementVector = Vector2.Zero;

        Vector2 presentPosition = GlobalPosition;
        Vector2 lockedOffset = locked.GlobalPosition - presentPosition;
        Vector2 otherOffset = other.GlobalPosition - presentPosition;

        // AudioManager.Instance.PlaySFX("Lock");

        Position = locked.GlobalPosition;
        other.GlobalPosition += otherOffset;
        locked.GlobalPosition -= lockedOffset;
    }

    public void Release(Node2D locked, Node2D other)
	{
        /*
        if (focus == sun)
        {
            if (!SunLocked) return;
            sunSprite.Unlock();
            SunLocked = false;
        }
        if (focus == moon)
        {
            if (!MoonLocked) return;
            moonSprite.Unlock();
            MoonLocked = false;
        }
        */

        Vector2 offset = (other.GlobalPosition - locked.GlobalPosition) / 2;

        // RecalculateSpeed();

        Vector2 direction = (other.GlobalPosition - locked.GlobalPosition);
        direction = new Vector2(direction.Y, -direction.X);

        // AudioManager.Instance.PlaySFX("Release");

        // MovementVector = (Quantize(direction, 16) * -spin);

        GlobalPosition += offset;
        other.GlobalPosition -= offset;
        locked.GlobalPosition -= offset;
    }
}
