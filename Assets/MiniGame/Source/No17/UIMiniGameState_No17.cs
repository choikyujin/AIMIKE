//using UnityEngine;
//using System.Collections.Generic;
//using System.Collections;
//using System;

//public class UIMiniGameState_No17 : UIMiniGameState
//{
//    private No17Window createObj = null;
//    private const int SucceedCount = 10;
//    private const float MoveSpeed = 0.25f;
//    private List<CharacterType> clearList = new List<CharacterType>();
//    private List<CharacterType> characterList = new List<CharacterType>();
//    private Animator animator = null;

//    public enum CharacterType
//    {
//        None,
//        Blue,
//        Brown,
//        Green
//    }

//    public override void OnEnter()
//    {
//        base.OnEnter();
//        CreateObj();
//    }

//    private void CreateObj()
//    {
//        DestroyObj();
//        if (createObj == null)
//        {
//            if (ResourcesManager.LoadAndInstantiate("MiniGame", "No17/No17.prefab", ref createObj))
//                GameUtility.AddChild(createObj, owner.createObj.contentsA);
//        }
//        if (createObj != null)
//            InitMiniGame();
//        else
//            Remove();
//    }

//    private void InitMiniGame()
//    {
//        SetAppear(createObj.startTw);
//        createObj.StartCoroutine(StartCreateCharacter());
//    }

//    private IEnumerator StartCreateCharacter()
//    {
//        while (clearList.Count < SucceedCount)
//        {
//            yield return new WaitForSeconds(MoveSpeed);
//            if (characterList.Count < SucceedCount)
//                characterList.Add(GetRandomCharacterType());
//            if (animator != null || characterList.Count >= SucceedCount)
//                continue;
//            SetNextCharacterList();
//        }
//    }

//    private CharacterType GetRandomCharacterType()
//    {
//        return (CharacterType)UnityEngine.Random.Range((int)CharacterType.Blue, (int)CharacterType.Green + 1);
//    }

//    private void SetNextCharacterList()
//    {
//        var list = createObj.characterList;
//        if (list == null)
//            return;

//        for (int i = list.Length - 1, j = i; i >= 0; --i, --j)
//        {
//            var character = list[i];
//            if (character == null)
//                continue;

//            var characterType = CharacterType.None;
//            j = GetRecursiveCharacterType(j, ref characterType);
//            if (characterType != CharacterType.None)
//                character.SetImage(GetCharacterSpriteName(characterType, false, false));
//            else
//                character.img.enabled = false;
//        }
//    }

//    private void SetFirstCharacter(bool isSucceed)
//    {
//        var character = GetCharacter(0);
//        if (character != null)
//        {
//            animator = character.effectAni;
//            if (!isSucceed)
//            {
//                SFXManager.Instance.PlaySFX(116);
//                character.SetImage(GetCharacterSpriteName(GetCharacterType(0), true, false));
//                UIStateManager.Instance.AddSubState(typeof(UISubState_AnimationPlayer), animator, "Failed",
//                    new Action(() =>
//                    {
//                        CompletedAni();
//                    }));
//            }
//            else
//            {
//                SFXManager.Instance.PlaySFX(115);
//                character.SetImage(GetCharacterSpriteName(GetCharacterType(0), false, true));
//                UIStateManager.Instance.AddSubState(typeof(UISubState_AnimationPlayer), animator, "Succeed",
//                    new Action(() =>
//                    {
//                        CompletedAni();
//                    }));
//            }
//        }
//    }

//    private No17Character GetCharacter(int idx)
//    {
//        var list = createObj.characterList;
//        if (list != null && list.Length > idx)
//        {
//            if (list[idx].img.enabled)
//                return list[idx];
//            else
//                return GetCharacter(idx + 1);
//        }
//        return null;
//    }

//    private string GetCharacterSpriteName(CharacterType type, bool failed, bool succeed)
//    {
//        string name = "char-blue";
//        switch (type)
//        {
//            case CharacterType.Blue:
//                name = "char-blue";
//                break;
//            case CharacterType.Brown:
//                name = "char-brown";
//                break;
//            case CharacterType.Green:
//                name = "char-green";
//                break;
//        }
//        if (failed)
//            name += "-false";
//        if (succeed)
//            name += "-true";
//        return name;
//    }

//    private int GetRecursiveCharacterType(int idx, ref CharacterType characterType)
//    {
//        if (characterList.Count > idx && idx >= 0)
//        {
//            characterType = characterList[idx];
//            return idx;
//        }
//        if (idx < 0)
//        {
//            characterType = CharacterType.None;
//            return idx;
//        }
//        return GetRecursiveCharacterType(idx - 1, ref characterType);
//    }

//    private CharacterType GetCharacterType(int idx)
//    {
//        if (characterList.Count > idx)
//            return characterList[idx];
//        return CharacterType.None;
//    }

//    private void DestroyObj()
//    {
//        if (createObj != null)
//        {
//            createObj.StopAllCoroutines();
//            GameObject.Destroy(createObj.gameObject);
//        }
//        createObj = null;
//    }

//    public override void OnExit()
//    {
//        base.OnExit();
//        DestroyObj();
//        clearList.Clear();
//        characterList.Clear();
//        animator = null;
//    }

//    public override void OnEvent(EventMessage msg)
//    {
//        base.OnEvent(msg);
//        switch (msg.message)
//        {
//            case "ClickColorButton":
//                OnClickColorButton(msg as EventMessageEx);
//                break;
//        }
//    }

//    private void OnClickColorButton(EventMessageEx msg)
//    {
//        if (clearList.Count >= SucceedCount || animator != null)
//            return;

//        var item = UIGameUtility.GetItemFromButton<No17ColorButton>(msg, 0);
//        if (item != null && animator == null)
//        {
//            if (GetCharacterType(0) == item.btnType)
//            {
//                clearList.Add(item.btnType);
//                SetFirstCharacter(true);
//            }
//            else
//            {
//                SetFirstCharacter(false);
//            }
//        }
//    }

//    private void CompletedAni()
//    {
//        if (clearList.Count < SucceedCount)
//        {
//            animator = null;
//            if (characterList.Count > 0)
//            {
//                characterList.RemoveAt(0);
//                SetNextCharacterList();
//            }
//        }
//        if (clearList.Count == SucceedCount)
//            owner.ClearMiniGame();
//    }
//}
