using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 확률 계산 및 등급 반환
public class DropChanceCalculator : MonoBehaviour
{
    private List<string> tagName;   // 해당 스크립트를 가진 오브젝트의 태그에 따른 확률을 지정하기 위한 태그 리스트
    private float[][] levelInfo;

    // 적 관련
    [SerializeField] private float gradeDropProbability; // 등급별 아이템 드랍할 확률
    public int playerLevel = 1;  //  해당 적을 죽인 플레이어 컨트롤러

    private void Start()
    {
        tagName = new List<string>() { "Enemy", "Store", "HiddenStore" };

        levelInfo = new float[11][];
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log(RandomDropItem() + "당첨!" + " 확률 : " + gradeDropProbability);
        //}
    }

    public void SetLevel(int level)
    {
        this.playerLevel = level;
    }

    // 아이템 등급 반환 (0 : 커먼, 1 : 레어, 2 : 레전더리)
    private ItemType GetGrade(float common, float rare, float legendary)
    {
        if (gradeDropProbability <= common)
        {
            return ItemType.COMMON;
        }
        else if (gradeDropProbability <= common + rare)
        {
            return ItemType.RARE;
        }

        return ItemType.LEGENDARY;
    }

    // 등급별로 계산하여 ItemType 반환
    public ItemType RandomDropItem()
    {
        gradeDropProbability = Random.Range(0f, 100f);

        // 적
        if (this.CompareTag(tagName[0]))
        {
            levelInfo[0] = new float[]  { 99.9f, 0.1f,  0    };
            levelInfo[1] = new float[]  { 95,    5,     0    };
            levelInfo[2] = new float[]  { 92,    7.7f,  0.3f };
            levelInfo[3] = new float[]  { 88,    11.5f, 0.5f };
            levelInfo[4] = new float[]  { 85,    14,    1    };
            levelInfo[5] = new float[]  { 80,    18,    2    };
            levelInfo[6] = new float[]  { 75,    22,    3    };
            levelInfo[7] = new float[]  { 70,    26,    4    };
            levelInfo[8] = new float[]  { 65,    30,    5    };
            levelInfo[9] = new float[]  { 57,    35,    8    };
            levelInfo[10] = new float[] { 50,    40,    10   };

            // 적이 아이템을 드랍할 확률 50퍼
            if (Random.Range(0f, 100f) >= 50.0f)
            {
                return GetGrade(
                    levelInfo[playerLevel][0], levelInfo[playerLevel][1], levelInfo[playerLevel][2]
                    );
            }
            else
            {
                Debug.Log("꽝!");
            }
        }

        // 상점
        else if (this.CompareTag(tagName[1]))
        {
            levelInfo[0] = new float[]  { 100,  0,  0   };
            levelInfo[1] = new float[]  { 95,   5,  0   };
            levelInfo[2] = new float[]  { 90,   9,  1   };
            levelInfo[3] = new float[]  { 85,   13, 2   };
            levelInfo[4] = new float[]  { 77,   20, 3   };
            levelInfo[5] = new float[]  { 70,   25, 5   };
            levelInfo[6] = new float[]  { 63,   30, 7   };
            levelInfo[7] = new float[]  { 57,   33, 10  };
            levelInfo[8] = new float[]  { 52,   35, 13  };
            levelInfo[9] = new float[]  { 38,   45, 17  };
            levelInfo[10] = new float[] { 30,   50, 20  };

            return GetGrade(
                levelInfo[playerLevel][0], levelInfo[playerLevel][1], levelInfo[playerLevel][2]
            );
        }

        // 히든 상점
        else if (this.CompareTag(tagName[2]))
        {
            levelInfo[0] = new float[]  { 100,   0,  0   };
            levelInfo[1] = new float[]  { 90,    10, 0   };
            levelInfo[2] = new float[]  { 85,    14, 1   };
            levelInfo[3] = new float[]  { 81,    16, 3   };
            levelInfo[4] = new float[]  { 76,    19, 5   };
            levelInfo[5] = new float[]  { 70,    23, 7   };
            levelInfo[6] = new float[]  { 60,    30, 10  };
            levelInfo[7] = new float[]  { 55,    33, 12  };
            levelInfo[8] = new float[]  { 50,    35, 15  };
            levelInfo[9] = new float[]  { 35,    45, 20  };
            levelInfo[10] = new float[] { 25,   50,  25  };

            return GetGrade(
                levelInfo[playerLevel][0], levelInfo[playerLevel][1], levelInfo[playerLevel][2]
            );
        }

        return ItemType.COMMON;
    }
}
