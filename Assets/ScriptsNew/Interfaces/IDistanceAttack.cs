using UnityEngine;

public interface IDistanceAttack 
{
    public void Initialize(Vector3 direction, string ownerTag, Transform spawnpoint = default);

}