using AutoMapper;
using SM.DataAccess;
using SM.Dto;

namespace SM.DTOs.Maps
{
    internal static class AutomapperConfig
    {
        private static MapperConfiguration _config;
        public static MapperConfiguration GetConfig()
        {
            return _config ?? (_config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Country, CountryDto>();
                cfg.CreateMap<CountryLocaleCode, CountryLocaleCodeDto>();
                cfg.CreateMap<Department, DepartmentDto>();
                cfg.CreateMap<ScenarioRoleDescription, ScenarioRoleDescriptionDto>();
                cfg.CreateMap<Institution, InstitutionDto>();
                cfg.CreateMap<Manequin, ManequinDto>();
                cfg.CreateMap<ManequinManufacturer, ManequinManufacturerDto>();
                cfg.CreateMap<Participant, ParticipantDto>();
                cfg.CreateMap<ProfessionalRole, ProfessionalRoleDto>();
                cfg.CreateMap<Scenario, ScenarioDto>();
                cfg.CreateMap<ScenarioResource, ScenarioResourceDto>();
                cfg.CreateMap<Course, CourseDto>();
                cfg.CreateMap<CourseParticipant, CourseParticipantDto>();
                cfg.CreateMap<CourseType, CourseTypeDto>();
                cfg.CreateMap<CourseSlot, CourseSlotDto>();
                cfg.CreateMap<ScenarioSlot, ScenarioSlotDto>();
                cfg.CreateMap<CourseTeachingResource, CourseTeachingResourceDto>();
                cfg.CreateMap<CourseSlotPresenter, CourseSlotPresenterDto>();
                cfg.CreateMap<ScenarioFacultyRole, ScenarioFacultyRoleDto>();
            }));

            
        }

    }
}
