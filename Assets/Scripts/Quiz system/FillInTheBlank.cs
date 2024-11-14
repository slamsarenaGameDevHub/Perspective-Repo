using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FillInTheBlank : MonoBehaviour
{
    public List<Question> AllQuestions;
    [HideInInspector]
    public List<Question> availableQuestions;
    [HideInInspector]
    public List<int> attemptedQuestionsId;


    int currentQuestion = 0;
    [SerializeField] TMP_Text QuestionText;
    [SerializeField] TMP_InputField answerField;
    [SerializeField] GameObject QuestionOverlay;

    WalkingNPC askingNpc;
    private void Start()
    {
        QuestionOverlay.SetActive(false);
    }
    public void AskQuestion(WalkingNPC npcAsking)
    {
        UpdateQuestions();
        askingNpc= npcAsking;
    }
    void Update()
    {
        if (QuestionOverlay.activeInHierarchy) return;
        currentQuestion = Random.Range(0, availableQuestions.Count);
    }
    void UpdateQuestions()
    {
        availableQuestions = new List<Question>();
        foreach (Question question in AllQuestions)
        {
            if (!attemptedQuestionsId.Contains(question.QuestionId))
            {
                availableQuestions.Add(question);
            }
        }
        GiveQuestion();
        QuestionOverlay.SetActive(true);
    }
    void GiveQuestion()
    {
        for (int i = 0; i < availableQuestions.Count; i++)
        {
            if (currentQuestion == i)
            {
                QuestionText.text = availableQuestions[currentQuestion].QuestionSentence;
            }
        }
    }
    public void Submit()
    {

        if (availableQuestions[currentQuestion].Answers.Contains(answerField.text))
        {
            print("Correct answer");
        }
        else
        {
            print("wrong answer");
        }
        askingNpc.ChangeState();
        askingNpc=null;

    }


}
[System.Serializable]
public class Question
{
    [TextArea(7, 10)]
    public string QuestionSentence;
    public List<string> Answers;
    public int QuestionId;
    public int MoneyReward;
}