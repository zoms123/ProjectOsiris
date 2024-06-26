using System;

public interface IActivable 
{
    public event Action OnActivated;
    public event Action OnDeactivated;
}
