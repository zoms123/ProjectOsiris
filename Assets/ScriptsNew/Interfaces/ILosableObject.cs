using System;

public interface ILosableObject
{
    public event Action OnLoseObject;
}
