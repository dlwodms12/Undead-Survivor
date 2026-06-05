using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public bool isFirstPlay = true;

    // 업적 종류 추가를 대비해 리스트로 관리
    // 0: UnlockPotato, 1: UnlockBean
    public List<bool> achievementUnlocked = new List<bool> { false, false };

}
