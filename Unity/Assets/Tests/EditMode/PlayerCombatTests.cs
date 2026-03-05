using NUnit.Framework;
using UnityEngine;
using System.Reflection;
using TaekwondoTech.Player;

namespace TaekwondoTech.Tests.EditMode
{
    public class PlayerCombatTests
    {
        private GameObject _gameObject;
        private PlayerCombat _playerCombat;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject("TestPlayer");
            _playerCombat = _gameObject.AddComponent<PlayerCombat>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void CanAttack_BeforeAnyAttack_IsTrue()
        {
            Assert.IsTrue(_playerCombat.CanAttack);
        }

        [Test]
        public void IsAttacking_BeforeAnyAttack_IsFalse()
        {
            Assert.IsFalse(_playerCombat.IsAttacking);
        }

        [Test]
        public void AttackType_BeforeAnyAttack_IsZero()
        {
            Assert.AreEqual(0, _playerCombat.AttackType);
        }

        [Test]
        public void AfterPunch_CanAttack_IsFalse()
        {
            _playerCombat.PerformPunch();

            Assert.IsFalse(_playerCombat.CanAttack);
        }

        [Test]
        public void AfterPunch_AttackType_IsPunch()
        {
            _playerCombat.PerformPunch();

            Assert.AreEqual(1, _playerCombat.AttackType);
        }

        [Test]
        public void AfterKick_AttackType_IsKick()
        {
            _playerCombat.PerformKick();

            Assert.AreEqual(2, _playerCombat.AttackType);
        }

        [Test]
        public void AfterCooldownExpires_CanAttack_ReturnsTrue()
        {
            _playerCombat.PerformPunch();
            Assert.IsFalse(_playerCombat.CanAttack, "CanAttack should be false during cooldown");

            // Simulate cooldown expiry — real timer behaviour is tested in PlayMode
            typeof(PlayerCombat)
                .GetField("_canPunch", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_playerCombat, true);

            Assert.IsTrue(_playerCombat.CanAttack, "CanAttack should be true after cooldown expires");
        }
    }
}
