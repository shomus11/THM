using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    Interactor interactor;
    float waitTime = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Interact(Interactor interactor)
    {
        Debug.Log("Interact with NPC");
        this.interactor = interactor;
        DoInteract();
    }

    public void OnFinishedInteract()
    {
        interactor.OnFinishedInteract();
    }

    public void DoInteract()
    {
        StartCoroutine(StartInteract());
    }

    public IEnumerator StartInteract()
    {
        yield return new WaitForSeconds(.5f);
        OnFinishedInteract();
    }
}
