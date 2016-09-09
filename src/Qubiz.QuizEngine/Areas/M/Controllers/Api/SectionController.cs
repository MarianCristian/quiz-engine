﻿using Qubiz.QuizEngine.Database.Entities;
using Qubiz.QuizEngine.Infrastructure;
using Qubiz.QuizEngine.Services.SectionService;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Qubiz.QuizEngine.Areas.M.Controllers.Api
{
    [Admin]
    public class SectionController : ApiController
    {
        private readonly ISectionService sectionService;

        public SectionController(ISectionService sectionService)
        {
            this.sectionService = sectionService;
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            Services.Contract.Section[] dbSections = await sectionService.GetAllSectionsAsync();
            QuizEngine.Controllers.Contract.Section[] sections = dbSections.DeepCopyTo<QuizEngine.Controllers.Contract.Section[]>();
            return Ok(sections);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            Services.Contract.Section dbSection = await sectionService.GetSectionAsync(id);
            QuizEngine.Controllers.Contract.Section section = dbSection.DeepCopyTo<QuizEngine.Controllers.Contract.Section>();

            return Ok(section);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            ValidationError[] validationErrors = await sectionService.DeleteSectionAsync(id);
            if (validationErrors.Any())
                return BadRequest();

            return Ok();
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post(QuizEngine.Controllers.Contract.Section section)
        {
            ValidationError[] validationErrors = await sectionService.AddSectionAsync(section.DeepCopyTo<Services.Contract.Section>());
            if (validationErrors.Any())
                return BadRequest();

            return Ok();
        }

        [HttpPut]
        public async Task<IHttpActionResult> Put(QuizEngine.Controllers.Contract.Section section)
        {
            ValidationError[] validationErrors = await sectionService.UpdateSectionAsync(section.DeepCopyTo<Services.Contract.Section>());
            if (validationErrors.Any())
                return BadRequest();

            return Ok();
        }
    }
}