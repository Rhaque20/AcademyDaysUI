using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ListOption : MonoBehaviour
{
    // Start is called before the first frame update
    public string optName;
    private TMP_Text name, subtext;
    private Image sprite;
    DropdownManager dm;
    [SerializeField]TMPro.TMP_InputField inputField;
    void Start()
    {
        
    }

    public void Initialize(DropdownManager d)
    {
        name = transform.GetChild(0).GetComponent<TMP_Text>();
        subtext = transform.GetChild(1).GetComponent<TMP_Text>();
        sprite = transform.GetChild(2).GetComponent<Image>();
        dm = d;
    }

    public void activateFunction()
    {
        switch((int)dm.dropState)
        {
            case 0:
                TriggerTimeEffect();
                break;
            case 1:
                CRPush();
                break;
        }
    }

    public void TriggerTimeEffect()
    {
        dm.ApplyTimeEffect(optName);
    }

    public void CRPush()
    {
        float crVal = 0f;
        bool success = false;
        success = float.TryParse(inputField.text,out crVal);
        if (success)
            dm.CRPush(optName,crVal);
        
    }

    public void SetUpContent(string s1, string s2, Sprite s, int i)
    {

        this.gameObject.SetActive(true);
        name.text = s1;
        optName = s1;

        if (s2 != null)
        {
            transform.GetChild(1).gameObject.SetActive(true);
            subtext.text = s2;
        }
        else
        {
            this.transform.GetChild(1).gameObject.SetActive(false);
        }

        if (s != null)
        {
            this.transform.GetChild(2).gameObject.SetActive(true);
            sprite.sprite = s;
        }
        else
        {
            this.transform.GetChild(2).gameObject.SetActive(false);
        }
        //index = i;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
