﻿using Qubiz.QuizEngine.Database.Entities;
using Qubiz.QuizEngine.Database.Repositories;
using Qubiz.QuizEngine.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Qubiz.QuizEngine.Services.SectionService
{
	public class SectionService : ISectionService
	{
		private readonly IConfig config;

		public SectionService(IConfig config)
		{
			this.config = config;
		}

		public async Task<ValidationError[]> DeleteSectionAsync(Guid id)
		{
			using (IUnitOfWork unitOfWork = new UnitOfWork(config))
			{
				Section section = await unitOfWork.SectionRepository.GetSectionByIDAsync(id);
				if (section == null)
				{
					return new ValidationError[1] { new ValidationError() { Message = "Deletion failed! There is no Section instance with this ID!" } };
				}

				unitOfWork.SectionRepository.Delete(section);

				await unitOfWork.SaveAsync();

				return new ValidationError[0];
			}
		}

		public async Task<Section[]> GetAllSectionsAsync()
		{
			using (IUnitOfWork unitOfWork = new UnitOfWork(config))
			{
				return await unitOfWork.SectionRepository.GetAllSectionsAsync();
			}
		}

		public async Task<ValidationError[]> AddSectionAsync(Section section)
		{
			using (IUnitOfWork unitOfWork = new UnitOfWork(config))
			{
				Section dbSection = await unitOfWork.SectionRepository.GetSectionByNameAsync(section.Name);
				if (dbSection == null)
				{
					unitOfWork.SectionRepository.Create(section);
					await unitOfWork.SaveAsync();
					return new ValidationError[0];
				}

				return new ValidationError[1] { new ValidationError() { Message = "Add failed! There already exists a Section instance with this name!" } };
			}
		}

		public async Task<ValidationError[]> UpdateSectionAsync(Section section)
		{
			using (IUnitOfWork unitOfWork = new UnitOfWork(config))
			{
				Section dbSection = await unitOfWork.SectionRepository.GetSectionByNameAsync(section.Name);
				if (dbSection != null && dbSection.ID != section.ID)
				{
					return new ValidationError[1] { new ValidationError() { Message = "Update failed! There is no Section instance with this ID!" } };
				}
                dbSection = await unitOfWork.SectionRepository.GetSectionByIDAsync(section.ID);

                Mapper.Map(section, dbSection);

				unitOfWork.SectionRepository.Update(dbSection);

				await unitOfWork.SaveAsync();

				return new ValidationError[0];
			}
		}

		public async Task<Section> GetSectionAsync(Guid id)
		{
			using (IUnitOfWork unitOfWork = new UnitOfWork(config))
			{
				return await unitOfWork.SectionRepository.GetSectionByIDAsync(id);
			}
		}
	}
}