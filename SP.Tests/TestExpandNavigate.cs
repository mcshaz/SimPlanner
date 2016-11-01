using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Collections.Generic;
using SP.DataAccess;
using System.Data.Entity;
using System.Linq;
using SP.Dto.Maps;
using SP.Dto;
using SP.Dto.ProcessBreezeRequests;

namespace SimPlanner.Tests
{
    [TestClass]
    public class TestExpandNavigate
    {

        static Expression<Func<Bar, BarDto>> MapBar = b => new BarDto { BarId = b.BarId, BarString = b.BarString };
        static Expression<Func<Foo, FooDto>> MapFoo = f => new FooDto { FooId = f.FooId };
        static readonly NavProperty[] _navProperties = new[] {
                new NavProperty(typeof(FooDto).GetProperty("Bars") ,MapBar),
                new NavProperty(typeof(FooDto).GetProperty("B"), MapBar)
            };
        CurrentUser _currentUser;
        CurrentUser CurrentUser
        {
            get
            {
                return _currentUser ?? (_currentUser = new CurrentUser(new MockIPrincipal()));
            }
        }

        Expression<Func<Foo, FooDto>> target = f => new FooDto
        {
            FooId = f.FooId,
            B = new BarDto { BarId = f.B.BarId, BarString = f.B.BarString },
            Bars = f.Bars.Select(b => new BarDto { BarId = b.BarId, BarString = b.BarString }).ToList()
        };
        
