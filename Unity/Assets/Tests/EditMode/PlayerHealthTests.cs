using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using TaekwondoTech.Player;

namespace TaekwondoTech.Tests.EditMode
{
    public class PlayerHealthTests
    {
        private GameObject _gameObject;
        private PlayerHealth _playerHealth;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject("TestPlayer");
            _playerHealth = _gameObject.AddComponent<PlayerHealth>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void TakeDamage_ReducesHealthByCorrectAmount()
        {
            // MaxHealth is 3; taking 1 damage should leave 2
            _playerHealth.TakeDamage(1);

            Assert.AreEqual(_playerHealth.MaxHealth - 1, _playerHealth.CurrentHealth);
        }

        [Test]
        public void TakeDamage_BelowZero_ClampsToZero()
        {
            _playerHealth.TakeDamage(10);

            Assert.AreEqual(0, _playerHealth.CurrentHealth);
        }

        [Test]
        public void TakeDamage_HealthReachesZero_FiresDeathEvent()
        {
            bool deathEventFired = false;
            _playerHealth.OnPlayerDeath.AddListener(() => deathEventFired = true);

            _playerHealth.TakeDamage(3);

            Assert.IsTrue(deathEventFired);
        }

        [Test]
        public void Heal_IncreasesCurrentHealth()
        {
            // Reduce to 1 HP, then heal 1 to reach 2 HP
            _playerHealth.TakeDamage(_playerHealth.MaxHealth - 1);

            _playerHealth.Heal(1);

            Assert.AreEqual(2, _playerHealth.CurrentHealth);
        }

        [Test]
        public void Heal_AboveMax_ClampsToMaxHealth()
        {
            _playerHealth.TakeDamage(1);

            _playerHealth.Heal(10);

            Assert.AreEqual(_playerHealth.MaxHealth, _playerHealth.CurrentHealth);
        }

        [Test]
        public void IsAlive_HealthAboveZero_ReturnsTrue()
        {
            Assert.IsTrue(_playerHealth.IsAlive);
        }

        [Test]
        public void IsAlive_HealthIsZero_ReturnsFalse()
        {
            _playerHealth.TakeDamage(3);

            Assert.IsFalse(_playerHealth.IsAlive);
        }

        [Test]
        public void TakeDamage_WhenInvincible_IgnoresDamage()
        {
            var field = typeof(PlayerHealth).GetField(
                "_isInvincible",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(field, "Expected private field '_isInvincible' on PlayerHealth.");
            field.SetValue(_playerHealth, true);

            _playerHealth.TakeDamage(1);

            Assert.AreEqual(_playerHealth.MaxHealth, _playerHealth.CurrentHealth);
        }
    }
}
