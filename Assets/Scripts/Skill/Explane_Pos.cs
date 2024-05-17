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
        Debug.Log($"SetMousePos : {pos}");
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
    
    //pos : mouse position
    public static void SetSkillExplanePos_Under(Vector3 pos)
    {
        float multi = 2f;
        if (skill_explane != null)
        {
            if(pos.x >= 0.55)
            {
                skill_explane.transform.position = new Vector3((Mathf.Pow(pos.x, 3) + 650), pos.y * multi + 540, pos.z);
            }
            else
            {
                skill_explane.transform.position = new Vector3((Mathf.Pow(pos.x, 3) + 1050), pos.y * multi + 540, pos.z);
            }
            Debug.Log($"ex pos : {skill_explane.transform.position}");
            skill_explane.GetComponentInChildren<Text>().text = CharSkill.GetSkillDesc();
        }
    }
    
    //pos : collider's position
    public static void SetSkillExplanePos_Collider(Vector3 pos)
    {
        float multi = 2f;
        if (skill_explane != null)
        {
            skill_explane.transform.position = new Vector3(pos.x * multi + 650, pos.y + 740, pos.z);

            Debug.Log($"ex pos : {skill_explane.transform.position}");
            skill_explane.GetComponentInChildren<Text>().text = CharSkill.GetSkillDesc();
        }
    }
    //pos : collider's position
    public static void SetSkillExplanePos_Collider2(Vector3 pos)
    {
        float multi = 2f;
        if (skill_explane != null)
        {
            skill_explane.transform.position = new Vector3(pos.x * multi + 1050, pos.y + 740, pos.z);

            Debug.Log($"ex pos : {skill_explane.transform.position}");
            skill_explane.GetComponentInChildren<Text>().text = CharSkill.GetSkillDesc();
        }
    }
}
