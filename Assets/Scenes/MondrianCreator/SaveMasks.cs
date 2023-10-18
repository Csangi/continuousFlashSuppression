using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMasks: MonoBehaviour
{
    public Transform box;
    public Transform saveMasksTab;
    public Transform buttonImage;

    public void buttonPress()
    {
        if(box.gameObject.activeSelf)
        {
            box.LeanMoveLocalY(-400, 0.5f).setEaseInExpo();
            saveMasksTab.LeanMoveLocalY(-150, 0.7f).setEaseInExpo();
            StartCoroutine(setGOtoFalse());
        }
        else 
            box.gameObject.SetActive(true);
        buttonImage.Rotate(180, 0, 0);
    }


    private IEnumerator setGOtoFalse()
    {
        yield return new WaitForSecondsRealtime(1);
        box.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    private void OnEnable()
    {
        box.localPosition = new Vector2(0, -Screen.height);
        box.LeanMoveLocalY(-39, 0.5f).setEaseOutExpo().delay = 0.1f;
        saveMasksTab.LeanMoveLocalY(150, 0.35f).setEaseOutExpo();
    }

    public void OnDisable()
    {
        box.LeanMoveLocalY(-400, 0.5f).setEaseInExpo().delay = 0.1f;
    }
}
