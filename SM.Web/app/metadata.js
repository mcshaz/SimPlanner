(function(){	window.medsimMetadata = {
		getBreezeMetadata: getBreezeMetadata,
		getBreezeValidators: getBreezeValidators
	}
	function getBreezeMetadata(){
		return JSON.stringify({"metadataVersion":"1.0.5","namingConvention":"camelCase","localQueryComparisonOptions":"caseInsensitiveSQL","dataServices":[],"structuralTypes":[{"shortName":"ActivityTeachingResourceDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"ActivityTeachingResources","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"courseActivityId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]},{"name":"name","dataType":"String","validators":[{"name":"string"},{"name":"required"},{"name":"maxLength","maxLength":128}]},{"name":"resourceFilename","dataType":"String","validators":[{"name":"string"},{"name":"maxLength","maxLength":256}]}],"navigationProperties":[{"name":"chosenTeachingResources","entityTypeName":"ChosenTeachingResourceDto:#SM.Dto","isScalar":false,"associationName":"ActivityTeachingResourceDto_ChosenTeachingResources","invForeignKeyNames":["activityTeachingResourceId"]},{"name":"courseActivity","entityTypeName":"CourseActivityDto:#SM.Dto","isScalar":true,"associationName":"CourseActivityDto_ActivityChoices","foreignKeyNames":["courseActivityId"]}]},{"shortName":"ChosenTeachingResourceDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"ChosenTeachingResources","dataProperties":[{"name":"courseId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"courseSlotId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"activityTeachingResourceId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]}],"navigationProperties":[{"name":"activityTeachingResource","entityTypeName":"ActivityTeachingResourceDto:#SM.Dto","isScalar":true,"associationName":"ActivityTeachingResourceDto_ChosenTeachingResources","foreignKeyNames":["activityTeachingResourceId"]},{"name":"course","entityTypeName":"CourseDto:#SM.Dto","isScalar":true,"associationName":"CourseDto_ChosenTeachingResources","foreignKeyNames":["courseId"]},{"name":"courseSlot","entityTypeName":"CourseSlotDto:#SM.Dto","isScalar":true,"associationName":"CourseSlotDto_ChosenTeachingResources","foreignKeyNames":["courseSlotId"]}]},{"shortName":"CourseDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"Courses","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"startTime","dataType":"DateTime","isNullable":false,"defaultValue":"","validators":[{"name":"required"},{"name":"date"}]},{"name":"finishTime","dataType":"DateTime","isNullable":false,"defaultValue":"","validators":[{"name":"required"},{"name":"date"}]},{"name":"facultyMeetingTime","dataType":"DateTime","validators":[{"name":"date"}]},{"name":"departmentId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]},{"name":"outreachingDepartmentId","dataType":"Guid","validators":[{"name":"guid"}]},{"name":"roomId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]},{"name":"facultyNoRequired","dataType":"Byte","isNullable":false,"defaultValue":0,"validators":[{"name":"required"},{"min":0,"max":255,"name":"byte"}]},{"name":"courseTypeId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]},{"name":"participantVideoFilename","dataType":"String","maxLength":256,"validators":[{"maxLength":256,"name":"maxLength"}]},{"name":"feedbackSummaryFilename","dataType":"String","maxLength":256,"validators":[{"maxLength":256,"name":"maxLength"}]}],"navigationProperties":[{"name":"chosenTeachingResources","entityTypeName":"ChosenTeachingResourceDto:#SM.Dto","isScalar":false,"associationName":"CourseDto_ChosenTeachingResources","invForeignKeyNames":["courseId"]},{"name":"courseFormat","entityTypeName":"CourseFormatDto:#SM.Dto","isScalar":true,"associationName":"CourseFormatDto_Courses","foreignKeyNames":["courseTypeId"]},{"name":"courseParticipants","entityTypeName":"CourseParticipantDto:#SM.Dto","isScalar":false,"associationName":"CourseDto_CourseParticipants","invForeignKeyNames":["courseId"]},{"name":"courseScenarioFacultyRoles","entityTypeName":"CourseScenarioFacultyRoleDto:#SM.Dto","isScalar":false,"associationName":"CourseDto_CourseScenarioFacultyRoles","invForeignKeyNames":["courseId"]},{"name":"courseSlotPresenters","entityTypeName":"CourseSlotPresenterDto:#SM.Dto","isScalar":false,"associationName":"CourseDto_CourseSlotPresenters","invForeignKeyNames":["courseId"]},{"name":"courseSlotScenarios","entityTypeName":"CourseSlotScenarioDto:#SM.Dto","isScalar":false,"associationName":"CourseDto_CourseSlotScenarios","invForeignKeyNames":["courseId"]},{"name":"department","entityTypeName":"DepartmentDto:#SM.Dto","isScalar":true,"associationName":"DepartmentDto_Courses","foreignKeyNames":["departmentId"]},{"name":"outreachingDepartment","entityTypeName":"DepartmentDto:#SM.Dto","isScalar":true,"associationName":"DepartmentDto_OutreachCourses","foreignKeyNames":["outreachingDepartmentId"]},{"name":"room","entityTypeName":"RoomDto:#SM.Dto","isScalar":true,"associationName":"RoomDto_Courses","foreignKeyNames":["roomId"]}]},{"shortName":"CourseFormatDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"CourseFormats","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"description","dataType":"String","isNullable":false,"defaultValue":"","maxLength":50,"validators":[{"name":"required"},{"maxLength":50,"name":"maxLength"}]},{"name":"daysDuration","dataType":"Byte","isNullable":false,"defaultValue":0,"validators":[{"name":"required"},{"name":"numericRange","min":1,"max":250}]},{"name":"courseTypeId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]}],"navigationProperties":[{"name":"courses","entityTypeName":"CourseDto:#SM.Dto","isScalar":false,"associationName":"CourseFormatDto_Courses","invForeignKeyNames":["courseTypeId"]},{"name":"courseSlots","entityTypeName":"CourseSlotDto:#SM.Dto","isScalar":false,"associationName":"CourseFormatDto_CourseSlots","invForeignKeyNames":["courseFormatId"]},{"name":"courseType","entityTypeName":"CourseTypeDto:#SM.Dto","isScalar":true,"associationName":"CourseTypeDto_CourseFormats","foreignKeyNames":["courseTypeId"]}]},{"shortName":"CourseSlotDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"CourseSlots","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"minutesDuration","dataType":"Byte","isNullable":false,"defaultValue":0,"validators":[{"name":"required"},{"name":"numericRange","min":1,"max":240}]},{"name":"isActive","dataType":"Boolean","isNullable":false,"defaultValue":false,"validators":[{"name":"required"},{"name":"bool"}]},{"name":"order","dataType":"Byte","isNullable":false,"defaultValue":0,"validators":[{"name":"required"},{"name":"numericRange","min":0,"max":100}]},{"name":"day","dataType":"Byte","isNullable":false,"defaultValue":0,"validators":[{"name":"required"},{"name":"numericRange","min":1,"max":28}]},{"name":"activityId","dataType":"Guid","validators":[{"name":"guid"}]},{"name":"courseFormatId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]}],"navigationProperties":[{"name":"activity","entityTypeName":"CourseActivityDto:#SM.Dto","isScalar":true,"associationName":"CourseActivityDto_CourseSlots","foreignKeyNames":["activityId"]},{"name":"chosenTeachingResources","entityTypeName":"ChosenTeachingResourceDto:#SM.Dto","isScalar":false,"associationName":"CourseSlotDto_ChosenTeachingResources","invForeignKeyNames":["courseSlotId"]},{"name":"courseFormat","entityTypeName":"CourseFormatDto:#SM.Dto","isScalar":true,"associationName":"CourseFormatDto_CourseSlots","foreignKeyNames":["courseFormatId"]},{"name":"courseScenarioFacultyRoles","entityTypeName":"CourseScenarioFacultyRoleDto:#SM.Dto","isScalar":false,"associationName":"CourseSlotDto_CourseScenarioFacultyRoles","invForeignKeyNames":["courseSlotId"]},{"name":"courseSlotPresenters","entityTypeName":"CourseSlotPresenterDto:#SM.Dto","isScalar":false,"associationName":"CourseSlotDto_CourseSlotPresenters","invForeignKeyNames":["courseSlotId"]},{"name":"courseSlotScenarios","entityTypeName":"CourseSlotScenarioDto:#SM.Dto","isScalar":false,"associationName":"CourseSlotDto_CourseSlotScenarios","invForeignKeyNames":["courseSlotId"]}]},{"shortName":"CourseActivityDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"CourseActivities","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"name","dataType":"String","isNullable":false,"defaultValue":"","maxLength":128,"validators":[{"name":"required"},{"maxLength":128,"name":"maxLength"}]},{"name":"courseTypeId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]}],"navigationProperties":[{"name":"activityChoices","entityTypeName":"ActivityTeachingResourceDto:#SM.Dto","isScalar":false,"associationName":"CourseActivityDto_ActivityChoices","invForeignKeyNames":["courseActivityId"]},{"name":"courseSlots","entityTypeName":"CourseSlotDto:#SM.Dto","isScalar":false,"associationName":"CourseActivityDto_CourseSlots","invForeignKeyNames":["activityId"]},{"name":"courseType","entityTypeName":"CourseTypeDto:#SM.Dto","isScalar":true,"associationName":"CourseTypeDto_CourseActivities","foreignKeyNames":["courseTypeId"]}]},{"shortName":"CourseTypeDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"CourseTypes","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"description","dataType":"String","isNullable":false,"defaultValue":"","maxLength":50,"validators":[{"name":"required"},{"maxLength":50,"name":"maxLength"}]},{"name":"abbrev","dataType":"String","isNullable":false,"defaultValue":"","maxLength":32,"validators":[{"name":"required"},{"maxLength":32,"name":"maxLength"}]},{"name":"isInstructorCourse","dataType":"Boolean","isNullable":false,"defaultValue":false,"validators":[{"name":"required"},{"name":"bool"}]},{"name":"emersionCategory","dataType":"String","validators":[{"name":"string"}],"enumType":"Edm.Self.Emersion"}],"navigationProperties":[{"name":"courseActivities","entityTypeName":"CourseActivityDto:#SM.Dto","isScalar":false,"associationName":"CourseTypeDto_CourseActivities","invForeignKeyNames":["courseTypeId"]},{"name":"courseFormats","entityTypeName":"CourseFormatDto:#SM.Dto","isScalar":false,"associationName":"CourseTypeDto_CourseFormats","invForeignKeyNames":["courseTypeId"]},{"name":"facultySimRoles","entityTypeName":"FacultySimRoleDto:#SM.Dto","isScalar":false,"associationName":"CourseTypeDto_FacultySimRoles","invForeignKeyNames":["courseTypeId"]},{"name":"scenarios","entityTypeName":"ScenarioDto:#SM.Dto","isScalar":false,"associationName":"CourseTypeDto_Scenarios","invForeignKeyNames":["courseTypeId"]}]},{"shortName":"DepartmentDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"Departments","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"name","dataType":"String","isNullable":false,"defaultValue":"","maxLength":64,"validators":[{"name":"required"},{"maxLength":64,"name":"maxLength"}]},{"name":"abbreviation","dataType":"String","maxLength":16,"validators":[{"maxLength":16,"name":"maxLength"}]},{"name":"institutionId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]},{"name":"invitationLetterFilename","dataType":"String","maxLength":256,"validators":[{"maxLength":256,"name":"maxLength"}]},{"name":"certificateFilename","dataType":"String","maxLength":256,"validators":[{"maxLength":256,"name":"maxLength"}]}],"navigationProperties":[{"name":"courseParticipants","entityTypeName":"CourseParticipantDto:#SM.Dto","isScalar":false,"associationName":"DepartmentDto_CourseParticipants","invForeignKeyNames":["departmentId"]},{"name":"courses","entityTypeName":"CourseDto:#SM.Dto","isScalar":false,"associationName":"DepartmentDto_Courses","invForeignKeyNames":["departmentId"]},{"name":"institution","entityTypeName":"InstitutionDto:#SM.Dto","isScalar":true,"associationName":"InstitutionDto_Departments","foreignKeyNames":["institutionId"]},{"name":"manequins","entityTypeName":"ManequinDto:#SM.Dto","isScalar":false,"associationName":"ManequinDto_Department","invForeignKeyNames":["departmentId"]},{"name":"outreachCourses","entityTypeName":"CourseDto:#SM.Dto","isScalar":false,"associationName":"DepartmentDto_OutreachCourses","invForeignKeyNames":["outreachingDepartmentId"]},{"name":"participants","entityTypeName":"ParticipantDto:#SM.Dto","isScalar":false,"associationName":"DepartmentDto_Participants","invForeignKeyNames":["defaultDepartmentId"]},{"name":"rooms","entityTypeName":"RoomDto:#SM.Dto","isScalar":false,"associationName":"DepartmentDto_Rooms","invForeignKeyNames":["departmentId"]},{"name":"scenarios","entityTypeName":"ScenarioDto:#SM.Dto","isScalar":false,"associationName":"DepartmentDto_Scenarios","invForeignKeyNames":["departmentId"]}]},{"shortName":"CourseParticipantDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"CourseParticipants","dataProperties":[{"name":"courseId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"participantId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"isConfirmed","dataType":"Boolean","isNullable":false,"defaultValue":false,"validators":[{"name":"required"},{"name":"bool"}]},{"name":"isFaculty","dataType":"Boolean","isNullable":false,"defaultValue":false,"validators":[{"name":"required"},{"name":"bool"}]},{"name":"isOrganiser","dataType":"Boolean","isNullable":false,"defaultValue":false,"validators":[{"name":"required"},{"name":"bool"}]},{"name":"departmentId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]},{"name":"professionalRoleId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]}],"navigationProperties":[{"name":"course","entityTypeName":"CourseDto:#SM.Dto","isScalar":true,"associationName":"CourseDto_CourseParticipants","foreignKeyNames":["courseId"]},{"name":"department","entityTypeName":"DepartmentDto:#SM.Dto","isScalar":true,"associationName":"DepartmentDto_CourseParticipants","foreignKeyNames":["departmentId"]},{"name":"participant","entityTypeName":"ParticipantDto:#SM.Dto","isScalar":true,"associationName":"ParticipantDto_CourseParticipants","foreignKeyNames":["participantId"]},{"name":"professionalRole","entityTypeName":"ProfessionalRoleDto:#SM.Dto","isScalar":true,"associationName":"ProfessionalRoleDto_CourseParticipants","foreignKeyNames":["professionalRoleId"]}]},{"shortName":"ParticipantDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"ParticipantDtoes","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"email","dataType":"String","isNullable":false,"defaultValue":"","maxLength":256,"validators":[{"name":"required"},{"maxLength":256,"name":"maxLength"},{"name":"emailAddress"}]},{"name":"phoneNumber","dataType":"String","maxLength":32,"validators":[{"maxLength":32,"name":"maxLength"},{"name":"phone"}]},{"name":"alternateEmail","dataType":"String","maxLength":256,"validators":[{"maxLength":256,"name":"maxLength"},{"name":"emailAddress"}]},{"name":"fullName","dataType":"String","isNullable":false,"defaultValue":"","maxLength":256,"validators":[{"name":"required"},{"maxLength":256,"name":"maxLength"},{"name":"personFullName","minNames":2,"maxNames":5,"minNameLength":2}]},{"name":"defaultDepartmentId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]},{"name":"defaultProfessionalRoleId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]}],"navigationProperties":[{"name":"courseParticipants","entityTypeName":"CourseParticipantDto:#SM.Dto","isScalar":false,"associationName":"ParticipantDto_CourseParticipants","invForeignKeyNames":["participantId"]},{"name":"courseScenarioFacultyRoles","entityTypeName":"CourseScenarioFacultyRoleDto:#SM.Dto","isScalar":false,"associationName":"ParticipantDto_CourseScenarioFacultyRoles","invForeignKeyNames":["facultyMemberId"]},{"name":"courseSlotPresentations","entityTypeName":"CourseSlotPresenterDto:#SM.Dto","isScalar":false,"associationName":"ParticipantDto_CourseSlotPresentations","invForeignKeyNames":["presenterId"]},{"name":"department","entityTypeName":"DepartmentDto:#SM.Dto","isScalar":true,"associationName":"DepartmentDto_Participants","foreignKeyNames":["defaultDepartmentId"]},{"name":"professionalRole","entityTypeName":"ProfessionalRoleDto:#SM.Dto","isScalar":true,"associationName":"ProfessionalRoleDto_Participants","foreignKeyNames":["defaultProfessionalRoleId"]}]},{"shortName":"CourseScenarioFacultyRoleDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"CourseScenarioFacultyRoles","dataProperties":[{"name":"courseId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"courseSlotId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"facultyMemberId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"facultySimRoleId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]}],"navigationProperties":[{"name":"course","entityTypeName":"CourseDto:#SM.Dto","isScalar":true,"associationName":"CourseDto_CourseScenarioFacultyRoles","foreignKeyNames":["courseId"]},{"name":"courseSlot","entityTypeName":"CourseSlotDto:#SM.Dto","isScalar":true,"associationName":"CourseSlotDto_CourseScenarioFacultyRoles","foreignKeyNames":["courseSlotId"]},{"name":"facultyMember","entityTypeName":"ParticipantDto:#SM.Dto","isScalar":true,"associationName":"ParticipantDto_CourseScenarioFacultyRoles","foreignKeyNames":["facultyMemberId"]},{"name":"facultySimRole","entityTypeName":"FacultySimRoleDto:#SM.Dto","isScalar":true,"associationName":"FacultySimRoleDto_CourseScenarioFacultyRoles","foreignKeyNames":["facultySimRoleId"]}]},{"shortName":"FacultySimRoleDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"FacultySimRoles","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"description","dataType":"String","isNullable":false,"defaultValue":"","maxLength":50,"validators":[{"name":"required"},{"maxLength":50,"name":"maxLength"}]},{"name":"courseTypeId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]}],"navigationProperties":[{"name":"courseScenarioFacultyRoles","entityTypeName":"CourseScenarioFacultyRoleDto:#SM.Dto","isScalar":false,"associationName":"FacultySimRoleDto_CourseScenarioFacultyRoles","invForeignKeyNames":["facultySimRoleId"]},{"name":"courseType","entityTypeName":"CourseTypeDto:#SM.Dto","isScalar":true,"associationName":"CourseTypeDto_FacultySimRoles","foreignKeyNames":["courseTypeId"]}]},{"shortName":"CourseSlotPresenterDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"CourseSlotPresenters","dataProperties":[{"name":"courseId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"courseSlotId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"presenterId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]}],"navigationProperties":[{"name":"course","entityTypeName":"CourseDto:#SM.Dto","isScalar":true,"associationName":"CourseDto_CourseSlotPresenters","foreignKeyNames":["courseId"]},{"name":"courseSlot","entityTypeName":"CourseSlotDto:#SM.Dto","isScalar":true,"associationName":"CourseSlotDto_CourseSlotPresenters","foreignKeyNames":["courseSlotId"]},{"name":"presenter","entityTypeName":"ParticipantDto:#SM.Dto","isScalar":true,"associationName":"ParticipantDto_CourseSlotPresentations","foreignKeyNames":["presenterId"]}]},{"shortName":"ProfessionalRoleDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"ProfessionalRoles","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"description","dataType":"String","isNullable":false,"defaultValue":"","maxLength":50,"validators":[{"name":"required"},{"maxLength":50,"name":"maxLength"}]},{"name":"category","dataType":"String","isNullable":false,"defaultValue":"","validators":[{"name":"required"},{"name":"string"}],"enumType":"Edm.Self.ProfessionalCategory"}],"navigationProperties":[{"name":"courseParticipants","entityTypeName":"CourseParticipantDto:#SM.Dto","isScalar":false,"associationName":"ProfessionalRoleDto_CourseParticipants","invForeignKeyNames":["professionalRoleId"]},{"name":"participants","entityTypeName":"ParticipantDto:#SM.Dto","isScalar":false,"associationName":"ProfessionalRoleDto_Participants","invForeignKeyNames":["defaultProfessionalRoleId"]}]},{"shortName":"InstitutionDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"Institutions","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"name","dataType":"String","isNullable":false,"defaultValue":"","maxLength":50,"validators":[{"name":"required"},{"maxLength":50,"name":"maxLength"}]},{"name":"about","dataType":"String","validators":[{"name":"string"}]},{"name":"localeCode","dataType":"String","isNullable":false,"defaultValue":"","maxLength":5,"validators":[{"name":"required"},{"name":"stringLength","minLength":5,"maxLength":5}]},{"name":"standardTimeZone","dataType":"String","maxLength":40,"validators":[{"maxLength":40,"name":"maxLength"}]}],"navigationProperties":[{"name":"country","entityTypeName":"CountryDto:#SM.Dto","isScalar":true,"associationName":"CountryDto_Institutions","foreignKeyNames":["localeCode"]},{"name":"departments","entityTypeName":"DepartmentDto:#SM.Dto","isScalar":false,"associationName":"InstitutionDto_Departments","invForeignKeyNames":["institutionId"]}]},{"shortName":"CountryDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"Countries","dataProperties":[{"name":"localeCode","dataType":"String","isNullable":false,"defaultValue":"","isPartOfKey":true,"maxLength":5,"validators":[{"name":"required"},{"name":"stringLength","minLength":5,"maxLength":5},{"name":"regularExpression","expression":"[a-z]{2}-[A-Z]{2}"}]},{"name":"name","dataType":"String","isNullable":false,"defaultValue":"","maxLength":50,"validators":[{"name":"required"},{"maxLength":50,"name":"maxLength"}]},{"name":"dialCode","dataType":"String","isNullable":false,"defaultValue":"","maxLength":3,"validators":[{"name":"required"},{"name":"stringLength","minLength":2,"maxLength":3},{"name":"regularExpression","expression":"\\d+"}]}],"navigationProperties":[{"name":"institutions","entityTypeName":"InstitutionDto:#SM.Dto","isScalar":false,"associationName":"CountryDto_Institutions","invForeignKeyNames":["localeCode"]}]},{"shortName":"ManequinDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"Manequins","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"description","dataType":"String","isNullable":false,"defaultValue":"","maxLength":128,"validators":[{"name":"required"},{"maxLength":128,"name":"maxLength"}]},{"name":"departmentId","dataType":"Guid","validators":[{"name":"guid"}]},{"name":"modelId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]},{"name":"purchasedNew","dataType":"Boolean","isNullable":false,"defaultValue":false,"validators":[{"name":"required"},{"name":"bool"}]},{"name":"purchaseDate","dataType":"DateTime","validators":[{"name":"date"}]},{"name":"localCurrencyPurchasePrice","dataType":"Decimal","validators":[{"name":"number"}]},{"name":"decommissionDate","dataType":"DateTime","validators":[{"name":"date"}]},{"name":"decommissionReason","dataType":"String","maxLength":512,"validators":[{"maxLength":512,"name":"maxLength"}]}],"navigationProperties":[{"name":"courseSlotScenarios","entityTypeName":"CourseSlotScenarioDto:#SM.Dto","isScalar":false,"associationName":"ManequinDto_CourseSlotScenarios","invForeignKeyNames":["manequinId"]},{"name":"department","entityTypeName":"DepartmentDto:#SM.Dto","isScalar":true,"associationName":"ManequinDto_Department","foreignKeyNames":["departmentId"]},{"name":"manequinServices","entityTypeName":"ManequinServiceDto:#SM.Dto","isScalar":false,"associationName":"ManequinDto_ManequinServices","invForeignKeyNames":["manequinId"]},{"name":"model","entityTypeName":"ManequinModelDto:#SM.Dto","isScalar":true,"associationName":"ManequinModelDto_Manequins","foreignKeyNames":["modelId"]}]},{"shortName":"CourseSlotScenarioDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"CourseSlotScenarios","dataProperties":[{"name":"courseId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"courseSlotId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"scenarioId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"manequinId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]}],"navigationProperties":[{"name":"course","entityTypeName":"CourseDto:#SM.Dto","isScalar":true,"associationName":"CourseDto_CourseSlotScenarios","foreignKeyNames":["courseId"]},{"name":"courseSlot","entityTypeName":"CourseSlotDto:#SM.Dto","isScalar":true,"associationName":"CourseSlotDto_CourseSlotScenarios","foreignKeyNames":["courseSlotId"]},{"name":"manequin","entityTypeName":"ManequinDto:#SM.Dto","isScalar":true,"associationName":"ManequinDto_CourseSlotScenarios","foreignKeyNames":["manequinId"]},{"name":"scenario","entityTypeName":"ScenarioDto:#SM.Dto","isScalar":true,"associationName":"ScenarioDto_CourseSlotScenarios","foreignKeyNames":["scenarioId"]}]},{"shortName":"ScenarioDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"Scenarios","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"description","dataType":"String","isNullable":false,"defaultValue":"","maxLength":128,"validators":[{"name":"required"},{"maxLength":128,"name":"maxLength"}]},{"name":"departmentId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]},{"name":"complexity","dataType":"String","isNullable":false,"defaultValue":"","validators":[{"name":"required"},{"name":"string"}],"enumType":"Edm.Self.Difficulty"},{"name":"emersionCategory","dataType":"String","validators":[{"name":"string"}],"enumType":"Edm.Self.Emersion"},{"name":"templateFilename","dataType":"String","maxLength":256,"validators":[{"maxLength":256,"name":"maxLength"}]},{"name":"manequinModelId","dataType":"Guid","validators":[{"name":"guid"}]},{"name":"courseTypeId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]}],"navigationProperties":[{"name":"courseSlotScenarios","entityTypeName":"CourseSlotScenarioDto:#SM.Dto","isScalar":false,"associationName":"ScenarioDto_CourseSlotScenarios","invForeignKeyNames":["scenarioId"]},{"name":"courseType","entityTypeName":"CourseTypeDto:#SM.Dto","isScalar":true,"associationName":"CourseTypeDto_Scenarios","foreignKeyNames":["courseTypeId"]},{"name":"department","entityTypeName":"DepartmentDto:#SM.Dto","isScalar":true,"associationName":"DepartmentDto_Scenarios","foreignKeyNames":["departmentId"]},{"name":"manequinModel","entityTypeName":"ManequinModelDto:#SM.Dto","isScalar":true,"associationName":"ManequinModelDto_Scenarios","foreignKeyNames":["manequinModelId"]},{"name":"scenarioResources","entityTypeName":"ScenarioResourceDto:#SM.Dto","isScalar":false,"associationName":"ScenarioResourceDto_Scenario","invForeignKeyNames":["scenarioId"]}]},{"shortName":"ManequinModelDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"ManequinModelDtoes","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"description","dataType":"String","isNullable":false,"defaultValue":"","maxLength":128,"validators":[{"name":"required"},{"maxLength":128,"name":"maxLength"}]},{"name":"manufacturerId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]}],"navigationProperties":[{"name":"manequins","entityTypeName":"ManequinDto:#SM.Dto","isScalar":false,"associationName":"ManequinModelDto_Manequins","invForeignKeyNames":["modelId"]},{"name":"manufacturer","entityTypeName":"ManequinManufacturerDto:#SM.Dto","isScalar":true,"associationName":"ManequinManufacturerDto_ManequinModels","foreignKeyNames":["manufacturerId"]},{"name":"scenarios","entityTypeName":"ScenarioDto:#SM.Dto","isScalar":false,"associationName":"ManequinModelDto_Scenarios","invForeignKeyNames":["manequinModelId"]}]},{"shortName":"ManequinManufacturerDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"ManequinManufacturers","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"name","dataType":"String","maxLength":256,"validators":[{"maxLength":256,"name":"maxLength"}]}],"navigationProperties":[{"name":"manequinModels","entityTypeName":"ManequinModelDto:#SM.Dto","isScalar":false,"associationName":"ManequinManufacturerDto_ManequinModels","invForeignKeyNames":["manufacturerId"]}]},{"shortName":"ScenarioResourceDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"ScenarioResources","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"scenarioId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]},{"name":"name","dataType":"String","validators":[{"name":"string"},{"name":"required"},{"name":"maxLength","maxLength":128}]},{"name":"resourceFilename","dataType":"String","validators":[{"name":"string"},{"name":"maxLength","maxLength":256}]}],"navigationProperties":[{"name":"scenario","entityTypeName":"ScenarioDto:#SM.Dto","isScalar":true,"associationName":"ScenarioResourceDto_Scenario","foreignKeyNames":["scenarioId"]}]},{"shortName":"ManequinServiceDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"ManequinServiceDtoes","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"problemDescription","dataType":"String","maxLength":1028,"validators":[{"maxLength":1028,"name":"maxLength"}]},{"name":"servicedInternally","dataType":"Boolean","isNullable":false,"defaultValue":false,"validators":[{"name":"required"},{"name":"bool"}]},{"name":"sent","dataType":"DateTime","isNullable":false,"defaultValue":"","validators":[{"name":"required"},{"name":"date"}]},{"name":"returned","dataType":"DateTime","validators":[{"name":"date"}]},{"name":"priceEstimate","dataType":"Decimal","validators":[{"name":"number"}]},{"name":"serviceCost","dataType":"Decimal","isNullable":false,"defaultValue":0,"validators":[{"name":"required"},{"name":"number"}]},{"name":"manequinId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]}],"navigationProperties":[{"name":"manequin","entityTypeName":"ManequinDto:#SM.Dto","isScalar":true,"associationName":"ManequinDto_ManequinServices","foreignKeyNames":["manequinId"]}]},{"shortName":"RoomDto","namespace":"SM.Dto","autoGeneratedKeyType":"None","defaultResourceName":"Rooms","dataProperties":[{"name":"id","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","isPartOfKey":true,"validators":[{"name":"required"},{"name":"guid"}]},{"name":"shortDescription","dataType":"String","maxLength":32,"validators":[{"maxLength":32,"name":"maxLength"}]},{"name":"fullDescription","dataType":"String","maxLength":64,"validators":[{"maxLength":64,"name":"maxLength"}]},{"name":"directions","dataType":"String","maxLength":256,"validators":[{"maxLength":256,"name":"maxLength"}]},{"name":"departmentId","dataType":"Guid","isNullable":false,"defaultValue":"00000000-0000-0000-0000-000000000000","validators":[{"name":"required"},{"name":"guid"}]}],"navigationProperties":[{"name":"courses","entityTypeName":"CourseDto:#SM.Dto","isScalar":false,"associationName":"RoomDto_Courses","invForeignKeyNames":["roomId"]},{"name":"department","entityTypeName":"DepartmentDto:#SM.Dto","isScalar":true,"associationName":"DepartmentDto_Rooms","foreignKeyNames":["departmentId"]}]}],"resourceEntityTypeMap":{"ActivityTeachingResources":"ActivityTeachingResourceDto:#SM.Dto","ChosenTeachingResources":"ChosenTeachingResourceDto:#SM.Dto","Courses":"CourseDto:#SM.Dto","CourseFormats":"CourseFormatDto:#SM.Dto","CourseSlots":"CourseSlotDto:#SM.Dto","CourseActivities":"CourseActivityDto:#SM.Dto","CourseTypes":"CourseTypeDto:#SM.Dto","Departments":"DepartmentDto:#SM.Dto","CourseParticipants":"CourseParticipantDto:#SM.Dto","ParticipantDtoes":"ParticipantDto:#SM.Dto","CourseScenarioFacultyRoles":"CourseScenarioFacultyRoleDto:#SM.Dto","FacultySimRoles":"FacultySimRoleDto:#SM.Dto","CourseSlotPresenters":"CourseSlotPresenterDto:#SM.Dto","ProfessionalRoles":"ProfessionalRoleDto:#SM.Dto","Institutions":"InstitutionDto:#SM.Dto","Countries":"CountryDto:#SM.Dto","Manequins":"ManequinDto:#SM.Dto","CourseSlotScenarios":"CourseSlotScenarioDto:#SM.Dto","Scenarios":"ScenarioDto:#SM.Dto","ManequinModelDtoes":"ManequinModelDto:#SM.Dto","ManequinManufacturers":"ManequinManufacturerDto:#SM.Dto","ScenarioResources":"ScenarioResourceDto:#SM.Dto","ManequinServiceDtoes":"ManequinServiceDto:#SM.Dto","Rooms":"RoomDto:#SM.Dto"}});
	}
	function getBreezeValidators(){
		return {"ChosenTeachingResourceDto":["activityTeachingResource","course","courseSlot"],"ActivityTeachingResourceDto":["courseActivity"],"CourseDto":["courseFormat","department","room"],"CourseParticipantDto":["course","department","participant","professionalRole"],"CourseScenarioFacultyRoleDto":["course","courseSlot","facultyMember","facultySimRole"],"CourseSlotPresenterDto":["course","courseSlot","presenter"],"CourseSlotScenarioDto":["course","courseSlot","manequin","scenario"],"CourseSlotDto":["courseFormat"],"CourseFormatDto":["courseType"],"CourseActivityDto":["courseType"],"FacultySimRoleDto":["courseType"],"ScenarioDto":["courseType","department"],"DepartmentDto":["institution"],"ParticipantDto":["department","professionalRole"],"RoomDto":["department"],"InstitutionDto":["country"],"ManequinServiceDto":["manequin"],"ManequinDto":["model"],"ScenarioResourceDto":["scenario"],"ManequinModelDto":["manufacturer"]};
	}
})();

