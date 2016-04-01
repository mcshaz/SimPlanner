using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SM.DataAccess
{
    [MetadataType(typeof(CourseSlotMetadata))]
    public class CourseSlot
    {
        public Guid Id { get; set; }
        public byte MinutesDuration { get; set; }
        public byte Order { get; set; }
        public byte Day { get; set; }
        public Guid? ActivityId { get; set; }
        public Guid CourseFormatId { get; set; }
        public virtual CourseActivity Activity { get; set; }
        public virtual CourseFormat CourseFormat { get; set; }

        ICollection<ChosenTeachingResource> _chosenTeachingResources;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChosenTeachingResource> ChosenTeachingResources
        {
            get
            {
                return _chosenTeachingResources ?? (_chosenTeachingResources = new List<ChosenTeachingResource>());
            }
            set
            {
                _chosenTeachingResources = value;
            }
        }

        ICollection<CourseSlotPresenter> _courseSlotPresenters;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlotPresenter> CourseSlotPresenters
        {
            get
            {
                return _courseSlotPresenters ?? (_courseSlotPresenters = new List<CourseSlotPresenter>());
            }
            set
            {
                _courseSlotPresenters = value;
            }
        }

        ICollection<CourseScenarioFacultyRole> _courseScenarioFacultyRoles;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseScenarioFacultyRole> CourseScenarioFacultyRoles
        {
            get
            {
                return _courseScenarioFacultyRoles ?? (_courseScenarioFacultyRoles = new List<CourseScenarioFacultyRole>());
            }
            set
            {
                _courseScenarioFacultyRoles = value;
            }
        }

        ICollection<CourseSlotScenario> _courseSlotScenarios;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlotScenario> CourseSlotScenarios
        {
            get
            {
                return _courseSlotScenarios ?? (_courseSlotScenarios = new List<CourseSlotScenario>());
            }
            set
            {
                _courseSlotScenarios = value;
            }
        }
    }
}
