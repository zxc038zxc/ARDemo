using UnityEngine;

public abstract class InitializeMonoBehaviour : MonoBehaviour, IInitializable
{
    public abstract void Initialize();
    public virtual void Deinitialize() { }
}
