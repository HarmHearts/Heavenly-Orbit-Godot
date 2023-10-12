using Godot;
using System;

public partial class Player : Node2D
{
	[Export]
	public float rotationSpeed;
	[Export]
	public float quantization;
	[Export]
	public float distanceSpeed;
	[Export]
	public float minDistance;
	[Export]
	public float maxDistance;
	[Export]
	public Node2D sun;
	[Export]
	public Node2D moon;
	[Export]
	public Node2D shifter;

	public float bodyDistance = 24;

	public bool locked;
	/// <summary>
	/// true for sun false for moon
	/// </summary>
	public bool lockedBody;

	public Vector2 moveDirection;
	public float moveSpeed;

	//input stuff
	private bool upHeld;
	private bool downHeld;

	[Signal]
	public delegate void SunLockedEventHandler();
	[Signal]
	public delegate void MoonLockedEventHandler();
	[Signal]
	public delegate void UnlockEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		//always be rotating
		this.Rotation += rotationSpeed * (float)delta;
		//do distancing
		if(upHeld)
		{
			bodyDistance = Mathf.MoveToward(bodyDistance, maxDistance, distanceSpeed * (float)delta);
		}
        if (downHeld)
        {
            bodyDistance = Mathf.MoveToward(bodyDistance, minDistance, distanceSpeed * (float)delta);
        }
		//set distance between bodies
		sun.Position = new Vector2(bodyDistance, 0);
        moon.Position = new Vector2(-bodyDistance, 0);
		//locked behaviors
		if(locked)
		{
            //set shifter position
            if (lockedBody) shifter.Position = new Vector2(-bodyDistance, 0);
			else shifter.Position = new Vector2(bodyDistance, 0);
			//calculate launch direction
			float launchAngle = Mathf.DegToRad(Mathf.Round(this.RotationDegrees / quantization) * quantization);
			if(rotationSpeed > 0) moveDirection = Vector2.FromAngle(launchAngle + 1.570796f);
			else moveDirection = Vector2.FromAngle(launchAngle - 1.570796f);
			if (lockedBody) moveDirection *= -1;
			moveSpeed = Mathf.Abs(rotationSpeed * (bodyDistance));
        }
		//unlocked behaviors
		else
		{
			this.Position += moveDirection * (moveSpeed * (float)delta);
		}
    }

    public override void _Input(InputEvent @event)
    {
		//get lock actions
		if(@event.IsActionPressed("Btn_A"))
		{
			if(!locked)
			{
				LockSun();
			}
			else if(locked && !lockedBody)
			{
				UnLock();
				LockSun();
			}
		}
		if(@event.IsActionReleased("Btn_A"))
		{
			if(locked && lockedBody)
			{
				UnLock();
			}
		}
        if (@event.IsActionPressed("Btn_B"))
        {
            if (!locked)
            {
                LockMoon();
            }
            else if (locked && lockedBody)
            {
                UnLock();
                LockMoon();
            }
        }
        if (@event.IsActionReleased("Btn_B"))
        {
            if (locked && !lockedBody)
            {
                UnLock();
            }
        }
        //get distance input state
        if (@event.IsActionPressed("Dpad_Up"))
        {
            upHeld = true;
			downHeld = false;
        }
		else if (@event.IsActionReleased("Dpad_Up"))
		{
			upHeld = false;
		}
        if (@event.IsActionPressed("Dpad_Down"))
        {
            downHeld = true;
            upHeld = false;
        }
        else if (@event.IsActionReleased("Dpad_Down"))
        {
            downHeld = false;
        }
    }

	private void UnLock()
	{
        this.Position = sun.GlobalPosition.Lerp(moon.GlobalPosition, 0.5f);
        shifter.Position = Vector2.Zero;
		locked = false;
		EmitSignal(SignalName.Unlock);
	}

	private void LockSun()
	{
		//do lockability check here
		locked = true;
		lockedBody = true;
		this.Position = sun.GlobalPosition;
		EmitSignal(SignalName.SunLocked);
	}

    private void LockMoon()
    {
        //do lockability check here
        locked = true;
        lockedBody = false;
        this.Position = moon.GlobalPosition;
        EmitSignal(SignalName.MoonLocked);
    }
}
