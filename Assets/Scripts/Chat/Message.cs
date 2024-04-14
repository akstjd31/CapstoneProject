using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Message : MonoBehaviour
{
    public Text NickName;
    public Text myMessage;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
    }

    [PunRPC]
    public void InitSettingRPC(string nickName, string receiveMessage)
    {
        NickName.text = nickName;
        myMessage.text = receiveMessage; //InsertNewlinesBetweenText(receiveMessage);

        //ScaleTextBoxWithTextSize();
    }

    private void ScaleTextBoxWithTextSize()
    {
        RectTransform nickNameRect, messageRect, emptyObjRect, messageBoxRect;

        nickNameRect = NickName.GetComponent<RectTransform>();
        messageRect = myMessage.GetComponent<RectTransform>();
        emptyObjRect = this.GetComponent<RectTransform>();
        messageBoxRect = this.GetComponentInChildren<Image>().GetComponent<RectTransform>();

        messageBoxRect.sizeDelta = new Vector2(messageBoxRect.sizeDelta.x, myMessage.preferredHeight);

        //Vector2 newHeight = messageRect.sizeDelta;
        //newHeight.y += cnt * increaseHeight;

        //messageRect.sizeDelta = newHeight;
        //messageBoxRect.sizeDelta = new Vector2(
        //    messageBoxRect.sizeDelta.x,
        //    newHeight.y);

        emptyObjRect.sizeDelta = new Vector2(
            emptyObjRect.sizeDelta.x,
            nickNameRect.sizeDelta.y + messageBoxRect.sizeDelta.y + 40);

        nickNameRect.position = new Vector2(
            nickNameRect.position.x,
            -messageRect.position.y + messageBoxRect.sizeDelta.y / 2);
    }
}
