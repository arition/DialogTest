using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class FeelingIndicator : MonoBehaviour
{
    public int CurrentExp;
    public int[] FeelingExpList = { 10, 80, 180, 500, 1000 };

    public Animator HeartAnimator;

    public TMP_Text Level;

    public TMP_Text Progress;

    private int CurrentLevel
    {
        get
        {
            for (var i = 0; i < FeelingExpList.Length; i++)
                if (CurrentExp < FeelingExpList[i])
                    return i + 1;
            return FeelingExpList.Length + 1;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        Level?.SetText(CurrentLevel.ToString());
        Progress?.SetText($"{CurrentExp}/{FeelingExpList[CurrentLevel - 1]}");
    }

    public void SetExp(int exp)
    {
        CurrentExp = exp;
        Level?.SetText(CurrentLevel.ToString());
        Progress?.SetText($"{CurrentExp}/{FeelingExpList[CurrentLevel - 1]}");
    }

    public void AddExp(int exp)
    {
        var currentLevel = CurrentLevel;
        CurrentExp += exp;
        if (currentLevel != CurrentLevel) HeartAnimator?.SetTrigger("heartbeat");
        Level?.SetText(CurrentLevel.ToString());
        Progress?.SetText($"{CurrentExp}/{FeelingExpList[CurrentLevel - 1]}");
    }
}