using System.Collections;

public interface IInteractable
{
    public void Interact(Interactor interactor);
    public void OnFinishedInteract();
    public void DoInteract();
    public IEnumerator StartInteract();
}
