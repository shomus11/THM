using UnityEngine;
using UnityEngine.InputSystem;
public class Interactor : MonoBehaviour
{
    [SerializeField] float interactPointRadius = .5f;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] int numFound;
    [SerializeField] bool onInteraction = false;
    [SerializeField] GameObject interactionIndicator;
    [SerializeField] PlayerController player;
    Collider2D[] colliders = new Collider2D[3];
    bool canInteract = false;
    IInteractable targetedInteractObject;

    public bool OnInteraction { get => onInteraction; set => onInteraction = value; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowInteractionIndicator(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!OnInteraction)
            CheckInteraction();
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(transform.position, interactPointRadius);
    //}

    void CheckInteraction()
    {
        numFound = Physics2D.OverlapCircleNonAlloc(transform.position, interactPointRadius, colliders, interactableLayer);

        if (numFound > 0)
        {
            targetedInteractObject = colliders[0].GetComponent<IInteractable>();
            if (targetedInteractObject != null)
            {
                canInteract = true;
                if (!OnInteraction)
                    ShowInteractionIndicator(true);

                //if (Input.GetKeyDown(KeyCode.F))
                //    Interact(interactable);
            }
        }
        else
        {
            ShowInteractionIndicator(false);
            OnInteraction = false;
            targetedInteractObject = null;
            canInteract = false;
        }
    }
    public void InteractInput(InputAction.CallbackContext context)
    {
        if (canInteract)
            if (targetedInteractObject != null)
                Interact(targetedInteractObject);
    }
    void Interact(IInteractable interactable)
    {
        interactable.Interact(this);
        ShowInteractionIndicator(false);
        player.CanMove = false;
        OnInteraction = true;
        canInteract = false;

    }

    public void OnFinishedInteract()
    {
        ShowInteractionIndicator(true);
        player.CanMove = true;
        OnInteraction = false;
        canInteract = true;
    }

    public void ShowInteractionIndicator(bool set)
    {
        if (interactionIndicator != null)
            interactionIndicator.gameObject.SetActive(set);
    }
}
