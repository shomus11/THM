using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour, IInteractable
{
    Interactor interactor;
    float waitTime = 1f;
    [SerializeField] List<Sprite> rewards;
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
        if (!interactor.OnInteraction)
        {
            Debug.Log("Interact Chest");
            this.interactor = interactor;
            DoInteract();
        }
    }

    public void OnFinishedInteract()
    {
        //interactor.OnFinishedInteract();
    }

    public void DoInteract()
    {
        Sprite reward = rewards[Random.Range(0, rewards.Count)];
        PopUpReward.instance.PopIn(reward);
        SoundManager.Instance.PlaySE("Submit");
        //StartCoroutine(StartInteract());
    }

    public IEnumerator StartInteract()
    {
        yield return new WaitForSeconds(waitTime);
        OnFinishedInteract();
    }
}
