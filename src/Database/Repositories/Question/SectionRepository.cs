﻿using Qubiz.QuizEngine.Database.Entities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
namespace Qubiz.QuizEngine.Database.Repositories
{
	public class SectionRepository : BaseRepository<Section>, ISectionRepository
    {
        public SectionRepository(QuizEngineDataContext context, UnitOfWork unitOfWork)
            : base(context, unitOfWork)
        { }

        public async Task<Section[]> List()
        {
			return await dbSet.ToArrayAsync();
        }

		public async Task<Section> GetByName(string name)
		{
			return await dbSet.FirstOrDefaultAsync(s => s.Name == name);
		}

		public async Task<Section> GetByID(Guid id)
		{
			return await dbSet.FirstOrDefaultAsync(s => s.ID == id);
		}

	}
}