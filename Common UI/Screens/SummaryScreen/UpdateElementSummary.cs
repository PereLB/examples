using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateElementSummary : MonoBehaviour
{
    [Header("Possible Reward type")]
    [SerializeField] private List<RewardSO> typeRewards;
    [Header("Element components")]
    [SerializeField] private TextMeshProUGUI rewardName;
    [SerializeField] private TextMeshProUGUI rewardValue;
    [SerializeField] private Image rewardImage;
    [SerializeField] private Transform elementEndPosition;
    [SerializeField] private float anchorsYOffset;
    [Header("Listener")]
    [SerializeField] private VoidEventChannelSO skipEvent;

    private ElementAppearBehaviourSOBase appearBehaviourInstance;
    private Coroutine updateElement_CO;
    private Vector3 targetPosition;
    private int targetValue;

    public Coroutine AddElement(int m_rewardID, int m_value)
    {
        Setup(m_rewardID, m_value);
        updateElement_CO = StartCoroutine(UpdateElementAnimation(m_value));
        return updateElement_CO;
    }

    private void Setup(int m_rewardID, int m_value)
    {
        foreach (var reward in typeRewards)
        {
            if(reward.id == m_rewardID)
            {
                if (reward.rewardImage)
                {
                    rewardImage.enabled = true;
                    rewardImage.sprite = reward.rewardImage;
                }
                else
                {
                    rewardImage.enabled = false;
                    rewardImage.sprite = null;
                }

                rewardValue.enabled = true;
                rewardValue.text ="+ "+ m_value.ToString();
                targetValue = m_value;

                if (reward.behaviour)
                {
                    appearBehaviourInstance = null;
                    appearBehaviourInstance = Instantiate(reward.behaviour);
                    appearBehaviourInstance.Initialize(this.gameObject);
                }
                else
                {
                    appearBehaviourInstance = null;
                }
                return;
            }
        }

        rewardName.enabled = false;
        rewardImage.enabled = false;
        rewardValue.enabled = false;
        appearBehaviourInstance = null;
    }

    private IEnumerator UpdateElementAnimation(int m_value)
    {
        if (appearBehaviourInstance)
        {
            targetPosition = new Vector3(Screen.width / 2.0f, elementEndPosition.position.y - anchorsYOffset, 0);
            yield return new WaitForSeconds(appearBehaviourInstance.StartAppearAnimation(targetPosition, m_value));
        }
        else
        {
            Debug.LogWarning("An element doesn't have an appear behaviour assigned, resorting to default 4s wait");
            yield return new WaitForSeconds(4.0f);
        }
    }

    public void StopElementAnimation()
    {
        UnsuscribeToSkipChannel();
        appearBehaviourInstance.StopAppearAnimation(targetPosition, targetValue);
        if(updateElement_CO!=null)
            StopCoroutine(updateElement_CO);
        SceneManager.Instance.LoadHub();
    }

    private void UnsuscribeToSkipChannel()
    {
        if (skipEvent)
            skipEvent.OnEventRaised -= StopElementAnimation;
    }
}
