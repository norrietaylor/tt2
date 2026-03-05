using NUnit.Framework;
using UnityEngine;
using TaekwondoTech.Core;

namespace TaekwondoTech.Tests.EditMode
{
    public class GameManagerTests
    {
        private GameObject _gameObject;
        private GameManager _gameManager;

        [SetUp]
        public void SetUp()
        {
            GameManager.ResetForTesting();
            _gameObject = new GameObject("TestGameManager");
            _gameManager = _gameObject.AddComponent<GameManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
            GameManager.ResetForTesting();
        }

        [Test]
        public void RobotPartsCollected_InitiallyZero()
        {
            Assert.AreEqual(0, _gameManager.RobotPartsCollected);
        }

        [Test]
        public void IncrementRobotParts_IncreasesCountByOne()
        {
            _gameManager.IncrementRobotParts();

            Assert.AreEqual(1, _gameManager.RobotPartsCollected);
        }

        [Test]
        public void IncrementRobotParts_CalledMultipleTimes_AccumulatesCorrectly()
        {
            _gameManager.IncrementRobotParts();
            _gameManager.IncrementRobotParts();
            _gameManager.IncrementRobotParts();

            Assert.AreEqual(3, _gameManager.RobotPartsCollected);
        }

        [Test]
        public void Instance_AfterCreation_IsNotNull()
        {
            Assert.IsNotNull(GameManager.Instance);
        }
    }
}
