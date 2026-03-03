using NUnit.Framework;
using TaekwondoTech.Core;
using UnityEngine;

namespace TaekwondoTech.Tests.EditMode.Core
{
    /// <summary>
    /// EditMode tests for GameManager.
    /// Awake() runs automatically via AddComponent; Start() does not.
    /// </summary>
    public class GameManagerTests
    {
        private GameObject _go;
        private GameManager _gm;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("GameManager");
            _gm = _go.AddComponent<GameManager>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_go != null)
                Object.DestroyImmediate(_go);
        }

        [Test]
        public void Instance_IsSetAfterAwake()
        {
            Assert.IsNotNull(GameManager.Instance);
            Assert.AreEqual(_gm, GameManager.Instance);
        }

        [Test]
        public void LoadScene_NullSceneName_DoesNotThrow()
        {
            // Guard should log an error and return without throwing.
            Assert.DoesNotThrow(() => _gm.LoadScene(null));
        }

        [Test]
        public void LoadScene_EmptySceneName_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _gm.LoadScene(string.Empty));
        }

        [Test]
        public void LoadScene_WhitespaceSceneName_DoesNotThrow()
        {
            // Whitespace is not null-or-empty; falls through to build-index check.
            // GetBuildIndexByScenePath returns -1 for unknown scenes, so it logs
            // an error and returns without throwing.
            Assert.DoesNotThrow(() => _gm.LoadScene("   "));
        }

        [Test]
        public void LoadScene_UnregisteredSceneName_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _gm.LoadScene("NonExistentScene"));
        }
    }
}
