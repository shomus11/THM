using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    Interactor interactor;
    [SerializeField] float waitTime = 1f;
    public Canvas characterDialog;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterDialog.gameObject.SetActive(false);
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
        characterDialog.gameObject.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        characterDialog.gameObject.SetActive(false);
        OnFinishedInteract();
    }
}
