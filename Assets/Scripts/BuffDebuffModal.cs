using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDebuffModal : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]GameObject _statusIconPrefab;
    [SerializeField]Transform _iconList,_charList;
    void Start()
    {
        GameObject temp;
        for(int i = 0; i < 10; i++)
        {
            temp = Instantiate(_statusIconPrefab);
            temp.transform.SetParent(_iconList);
            temp.transform.localScale = new Vector2(1f,1f);
            temp.SetActive(false);
        } 
    }

    // Update is called once per frame
    
}
