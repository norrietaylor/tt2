using UnityEngine;

namespace TaekwondoTech.Enemies.States
{
    /// <summary>
    /// Defeated state when enemy health reaches zero.
    /// Plays defeat animation, disables collider, and destroys GameObject after delay.
    /// </summary>
    public class DefeatedState : IEnemyState
    {
        private readonly EnemyBase _enemy;
        private const float DEATH_DELAY = 1f;

        public DefeatedState(EnemyBase enemy)
        {
            _enemy = enemy;
        }

        public void Enter()
        {
            _enemy.StopMovement();

            // Disable collider
            if (_enemy.Collider != null)
            {
                _enemy.Collider.enabled = false;
            }

            // Play defeat animation
            if (_enemy.Animator != null)
            {
                _enemy.Animator.SetTrigger("Defeated");
            }

            // Hide alert indicator
            _enemy.ShowAlertIndicator(false);

            // Schedule destruction independent of per-frame Execute() calls
            Object.Destroy(_enemy.gameObject, DEATH_DELAY);
        }

        public void Execute()
        {
            // Destruction is scheduled in Enter() via Object.Destroy with delay
        }

        public void Exit()
        {
        }
    }
}
