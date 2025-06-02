using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_BlendShape : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer smr;
    [SerializeField] shapekey element;

    private List<string> shapekeyList = new List<string>
                                            {   "Eyes_Annoyed",
                                                "Eyes_Blink",
                                                "Eyes_Cry",
                                                "Eyes_Dead",
                                                "Eyes_Excited",
                                                "Eyes_Happy",
                                                "Eyes_LookDown",
                                                "Eyes_LookIn",
                                                "Eyes_LookOut",
                                                "Eyes_LookUp",
                                                "Eyes_Rabid",
                                                "Eyes_Sad",
                                                "Eyes_Shrink",
                                                "Eyes_Sleep",
                                                "Eyes_Spin",
                                                "Eyes_Squint",
                                                "Eyes_Trauma",
                                                "Sweat_L",
                                                "Sweat_R",
                                                "Teardrop_L",
                                                "Teardrop_R"
                                            };
    public void ChangeShapekey()
    {
        if (smr == null) return;

        // 선택한 Shapekey 이름으로 BlendShape 인덱스 찾기
        string shapeKeyName = shapekeyList[(int)element];
        int index = smr.sharedMesh.GetBlendShapeIndex(shapeKeyName);
        if (index < 0) return;

        // 모든 BlendShape 초기화
        for (int i = 0; i < smr.sharedMesh.blendShapeCount; i++)
        {
            smr.SetBlendShapeWeight(i, 0f);
        }

        // 선택된 BlendShape만 활성화
        smr.SetBlendShapeWeight(index, 100f);
    }

}

enum shapekey
{
    Eyes_Annoyed,
    Eyes_Blink,
    Eyes_Cry,
    Eyes_Dead,
    Eyes_Excited,
    Eyes_Happy,
    Eyes_LookDown,
    Eyes_LookIn,
    Eyes_LookOut,
    Eyes_LookUp,
    Eyes_Rabid,
    Eyes_Sad,
    Eyes_Shrink,
    Eyes_Sleep,
    Eyes_Spin,
    Eyes_Squint,
    Eyes_Trauma,
    Sweat_L,
    Sweat_R,
    Teardrop_L,
    Teardrop_R
}
