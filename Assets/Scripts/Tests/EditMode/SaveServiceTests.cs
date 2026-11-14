using System.IO;
using NUnit.Framework;
using UnityTemplate.Core;
using UnityTemplate.Economy;

namespace UnityTemplate.Tests
{
    /// <summary>Tests that progress survives a save/load round trip, a corrupt file and a reset.</summary>
    public class SaveServiceTests
    {
        private TestSaveFolder folder;

        [SetUp]
        public void CreateFolder()
        {
            folder = new TestSaveFolder();
        }

        [TearDown]
        public void DeleteFolder()
        {
            folder.Dispose();
        }

        [Test]
        public void SavedBalancesAreRestoredByANewSession()
        {
            var firstSession = folder.CreateSaveService();
            var credits = new CreditsService(firstSession);
            firstSession.Register(credits);
            firstSession.Load();
            credits.Add(Currency.Credits, 120);
            credits.Add(Currency.PremiumCredits, 7);

            var secondSession = folder.CreateSaveService();
            var restored = new CreditsService(secondSession);
            secondSession.Register(restored);
            secondSession.Load();

            Assert.AreEqual(120, restored.GetBalance(Currency.Credits));
            Assert.AreEqual(7, restored.GetBalance(Currency.PremiumCredits));
        }

        [Test]
        public void SaveBeforeLoadDoesNotOverwriteTheExistingFile()
        {
            var firstSession = folder.CreateSaveService();
            var credits = new CreditsService(firstSession);
            firstSession.Register(credits);
            firstSession.Load();
            credits.Add(Currency.Credits, 50);

            var notLoadedYet = folder.CreateSaveService();
            notLoadedYet.Register(new CreditsService(notLoadedYet));
            notLoadedYet.Save();

            var check = folder.CreateSaveService();
            var restored = new CreditsService(check);
            check.Register(restored);
            check.Load();
            Assert.AreEqual(50, restored.GetBalance(Currency.Credits));
        }

        [Test]
        public void ACorruptSaveFileFallsBackToTheBackup()
        {
            var session = folder.CreateSaveService();
            var credits = new CreditsService(session);
            session.Register(credits);
            session.Load();
            credits.Add(Currency.Credits, 10);
            credits.Add(Currency.Credits, 5);
            File.WriteAllText(Path.Combine(folder.Path, "save.json"), "{ not json");

            var recovered = folder.CreateSaveService();
            var restored = new CreditsService(recovered);
            recovered.Register(restored);
            recovered.Load();

            Assert.AreEqual(10, restored.GetBalance(Currency.Credits));
        }

        [Test]
        public void DeleteAllResetsServicesAndRaisesDataLoaded()
        {
            var session = folder.CreateSaveService();
            var credits = new CreditsService(session);
            session.Register(credits);
            session.Load();
            credits.Add(Currency.Credits, 99);
            var reloaded = false;
            session.DataLoaded += () => reloaded = true;

            session.DeleteAll();

            Assert.AreEqual(0, credits.GetBalance(Currency.Credits));
            Assert.IsTrue(reloaded);
            Assert.IsFalse(File.Exists(Path.Combine(folder.Path, "save.json")));
        }
    }
}
