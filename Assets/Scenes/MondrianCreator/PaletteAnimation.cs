using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteAnimation : MonoBehaviour
{
    public Transform background;
    public Transform title;
    public Transform buttonImage;
    public void buttonPress()
    {
        if (background.gameObject.activeSelf)
        {
            background.LeanMoveLocalY(-400, 0.5f).setEaseInExpo();
            title.LeanMoveLocalY(-150, 0.7f).setEaseInExpo();
            StartCoroutine(setGOtoFalse());
        }
        else
            background.gameObject.SetActive(true);
        buttonImage.Rotate(180, 0, 0);
    }


    private IEnumerator setGOtoFalse()
    {
        yield return new WaitForSecondsRealtime(1);
        background.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    private void OnEnable()
    {
        background.localPosition = new Vector2(0, -Screen.height);
        background.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.1f;
        title.LeanMoveLocalY(150, 0.35f).setEaseOutExpo();
    }

    public void OnDisable()
    {
        background.LeanMoveLocalY(-400, 0.5f).setEaseInExpo().delay = 0.1f;
    }
}
