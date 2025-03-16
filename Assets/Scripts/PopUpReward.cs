using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PopUpReward : MonoBehaviour
{
    public static PopUpReward instance;

    [SerializeField] RectTransform popUpRewardPanel;
    [SerializeField] Image popUpRewardItem;
    [SerializeField] float popUpRewardItemImageMultipler;

    [SerializeField] float baseAnimationDuration = 0.25f;
    [SerializeField] bool onPopIn = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    if (onPopIn)
        //    {
        //        PopOut();
        //    }
        //}
    }
    public void PopIn()
    {
        float totalAnimationDuration = 0;
        Sequence sequence = DOTween.Sequence();
        sequence.Insert(totalAnimationDuration, popUpRewardPanel.DOScaleY(0, baseAnimationDuration).From(1).SetEase(Ease.OutBack));
        totalAnimationDuration += baseAnimationDuration;
        sequence.Insert(totalAnimationDuration, popUpRewardItem.rectTransform.DOScale(Vector3.zero, baseAnimationDuration).From(1).SetEase(Ease.OutBack));
    }
    public void PopIn(Sprite rewardTarget = null)
    {
        if (!onPopIn)
        {
            popUpRewardItem.sprite = rewardTarget;
            popUpRewardPanel.gameObject.SetActive(true);
            popUpRewardItem.gameObject.SetActive(true);

            popUpRewardItem.SetNativeSize();
            float width = popUpRewardItem.rectTransform.rect.width * popUpRewardItemImageMultipler;
            float height = popUpRewardItem.rectTransform.rect.height * popUpRewardItemImageMultipler;
            Vector2 size = new Vector2(width, height);
            popUpRewardItem.rectTransform.sizeDelta = size;

            float totalAnimationDuration = 0;
            Sequence sequence = DOTween.Sequence();
            sequence.Insert(totalAnimationDuration, popUpRewardItem.rectTransform.DOScale(Vector3.one, baseAnimationDuration).From(Vector3.zero).SetEase(Ease.OutBack));
            totalAnimationDuration += baseAnimationDuration;
            sequence.Insert(totalAnimationDuration, popUpRewardPanel.DOScale(1, baseAnimationDuration).From(0).SetEase(Ease.OutBack)).OnComplete(() =>
            {
                onPopIn = true;
            });
        }
    }

    public void PopOut()
    {
        float totalAnimationDuration = 0;
        Sequence sequence = DOTween.Sequence();
        sequence.Insert(totalAnimationDuration, popUpRewardPanel.DOScale(0, baseAnimationDuration).From(1).SetEase(Ease.Linear));
        totalAnimationDuration += baseAnimationDuration;
        sequence.Insert(totalAnimationDuration, popUpRewardItem.rectTransform.DOScale(Vector3.zero, baseAnimationDuration).From(1).SetEase(Ease.Linear)).OnComplete(() =>
        {
            popUpRewardPanel.gameObject.SetActive(false);
            popUpRewardItem.gameObject.SetActive(false);
            PlayerController.instance.GetComponent<Interactor>().OnFinishedInteract();
            onPopIn = false;
        });
    }
    int counter = 0;
    public void OnCompleteInteraction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SoundManager.Instance.PlaySE("Submit");
            //    Debug.Log("test");
            if (onPopIn)
                PopOut();
        }
        //{
        //    counter++;
        //    PopOut();
        //}
    }
}
