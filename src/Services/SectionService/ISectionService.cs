﻿using Qubiz.QuizEngine.Database.Entities;
using Qubiz.QuizEngine.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Qubiz.QuizEngine.Services.SectionService
{
	public interface ISectionService
	{
		Task<Section[]> GetAllSectionsAsync();

		Task<ValidationError[]> DeleteSectionAsync(Guid id);
		
		Task<ValidationError[]> AddSectionAsync(Section newSection);

		Task<ValidationError[]>	UpdateSectionAsync(Section newSection);

		Task<Section> GetSectionAsync(Guid id);
	}
}