        [TestMethod]
        public void TestExpandNavigateLinq()
        {
            //var l = new LoggingVisitor();
            //l.Visit(target);
            var bar = new Bar { BarId = 12, BarString = "a" };
            var foo = new Foo { FooId = -1, B = bar, Bars =  new[] { bar, new Bar { BarId = 3, BarString = "b" } } };

            var mapNavEx = MapFoo.MapNavProperty(_navProperties);

            Console.WriteLine(mapNavEx.ToString());
            Func<Foo, FooDto> mapNav = mapNavEx.Compile();

            var testMapped = mapNav(foo);

            Console.WriteLine("Testing single property");
            Assert.AreNotEqual(foo, testMapped);
            Assert.AreEqual(foo.FooId, testMapped.FooId);
            Assert.IsNotNull(foo.B);
            Assert.AreNotEqual(foo.B, testMapped.B);
            Assert.AreEqual(foo.B.BarId, testMapped.B.BarId);
            Assert.AreEqual(foo.B.BarString, testMapped.B.BarString);

            Console.WriteLine("Testing collection"); 

            Assert.AreEqual(foo.Bars.Count, testMapped.Bars.Count()); //todo change to count property
            var fooBarsLast = foo.Bars.Last();
            var testBarsLast = testMapped.Bars.Last();

            Assert.AreEqual(fooBarsLast.BarId, fooBarsLast.BarId);


        }
        [TestMethod]
        public void TestExpandNavigatePerformance()
        {
            //performance tests

            int count = 100; // add 0s as appropriate
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < count; i++)
            {
                MapFoo.MapNavProperty(_navProperties);
            }
            Console.WriteLine("Svick method elapsed: " + sw.Elapsed);
            /*
            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                Svick.ExpressionTreeExtensions.MapNavProperty2(MapFoo, mapProperties);
            }
            Console.WriteLine("Svick method elapsed: " + sw.Elapsed);
            */
        }
        const string testUserName = "brentm@adhb.govt.nz";
        [TestMethod]
        public void TestEntityMappingNavProperty()
        {
            const string propName = "Department";
            using (MedSimDbContext db = new MedSimDbContext())
            {

                var mapNavEx = MapperConfig.GetToDtoLambda<Participant, ParticipantDto>(CurrentUser, new[] { propName });
                Console.WriteLine(mapNavEx.ToString());
                var part = db.Users.Include(propName).AsNoTracking().First(p => p.UserName == testUserName);

                var partVm = db.Users.Select(mapNavEx).First(p => p.Email == part.Email);

                Assert.AreEqual(part.FullName, partVm.FullName);

                Assert.AreEqual(part.Department.Id, partVm.Department.Id);
                Assert.AreEqual(part.Department.InstitutionId, partVm.Department.InstitutionId);
            }

        }
        [TestMethod]
        public void TestEntityMappingCollection()
        {
            const string collectionPropName = "CourseParticipants";
            using (MedSimDbContext db = new MedSimDbContext())
            {

                var mapNavEx = MapperConfig.GetToDtoLambda<Participant, ParticipantDto>(CurrentUser, new[] { collectionPropName });
                Console.WriteLine(mapNavEx.ToString());
                var part = db.Users.Include(collectionPropName).AsNoTracking().First(p => p.UserName == testUserName);

                var partVm = db.Users.Select(mapNavEx).First(p => p.Email == part.Email);

                Assert.AreEqual(part.FullName, partVm.FullName);

                var cp = part.CourseParticipants.First();
                var cpvm = partVm.CourseParticipants.First();
                Assert.AreEqual(cp.ParticipantId, cpvm.ParticipantId);
                Assert.AreEqual(cp.CourseId, cpvm.CourseId);
            }
        }
        [TestMethod]
        public void TestTreeTopMapping()
        {
            using (MedSimDbContext db = new MedSimDbContext())
            {
                var u = db.Scenarios.ProjectToDto<Scenario, ScenarioDto>(CurrentUser).ToList();
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(u,Newtonsoft.Json.Formatting.Indented));
            }
        }
        [TestMethod]
        public void TestComplexDtoTreeMapping()
        {
            //var test = MapperConfig.GetLambda<Participant, ParticipantDto>(new[] { "Department", "Department.Institution", "Department.Manikins", "Department.Manikins.CourseSlotScenarios", "ProfessionalRole.CourseParticipants" });
            //Console.WriteLine(test);
            var test2 = MapperConfig.GetToDtoLambda<Course, CourseDto>(CurrentUser, new[] {
                        "CourseParticipants",
                        "CourseFormat.CourseSlots.Activity.ActivityChoices",
                        "CourseSlotActivities",
                        "CourseSlotPresenters",
                        "CourseScenarioFacultyRoles",
                        "CourseFormat.CourseType.Scenarios"});



            Console.WriteLine(test2);
            using (MedSimDbContext db = new MedSimDbContext())
            {
                var c = db.Courses.Select(test2).First();
                /*
            var c = db.Courses.Select(m => new CourseDto()
            {
                CourseParticipants = m.CourseParticipants.Select(cp => new CourseParticipantDto()
                {
                    ParticipantId = cp.ParticipantId,
                }).ToList(),
                CourseFormat = new CourseFormatDto()
                {
                    Id = m.CourseFormat.Id,
                    CourseSlots = m.CourseFormat.CourseSlots.Select(cs => new CourseSlotDto()
                    {
                        Id = cs.Id,
                        MinutesDuration = cs.MinutesDuration,
                        Day = cs.Day,
                        Activity = cs.Activity==null
                            ?null
                            :new CourseActivityDto()
                            {
                                Id = cs.Activity.Id,
                                ActivityChoices = cs.Activity.ActivityChoices.Select(at => new ActivityDto()
                                    {
                                        Id = m.Id,
                                    }).ToList() 
                            }
                    }).ToList()
                }
            }).First();
                            */
                Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(c, Newtonsoft.Json.Formatting.Indented));

            }
        }
        [TestMethod]
        public void TestNestedWhere()
        {
            using (MedSimDbContext db = new MedSimDbContext())
            {
                Expression<Func<Institution, InstitutionDto>> i = u => new InstitutionDto
                {
                    Id = u.Id,
                    Departments = u.Departments.Where(d => d.Abbreviation == "ced").Select(d => new DepartmentDto { Id = d.Id, Abbreviation = d.Abbreviation }).ToList()
                };
                var insts = db.Institutions.Select(i);
                //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(insts, Newtonsoft.Json.Formatting.Indented));
                Console.WriteLine(i);
                //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(i, Newtonsoft.Json.Formatting.Indented));

               
            }
        }
    }

    //http://stackoverflow.com/questions/10898800/combine-two-linq-expressions-to-inject-navigation-property
    //and
    //https://coding.abel.nu/2013/01/merging-expression-trees-to-reuse-in-linq-queries/

}

