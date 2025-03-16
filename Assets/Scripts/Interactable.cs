using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    Interactor interactor;
    float waitTime = 0.5f;
    public float WaitTime { get => waitTime; set => waitTime = value; }

    public virtual void Interact(Interactor interactor)
    {
        this.interactor = interactor;
        DoInteract();
    }
    public virtual void DoInteract()
    {
        StartCoroutine(StartInteract());
    }
    public virtual void OnFinishedInteract()
    {
        interactor.OnFinishedInteract();
    }

    public virtual IEnumerator StartInteract()
    {
        yield return new WaitForSeconds(waitTime);
        OnFinishedInteract();
    }
}
