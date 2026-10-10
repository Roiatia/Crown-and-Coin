using UnityEngine;
using TMPro;

public class PlayerFeedbackUI : MonoBehaviour
{

    public static PlayerFeedbackUI Instance
    {
        get;
        private set;
    }


    [SerializeField] private TMP_Text feedBackText;
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private float duration = 2f;

    private float timer;

    private void Awake()
    {
        Instance = this;

        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
    
    }

    

    // Update is called once per frame
    void Update()
    {
        if (feedbackPanel == null || !feedbackPanel.activeSelf)
        {
            return;
        }
            

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            feedbackPanel.SetActive(false);
        }
            
    }

    public void ShowMessage(string message)
    {
        if (feedbackPanel == null ||  feedBackText == null)
        {
            return;
        }

        feedBackText.text = message;
        feedbackPanel.SetActive(true);
        timer = duration;

    }
}
