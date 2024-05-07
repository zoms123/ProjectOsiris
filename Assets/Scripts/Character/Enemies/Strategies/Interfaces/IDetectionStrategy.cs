using UnityEngine;

public interface IDetectionStrategy
{
    bool Execute(out Transform foundTarget);
}
