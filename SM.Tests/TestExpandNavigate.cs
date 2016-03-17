﻿using System;
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

        Expression<Func<Bar, BarDto>> MapBar = b => new BarDto { BarId = b.BarId, BarString = b.BarString };
        Expression<Func<Foo, FooDto>> MapFoo = f => new FooDto { FooId = f.FooId };
        
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

            var mapNavEx = MapFoo.MapNavProperty(new Dictionary<string,LambdaExpression> {
                { "Bars",MapBar},
                { "B", MapBar}
            });

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

            var mapProperties = new Dictionary<string, LambdaExpression> {
                { "Bars",MapBar},
                { "B", MapBar}
            };
            int count = 1000000;
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < count; i++)
            {
                MapFoo.MapNavProperty(mapProperties);
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

                var mapNavEx = ParticipantMaps.mapFromRepo().MapNavProperty(propName, DepartmentMaps.mapFromRepo());
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

                var mapNavEx = ParticipantMaps.mapFromRepo().MapNavProperty(collectionPropName, CourseParticipantMaps.mapFromRepo());
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
        public void TestComplexDtoTreeMapping()
        {
            var test = MapperConfig.GetLambda<Participant, ParticipantDto>(new[] { "Department", "Department.Institution", "Department.Manequins", "Department.Manequins.Scenarios", "ProfessionalRole.CourseParticipants" });
            Console.WriteLine(test);
        }
    }

    //http://stackoverflow.com/questions/10898800/combine-two-linq-expressions-to-inject-navigation-property
    //and
    //https://coding.abel.nu/2013/01/merging-expression-trees-to-reuse-in-linq-queries/

}
