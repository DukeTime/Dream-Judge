using UnityEngine;


namespace Map.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        // public bool Enabled { get; private set; } = true;
        //
        // public void Enable() => Enabled = true;
        // public void Disable() => Enabled = false;
        
        public abstract void Interact();
    }
}