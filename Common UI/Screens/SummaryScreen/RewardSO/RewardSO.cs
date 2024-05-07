using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "RewardType", menuName = "UI/RewardType")]
public class RewardSO : ScriptableObject
{
    public int id;
    public string rewardName;
    public Sprite rewardImage;
    public ElementAppearBehaviourSOBase behaviour;
}
