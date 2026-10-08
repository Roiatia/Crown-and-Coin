using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaterButtonController : MonoBehaviour
{
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private Button waterButton;
    [SerializeField] private TMP_Text buttonText;

    [SerializeField] private int waterReward = 10;
    [SerializeField] private float cooldownDuration = 8f;

    private float cooldownTimer;
    private bool isOnCooldown;

    private void Awake()
    {
        if (waterButton == null)
            waterButton = GetComponent<Button>();

        UpdateVisuals();
    }

    private void Update()
    {
        if (!isOnCooldown)
            return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            cooldownTimer = 0f;
            isOnCooldown = false;
        }

        UpdateVisuals();
    }

    public void CollectWater()
    {
        if (isOnCooldown)
            return;

        resourceManager.AddResource(ResourceType.Water, waterReward);

        if(PlayerFeedbackUI.Instance !=  null )
        {
            PlayerFeedbackUI.Instance.ShowMessage(" Huzzah! The water hath been gathered!");
        }

        isOnCooldown = true;
        cooldownTimer = cooldownDuration;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (waterButton != null)
            waterButton.interactable = !isOnCooldown;

        if (buttonText == null)
            return;

        buttonText.text = isOnCooldown ? Mathf.CeilToInt(cooldownTimer).ToString() : "+Water";
    }
}