using NUnit.Framework;
using UnityEngine;
using TaekwondoTech.Player;

namespace TaekwondoTech.Tests.EditMode
{
    /// <summary>
    /// EditMode unit tests for PlayerHealth covering damage, healing, death events,
    /// and invincibility (when health is zero).
    /// </summary>
    [TestFixture]
    public class PlayerHealthTests
    {
        private GameObject _go;
        private PlayerHealth _health;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("TestPlayer");
            _health = _go.AddComponent<PlayerHealth>();
            // Awake() runs on AddComponent, setting _currentHealth = _maxHealth (default 3)
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_go);
        }

        [Test]
        public void TakeDamage_ReducesCurrentHealth()
        {
            _health.TakeDamage(1);

            Assert.AreEqual(2, _health.CurrentHealth);
        }

        [Test]
        public void TakeDamage_ClampsToZero_WhenDamageExceedsHealth()
        {
            _health.TakeDamage(10);

            Assert.AreEqual(0, _health.CurrentHealth);
        }

        [Test]
        public void TakeDamage_FiresOnPlayerDeath_WhenHealthReachesZero()
        {
            bool deathFired = false;
            _health.OnPlayerDeath.AddListener(() => deathFired = true);

            _health.TakeDamage(3);

            Assert.IsTrue(deathFired, "OnPlayerDeath should fire when health reaches zero");
        }

        [Test]
        public void Heal_IncreasesCurrentHealth()
        {
            _health.TakeDamage(2); // health = 1
            _health.Heal(1);       // health = 2

            Assert.AreEqual(2, _health.CurrentHealth);
        }

        [Test]
        public void Heal_ClampsToMaxHealth_WhenHealExceedsMax()
        {
            _health.TakeDamage(1); // health = 2
            _health.Heal(100);     // should clamp to maxHealth = 3

            Assert.AreEqual(_health.MaxHealth, _health.CurrentHealth);
        }

        [Test]
        public void IsAlive_ReturnsTrue_WhenHealthAboveZero()
        {
            Assert.IsTrue(_health.IsAlive);
        }

        [Test]
        public void IsAlive_ReturnsFalse_WhenHealthIsZero()
        {
            _health.TakeDamage(3);

            Assert.IsFalse(_health.IsAlive);
        }

        [Test]
        public void OnHealthChanged_FiresWithCorrectValue_AfterTakeDamage()
        {
            int reportedHealth = -1;
            _health.OnHealthChanged.AddListener(h => reportedHealth = h);

            _health.TakeDamage(1);

            Assert.AreEqual(2, reportedHealth, "OnHealthChanged should report health after damage");
        }

        [Test]
        public void TakeDamage_IsIgnored_WhenAlreadyDead()
        {
            _health.TakeDamage(3); // kill player
            int deathCount = 0;
            _health.OnPlayerDeath.AddListener(() => deathCount++);

            _health.TakeDamage(1); // should be ignored since IsAlive is false

            Assert.AreEqual(0, _health.CurrentHealth);
            Assert.AreEqual(0, deathCount, "OnPlayerDeath should not fire again for a dead player");
        }

        [Test]
        public void Heal_IsIgnored_WhenDead()
        {
            _health.TakeDamage(3); // kill player
            _health.Heal(3);       // should be ignored since IsAlive is false

            Assert.AreEqual(0, _health.CurrentHealth, "Heal should have no effect on a dead player");
        }
    }
}
