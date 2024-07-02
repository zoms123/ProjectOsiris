using System;

public interface ILosableObject
{
    public event Action OnLoseObject;

    public void LoseObject();
}
