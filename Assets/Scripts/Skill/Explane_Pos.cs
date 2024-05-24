using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Explane_Pos : MonoBehaviour
{
    public static Explane_Pos Instance { get; private set; }

    public static GameObject skill_explane;
    private static Vector3 mousePos_pc;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void SetMousePos(Vector3 pos)
    {
        //Debug.Log($"SetMousePos : {pos}");
        mousePos_pc = pos;
    }
 
    //pos : mouse position
    public static void SetSkillExplanePos_SetMousePos()
    {
        if (skill_explane != null)
        {
            skill_explane.transform.position = new Vector3(mousePos_pc.x * 100 + 950, mousePos_pc.y * 50 + 440, mousePos_pc.z);
            skill_explane.GetComponentInChildren<Text>().text = CharSkill.GetSkillDesc();
        }
    }
}
