using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsPlayer : MonoBehaviour
{
    [SerializeField][TextArea]
    private string[] tips;

    [SerializeField]
    private Text tipLabel;

    private void Start()
    {
        NextTip();    
    }

    public void NextTip()
    {
        tipLabel.text = tips[Random.Range(0, tips.Length)].ToUpper();
    }
}
