using Qubiz.QuizEngine.Database.Entities;
using Qubiz.QuizEngine.Infrastructure;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Qubiz.QuizEngine.Database.Repositories
{
    public class SectionRepository : ISectionRepository
    {
        private DbContext dbContext;
        private DbSet<Section> dbSet;

        private UnitOfWork unitOfWork;


        public SectionRepository(QuizEngineDataContext context, UnitOfWork unitOfWork)
        {
            this.dbContext = context;
            this.dbSet = dbContext.Set<Section>();
            this.unitOfWork = unitOfWork;
        }

        public async Task<Contract.Section[]> ListAsync()
        {
            Section[] dbSections = await dbSet.ToArrayAsync();
            Contract.Section[] sections = dbSections.DeepCopyTo<Contract.Section[]>();

            return sections;
        }

        public async Task<Contract.Section> GetByNameAsync(string name)
        {
            Section dbSection = await dbSet.FirstOrDefaultAsync(s => s.Name == name);
            Contract.Section section = dbSection.DeepCopyTo<Contract.Section>();
            return section;
        }

        public async Task<Contract.Section> GetByIDAsync(Guid id)
        {
            Section dbSection = await dbSet.FirstOrDefaultAsync(s => s.ID == id);
            Contract.Section section = dbSection.DeepCopyTo<Contract.Section>();

            return section;
        }

        public void Create(Contract.Section section)
        {
            dbSet.Add(section.DeepCopyTo<Section>());

            dbContext.SaveChanges();

        }

        public void Update(Contract.Section section)
        {
            var dbSection = dbSet.Find(section.ID);
            if (dbSection != null)
            {
                dbSection.Name = section.Name;
                dbSet.Attach(dbSection);
            }
            dbContext.Entry(dbSection).State = EntityState.Modified;
            
            dbContext.SaveChanges();
        }

        public void Delete(Contract.Section section)
        {
            var dbSection = dbSet.Find(section.ID);
            
            if (dbContext.Entry(dbSection).State == EntityState.Detached)
            {
                dbSet.Attach(dbSection);
            }

            dbSet.Remove(dbSection);

            dbContext.SaveChanges();
        }

        public virtual void Upsert(Contract.Section section)
        {
            if (section == null) throw new System.NullReferenceException("Value cannot be null");

            Section existingSection = dbContext.Set<Section>().Find(section.DeepCopyTo<Section>().ID);

            if (existingSection == null)
            {
                dbContext.Set<Section>().Add(section.DeepCopyTo<Section>());
            }
            else
            {
                Mapper.Map(section, existingSection);
            }
            dbContext.SaveChanges();
        }
    }
}