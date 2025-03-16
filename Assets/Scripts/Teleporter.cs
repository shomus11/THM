using System.Collections;
using UI.Utiliy;
using UnityEngine;

public class Teleporter : MonoBehaviour, IInteractable
{
    [SerializeField] string sceneName;
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
        Debug.Log($"Teleport to {sceneName}");
        UITransition.instance.FadeOut(sceneName);
    }

    public void OnFinishedInteract()
    {
        throw new System.NotImplementedException();
    }

    public void DoInteract()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator StartInteract()
    {
        throw new System.NotImplementedException();
    }
}
