using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KnightTrainingButtonController : MonoBehaviour
{
    [SerializeField] private KingdomManager kingdomManager;
    [SerializeField] private Button trainingButton;
    [SerializeField] private TMP_Text buttonText;

    [SerializeField] private float cooldownDuration = 5f;

    private float cooldownTimer;
    private bool isOnCooldown;

    private void Awake()
    {
        if (trainingButton == null)
            trainingButton = GetComponent<Button>();

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

    public void TrainKnight()
    {
        if (isOnCooldown)
            return;

        bool trained = kingdomManager.TrainKnight();

        if (!trained)
        {
            buttonText.text = "No Resources";
            return;
        }

        isOnCooldown = true;
        cooldownTimer = cooldownDuration;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (trainingButton != null)
            trainingButton.interactable = !isOnCooldown;

        if (buttonText == null)
            return;

        if (isOnCooldown)
        {
            buttonText.text = Mathf.CeilToInt(cooldownTimer).ToString();
            return;
        }

        buttonText.text = "+Knight";
    }
}