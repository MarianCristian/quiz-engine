using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qubiz.QuizEngine.Infrastructure;
using Qubiz.QuizEngine.Database;
using Qubiz.QuizEngine.Database.Repositories;
using System.Linq;
using System.Threading.Tasks;
using Qubiz.QuizEngine.Database.Repositories.Contract;

namespace Qubiz.QuizEngine.UnitTesting.Database
{
    [TestClass]
    public class SectionRepositoryTest
    {
        private QuizEngineDataContext dbContext;
        private SectionRepository sectionRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            IConfig config = new Config();
            dbContext = new QuizEngineDataContext(config.ConnectionString);
            sectionRepository = new SectionRepository(dbContext, null);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            dbContext.Database.ExecuteSqlCommand("DELETE FROM [dbo].[Sections]");
        }

        [TestMethod]
        public async Task ListAsync_WhenSectionsExists_ThenReturnArrayOfSections()
        {
            Section section1 = sectionProvider();
            Section section2 = sectionProvider();

            sectionRepository.Upsert(section1);
            sectionRepository.Upsert(section2);

            Section[] dbSections = await sectionRepository.ListAsync();

            AssertSectionEqual(section1, dbSections.First(section => section.ID == section1.ID));
            AssertSectionEqual(section1, dbSections.First(section => section.ID == section1.ID));
        }

        [TestMethod]
        public async Task ListAsync_WhenSectionsDontExists_ThenReturnEmptyList()
        {
            Section[] sections = await sectionRepository.ListAsync();

            Assert.IsTrue(sections.Count() == 0);
        }

        [TestMethod]
        public async Task GetByIDAsync_WhenSectionExists_ThenReturnSectionByID()
        {
            Section section = sectionProvider();

            sectionRepository.Upsert(section);

            Section dbSection = await sectionRepository.GetByIDAsync(section.ID);

            AssertSectionEqual(section, dbSection);
        }

        [TestMethod]
        public async Task GetByIDAsync_WhenSectionDontExists_ThenReturnsNull()
        {
            Guid id = Guid.NewGuid();

            Section section = await sectionRepository.GetByIDAsync(id);

            Assert.IsNull(section);
        }

        [TestMethod]
        public async Task GetByNameAsync_WhenSectionExists_ThenReturnSectionByName()
        {
            Section section = sectionProvider();

            sectionRepository.Upsert(section);

            Section dbSection = await sectionRepository.GetByNameAsync(section.Name);

            AssertSectionEqual(section, dbSection);
        }

        [TestMethod]
        public async Task GetByNameAsync_WhenSectionDontExist_ThenReturnsNull()
        {
            Section section = await sectionRepository.GetByNameAsync("UnexistingSection");

            Assert.IsNull(section);
        }

        [TestMethod]
        public void Create_WhenSectionDontExist_ThenAddCurrentSection()
        {
            Section section = sectionProvider();
            sectionRepository.Create(section);

            Section dbSection = sectionRepository.GetByIDAsync(section.ID).Result;

            AssertSectionEqual(section, dbSection);
        }

        [TestMethod]
        public void Create_WhenSectionExists_ThenThrowsException()
        {
            Section dbSection = sectionProvider();
            Section section = dbSection;
            sectionRepository.Create(dbSection);
            try
            {
                sectionRepository.Create(section);
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
            }
        }

        [TestMethod]
        public void Create_WhenSectionIsNull_ThenThrowsException()
        {
            Section section = null;
            try
            {
                sectionRepository.Create(section);
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
            }
        }
        [TestMethod]
        public void Update_WhenSectionExists_ThenUpdateCurrentSection()
        {
            Section section = sectionProvider();
            sectionRepository.Create(section);
            var changedName = "New Name";
            section.Name = changedName;
            sectionRepository.Update(section);

            Section updatedSection = sectionRepository.GetByIDAsync(section.ID).Result;

            AssertSectionEqual(section, updatedSection);
        }

        [TestMethod]
        public void Update_WhenSectionDontExists_ThenThrowException()
        {
            Section section = sectionProvider();
            var changedName = "New Name";
            section.Name = changedName;
            try
            {
                sectionRepository.Update(section);
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
            }
        }

        [TestMethod]
        public void Delete_WhenSectionExists_DeleteSection()
        {
            Section section = sectionProvider();

            sectionRepository.Create(section);
            sectionRepository.Delete(section);

            Section dbSection = sectionRepository.GetByIDAsync(section.ID).Result;

            Assert.IsNull(dbSection);
        }

        [TestMethod]
        public void Delete_WhenSectionDontExists_ThrowException()
        {
            Section section = sectionProvider();
            try
            {
                sectionRepository.Delete(section);
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
            }
        }

        [TestMethod]
        public void Upsert_WhenAddingSectionThatDontExists_AddSection()
        {
            Section section = sectionProvider();
            sectionRepository.Upsert(section);

            Section dbSection = sectionRepository.GetByIDAsync(section.ID).Result;

            AssertSectionEqual(section, dbSection);
        }
        [TestMethod]
        public void Upsert_WhenAddingExistingSection_ThenUpdateExistingObject()
        {
            Section section = sectionProvider();
            sectionRepository.Upsert(section);

            Section newSection = new Section{ ID = section.ID, Name = "New Name"};
            sectionRepository.Upsert(newSection);

            section = sectionRepository.GetByIDAsync(section.ID).Result;

            AssertSectionEqual(section, newSection);
        }

        private Section sectionProvider()
        {
            return new Section { ID = Guid.NewGuid(), Name = "Test Name" };
        }

        private void AssertSectionEqual(Section expected, Section actual)
        {
            Assert.AreEqual(expected.ID, actual.ID);
            Assert.AreEqual(expected.Name, actual.Name);
        }


    }
}
