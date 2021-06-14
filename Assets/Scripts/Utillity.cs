using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

[System.Serializable]
public static class Utillity
{    
    //============================================================================================
    // CharngeParts()  
    // - 파츠변경
    //============================================================================================
    public static void CharngeParts(GameObject objParts, Transform tBoneRoot, Transform tRoot, string sParts, bool bHide = false)
    {
        ArrayList pArrayList = new ArrayList();
        GameObject obj = null;
        for (int i = 0; i < tRoot.childCount; i++)
        {
            obj = tRoot.transform.GetChild(i).gameObject;
            if (obj.name == sParts)
                pArrayList.Add(obj);
        }

        for (int i = 0; i < pArrayList.Count; i++)
        {
            obj = (GameObject)pArrayList[i];
            GameObject.Destroy(obj);
            obj = null;
        }

        SkinnedMeshRenderer[] BonedObjects = objParts.GetComponents<SkinnedMeshRenderer>();

        if (BonedObjects.Length == 0)
            BonedObjects = objParts.GetComponentsInChildren<SkinnedMeshRenderer>();

        if (BonedObjects.Length > 0)
        {
            foreach (SkinnedMeshRenderer smr in BonedObjects)
                ProcessBonedObject(smr, tBoneRoot, tRoot, sParts, objParts);

            GameObject.Destroy(objParts);
            objParts = null;
        }
        else
        {
            Debug.Log("파츠에러-SkinnedMeshRenderer : " + sParts);
            GameObject.Destroy(objParts);
            objParts = null;
        }
    }


    //============================================================================================
    // ProcessBonedObject()  
    // - 본 셋팅
    //============================================================================================
    private static void ProcessBonedObject(SkinnedMeshRenderer ThisRenderer, Transform tBoneRoot, Transform tRoot, string sParts, GameObject objParts)
    {
        GameObject newObject = null;
        SkinnedMeshRenderer NewRenderer = null;
        Transform[] MyBones = new Transform[ThisRenderer.bones.Length];

        Cloth pCloth = objParts.GetComponent<Cloth>();
        if(pCloth != null)
        {
            newObject = GameObject.Instantiate(objParts);

            Vector3 localpos = objParts.transform.localPosition;
            Quaternion localRot = objParts.transform.localRotation;
            newObject.transform.parent = tRoot;
            newObject.transform.localPosition = localpos;
            newObject.transform.localRotation = localRot;
            newObject.transform.localScale = Vector3.one;
            newObject.name = sParts;

            NewRenderer = newObject.GetComponent<SkinnedMeshRenderer>();
            for (int i = 0; i < ThisRenderer.bones.Length; i++)
                MyBones[i] = FindChildByName(ThisRenderer.bones[i].name, tBoneRoot);

            NewRenderer.updateWhenOffscreen = true;
            NewRenderer.bones = MyBones;
            NewRenderer.sharedMesh = ThisRenderer.sharedMesh;
            NewRenderer.materials = ThisRenderer.materials;
            RestoreShader(newObject);
        }
        else
        {
            newObject = new GameObject();
            newObject.transform.parent = tRoot;
            newObject.name = sParts;

            NewRenderer = newObject.AddComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
            for (int i = 0; i < ThisRenderer.bones.Length; i++)
                MyBones[i] = FindChildByName(ThisRenderer.bones[i].name, tBoneRoot);

            NewRenderer.updateWhenOffscreen = true;
            NewRenderer.bones = MyBones;
            NewRenderer.sharedMesh = ThisRenderer.sharedMesh;
            NewRenderer.materials = ThisRenderer.materials;
            RestoreShader(newObject);
        }               
    }

    //============================================================================================
    // FindChildByName
    // - 
    //===========================================================================================
    public static Transform FindChildByName(string ThisName, Transform ThisGObj)
    {
        Transform ReturnObj;

        if (ThisGObj.name == ThisName)
            return ThisGObj.transform;

        foreach (Transform child in ThisGObj)
        {
            ReturnObj = FindChildByName(ThisName, child);

            if (ReturnObj != null)
                return ReturnObj;
        }

        return null;
    }

    //============================================================================================
    //  ResetBone
    // - 본 재설정
    //============================================================================================
    public static void ResetBone(GameObject obj, Transform tBoneRoot)
    {
        SkinnedMeshRenderer ThisRenderer = obj.GetComponent<SkinnedMeshRenderer>();

        if (ThisRenderer == null)
            return;

        Transform[] MyBones = new Transform[ThisRenderer.bones.Length];
        for (int i = 0; i < ThisRenderer.bones.Length; i++)
            MyBones[i] = FindChildByName(ThisRenderer.bones[i].name, tBoneRoot);

        ThisRenderer.bones = MyBones;
    }

    //============================================================================================
    //  RestoreShader
    // - 셰이더 재설정
    //============================================================================================
    public static void RestoreShader(GameObject Obj)
    {
        Renderer[] renderers = Obj.transform.GetComponentsInChildren<Renderer>(true);
        int count = 0;
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].sharedMaterials == null)
                continue;

            for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
            {
                if (renderers[i].sharedMaterials[j] != null)
                {
                    renderers[i].sharedMaterials[j].shader = Shader.Find(renderers[i].sharedMaterials[j].shader.name);
                    count++;
                }
            }
        }
    }

    //============================================================================================
    //  CopyComponent
    // - 컴퍼넌트 복사
    //============================================================================================
    public static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();

        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }

    //============================================================================================
    //  CombineTextures
    // - 알파값이 초기화
    //============================================================================================
    public static Texture2D ResetTexturesAlpha(Texture2D aBaseTexture)
    {
        int width = aBaseTexture.width;
        int height = aBaseTexture.height;
        Color pColor;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                pColor = aBaseTexture.GetPixel(x, y);

                if (x == 0 && y == 0)
                {
                    aBaseTexture.SetPixel(x, y, new Color(pColor.r, pColor.g, pColor.b, 0f));
                }
                else
                {
                    if (pColor.a == 0f)
                        aBaseTexture.SetPixel(x, y, new Color(pColor.r, pColor.g, pColor.b, 1f));
                }
            }
        }

        aBaseTexture.Apply();
        return aBaseTexture;
    }


    //============================================================================================
    //  CombineTexturesAlpha
    // - 알파값이 있는 부분만 텍스쳐를 합친다.
    //============================================================================================
    public static Texture2D CombineTexturesAlpha(Texture2D aBaseTexture, Texture2D aToCopyTexture)
    {
        int width = aBaseTexture.width;
        int height = aBaseTexture.height;

        Color pColor_1;
        Color pColor_2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {                
                pColor_2 = aToCopyTexture.GetPixel(x, y);

                if(pColor_2.a == 0f)
                {
                    pColor_1 = aBaseTexture.GetPixel(x, y);
                    pColor_1.a = 0f;
                    aBaseTexture.SetPixel(x, y, new Color(pColor_1.r, pColor_1.g, pColor_1.b, 0f));
                }
            }
        }

        aBaseTexture.Apply();
        return aBaseTexture;

        /*
        // 파일저장
        byte[] bytes = m_pTexture2D_Result.EncodeToPNG();
        var dirPath = Application.dataPath + "/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "Image" + ".png", bytes);

        // HDRP/Lit Shader 텍스쳐설정
        m_pMaterial.SetTexture("_BaseColorMap", aReturnTexture);
        */
    }
}
