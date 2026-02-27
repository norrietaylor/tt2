using UnityEngine;
using TaekwondoTech.Core;

namespace TaekwondoTech.Collectibles
{
    /// <summary>
    /// Robot part collectible - tracked toward "collect all parts" goal.
    /// </summary>
    public class RobotPart : Collectible
    {
        [Header("Robot Part Settings")]
        [SerializeField] private string _partName = "Robot Part";

        protected override void OnCollectedLogic()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.IncrementRobotParts();
#if UNITY_EDITOR
                Debug.Log($"Collected {_partName}! Total parts: {GameManager.Instance.RobotPartsCollected}");
#endif
            }
        }
    }
}
