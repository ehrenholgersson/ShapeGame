using System.Collections.Generic;
using UnityEngine;

// struct to store touch input between frames
public struct TouchData
{
    public Vector3 Position;
    public Vector3 StartPosition;
    public int Id;
    public float StartTime;

    public float Duration { get => Time.time - StartTime; }

    public TouchData(Vector3 position, int id)
    {
        Position = position;
        StartPosition = position;
        Id = id;
        StartTime = Time.time;
    }
    public TouchData(Touch touch)
    {
        Position = touch.position;
        StartPosition = touch.position;
        Id = touch.fingerId; 
        StartTime = Time.time;
    }
}

// a list of the above with functions to easily retrive some information based on fingerId
public struct TouchInputs
{

    public List<TouchData> Touches;

    public float Duration(int id)
    {
        if (TryGetTouch(id, out TouchData touch))
        {
            return touch.Duration;
        }
        return 0;
    }

    public bool TryGetTouch(int id, out TouchData t)
    {
        if (Touches ==null) // becuase C# won't allow for parameterless constructors we need to initialize the list here
        {
            Touches = new List<TouchData>();
        }
        t = new TouchData();
        foreach (TouchData touch in Touches)
        {
            if (touch.Id == id)
            {
                t = touch;
                return true;
            }
        }
        return false;
    }

    public void Add(TouchData touch)
    {
        if (Touches == null)// becuase C# won't allow for parameterless constructors we need to initialize the list here
        {
            Touches = new List<TouchData>();
        }
        Touches.Add(touch);
    }

    public void Add(Touch touch)
    {
        if (Touches == null)// becuase C# won't allow for parameterless constructors we need to initialize the list here
        {
            Touches = new List<TouchData>();
        }

        Touches.Add(new TouchData(touch));
    }

    // Intent is for input code to remove the TouchData whenever phase = ended, the below is to check this is working
    public void CheckTouches()
    {
        foreach (TouchData touch in Touches)
        {
            if (touch.Duration > 10)
                Debug.Log("Old touch in data, ID: " + touch.Id + " for " + touch.Duration + " seconds");
        }
    }
    public void Remove (TouchData touch)
    {
        if (Touches.Contains(touch))
        {
            Touches.Remove(touch);
        }
    }
}
