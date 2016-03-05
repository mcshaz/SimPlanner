using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Collections.Generic;
using SM.DataAccess;
using System.Data.Entity;
using System.Linq;
using SM.DTOs.Maps;
using SM.Dto;

namespace SimManager.Tests
{
    [TestClass]
    public class TestExpandNavigate
    {
        class Foo
        {
            public int FooInt { get; set; }
            public Bar B { get; set; }
            public ICollection<Bar> Bars { get; set; }
        }

        class FooDto
        {
            public int FooInt { get; set; }
            public BarDto B { get; set; }
            public IEnumerable<BarDto> Bars { get; set; }
        }

        class Bar
        {
            public int BarInt { get; set; }
            public string BarString { get; set; }
        }

        class BarDto
        {
            public int BarInt { get; set; }
            public string BarString { get; set; }
        }

        Expression<Func<Bar, BarDto>> MapBar = b => new BarDto { BarInt = b.BarInt, BarString = b.BarString };
        Expression<Func<Foo, FooDto>> MapFoo = f => new FooDto { FooInt = f.FooInt };
        
        Expression<Func<Foo, FooDto>> target = f => new FooDto
        {
            FooInt = f.FooInt,
            B = new BarDto { BarInt = f.B.BarInt, BarString = f.B.BarString },
            Bars = f.Bars.Select(b => new BarDto { BarInt = b.BarInt, BarString = b.BarString }) //.ToList()
        };
        
        [TestMethod]
        public void TestExpandNavigateLinq()
        {
            //var l = new LoggingVisitor();
            //l.Visit(target);
            var bar = new Bar { BarInt = 12, BarString = "a" };
            var foo = new Foo { FooInt = -1, B = bar, Bars =  new[] { bar, new Bar { BarInt = 3, BarString = "b" } } };

            var mapNavEx = Svick.ExpressionTreeExtensions.MapNavProperty(MapFoo, MapBar, "Bars");

            Console.WriteLine(mapNavEx.ToString());
            Func<Foo, FooDto> mapNav = mapNavEx.Compile();

            var testMapped = mapNav(foo);

            Assert.AreNotEqual(foo, testMapped);
            Assert.AreEqual(foo.FooInt, testMapped.FooInt);
            Assert.IsNotNull(foo.B);
            Assert.AreNotEqual(foo.B, testMapped.B);
            Assert.AreEqual(foo.B.BarInt, testMapped.B.BarInt);
            Assert.AreEqual(foo.B.BarString, testMapped.B.BarString);

            mapNavEx = Svick.ExpressionTreeExtensions.MapNavProperty(MapFoo, MapBar, "Bars");

            Console.WriteLine(mapNavEx.ToString());
            mapNav = mapNavEx.Compile();

            testMapped = mapNav(foo);

            Assert.AreEqual(foo.Bars.Count, testMapped.Bars.Count()); //todo change to count property
            var fooBarsLast = foo.Bars.Last();
            var testBarsLast = testMapped.Bars.Last();

            Assert.AreEqual(fooBarsLast.BarInt, fooBarsLast.BarInt);


        }
        [TestMethod]
        public void TestExpandNavigatePerformance()
        { 
            //performance tests
            int count = 1000000;
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < count; i++)
            {
                IvanStoev.ExpressionTreeExtensions.MapNavProperty(MapFoo, MapBar, "B");
            }
            Console.WriteLine("IvanStoev method elapsed: " + sw.Elapsed);
            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                Svick.ExpressionTreeExtensions.MapNavProperty(MapFoo, MapBar, "B");
            }
            Console.WriteLine("Svick method elapsed: " + sw.Elapsed);
        }
        const string testUserName = "brentm@adhb.govt.nz";
        [TestMethod]
        public void TestEntityMappingNavProperty()
        {
            const string propName = "Department";
            using (MedSimDbContext db = new MedSimDbContext())
            {
                foreach (var mapNavEx in new Expression<Func<Participant, ParticipantDto>>[] { IvanStoev.ExpressionTreeExtensions.MapNavProperty(ParticipantMaps.mapFromRepo, DepartmentMaps.mapFromRepo, propName),
                    Svick.ExpressionTreeExtensions.MapNavProperty(ParticipantMaps.mapFromRepo, DepartmentMaps.mapFromRepo, propName)})
                {
                    Console.WriteLine(mapNavEx.ToString());
                    var part = db.Users.Include(propName).AsNoTracking().First(p => p.UserName == testUserName);

                    var partVm = db.Users.Select(mapNavEx).First(p => p.Email == part.Email);

                    Assert.AreEqual(part.FullName, partVm.FullName);

                    Assert.AreEqual(part.Department.Id, partVm.Department.Id);
                    Assert.AreEqual(part.Department.InstitutionId, partVm.Department.InstitutionId);
                }
            }

        }
        [TestMethod]
        public void TestEntityMappingCollection()
        {
            const string collectionPropName = "CourseParticipants";
            using (MedSimDbContext db = new MedSimDbContext())
            {
                foreach (var mapNavEx in new Expression<Func<Participant, ParticipantDto>>[] { IvanStoev.ExpressionTreeExtensions.MapNavProperty(ParticipantMaps.mapFromRepo, CourseParticipantMaps.mapFromRepo, collectionPropName),
                    Svick.ExpressionTreeExtensions.MapNavProperty(ParticipantMaps.mapFromRepo, CourseParticipantMaps.mapFromRepo, collectionPropName)})
                { 

                    var part = db.Users.Include(collectionPropName).AsNoTracking().First(p => p.UserName == testUserName);

                    var partVm = db.Users.Select(mapNavEx).First(p => p.Email == part.Email);

                    Assert.AreEqual(part.FullName, partVm.FullName);

                    var cp = part.CourseParticipants.First();
                    var cpvm = partVm.CourseParticipants.First();
                    Assert.AreEqual(cp.ParticipantId, cpvm.ParticipantId);
                    Assert.AreEqual(cp.CourseId, cpvm.CourseId);
                }
            }

        }
    }

    //http://stackoverflow.com/questions/10898800/combine-two-linq-expressions-to-inject-navigation-property
    //and
    //https://coding.abel.nu/2013/01/merging-expression-trees-to-reuse-in-linq-queries/

}

