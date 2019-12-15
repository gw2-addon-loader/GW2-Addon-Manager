using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.IO.Abstractions.TestingHelpers;

namespace GW2_Addon_Manager.Tests
{
    [TestClass()]
    public class ConfigurationTests
    {
        [TestMethod()]
        public void SetGamePathTest_SetsConfigFile()
        {
            //arrange
            string testPath = "C:\\Program Files\\Test\\Guild Wars 2";
            var mockFileSystem = new MockFileSystem();
            var mockInputFile = new MockFileData("game_path : C:\\Program Files\\Failed\\Guild Wars 2");
            Configuration.AttachMockFileSystem(mockFileSystem);
            mockFileSystem.AddFile($"{Directory.GetCurrentDirectory()}\\config.yaml", mockInputFile);

            //act
            Configuration.SetGamePath(testPath);

            //assert
            Assert.AreEqual(testPath, Configuration.getConfigAsYAML().game_path);
        }



    }
}