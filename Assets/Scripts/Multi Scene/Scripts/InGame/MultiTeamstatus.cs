using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiTeamstatus : MonoBehaviour
{
    public TMP_Text textName;
    public GameObject statusbar;
    private GridLayoutGroup gridLayoutGroup;
    public Image playerHpImage;
    void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        if (MultiScene.Instance.currentUser.Equals(gameObject.name))
        {
            statusbar.SetActive(true);
            textName.text = gameObject.name;
        }
        else
        {
            gameObject.SetActive(false);
        }    
    }

    void Update()
    {
        
    }
}
