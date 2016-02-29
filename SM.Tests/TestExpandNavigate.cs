using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using System.Diagnostics;

namespace SimManager.Tests
{
    [TestClass]
    public class TestExpandNavigate
    {
        [TestMethod]
        public void TestExpandNavigateLinq()
        {
            var bar = new Bar { BarInt = 12, BarString = "a" };
            var foo = new Foo { FooInt = -1, B = bar };

            Expression<Func<Bar, Bar>> mapBar = b => new Bar { BarInt = b.BarInt, BarString = b.BarString };
            Expression<Func<Foo, Foo>> mapFoo = f => new Foo { FooInt = f.FooInt, B = null };

            Expression<Func<Foo, Foo>> target = f => new Foo { FooInt = f.FooInt, B = new Bar { BarInt=f.B.BarInt, BarString = f.B.BarString } };

            foreach(var mapNavEx in new Expression <Func<Foo, Foo>>[] { IvanStoev.ExpressionTreeExtensions.MapNavProperty(mapFoo, mapBar, "B"),
                Svick.ExpressionTreeExtensions.MapNavProperty(mapFoo, mapBar, "B")})
            {
                Func<Foo, Foo> mapNav = mapNavEx.Compile();

                var testMapped = mapNav(foo);

                Assert.AreNotEqual(foo, testMapped);
                Assert.AreEqual(foo.FooInt, testMapped.FooInt);
                Assert.IsNotNull(foo.B);
                Assert.AreNotEqual(foo.B, testMapped.B);
                Assert.AreEqual(foo.B.BarInt, testMapped.B.BarInt);
                Assert.AreEqual(foo.B.BarString, testMapped.B.BarString);
            }

            //performance tests
            int count = 1000000;
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < count; i++)
            {
                IvanStoev.ExpressionTreeExtensions.MapNavProperty(mapFoo, mapBar, "B");
            }
            Console.WriteLine("IvanStoev method elapsed: " + sw.Elapsed);
            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                Svick.ExpressionTreeExtensions.MapNavProperty(mapFoo, mapBar, "B");
            }
            Console.WriteLine("Svick method elapsed: " + sw.Elapsed);
        }

        class Foo
        {
            public int FooInt { get; set; }
            public Bar B { get; set; }
        }
         
        class Bar
        {
            public int BarInt { get; set; }
            public string BarString { get; set; }
        }
    }

    //http://stackoverflow.com/questions/10898800/combine-two-linq-expressions-to-inject-navigation-property
    //and
    //https://coding.abel.nu/2013/01/merging-expression-trees-to-reuse-in-linq-queries/

    public class LoggingVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldExpr;
        private readonly Expression _newExpr;
        public LoggingVisitor(ParameterExpression oldExpr, Expression newExpr)
        {
            _oldExpr = oldExpr;
            _newExpr = newExpr;
        }
        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {//1st hit
            Console.WriteLine(node);
            return base.VisitMemberBinding(node);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {//2ndhit
            Console.WriteLine(node);
            return base.VisitMemberAssignment(node);
        }
        protected override Expression VisitMember(MemberExpression node)
        {//3rd hit
            Console.WriteLine(node);
            return base.VisitMember(node);
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            Console.WriteLine(node);
            return base.VisitMemberListBinding(node);
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            Console.WriteLine(node);
            return base.VisitMemberMemberBinding(node);
        }

    }

}

