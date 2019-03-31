using System;
using DreamPlace.Lib.Rx;
using NUnit.Framework;

namespace TestProject1
{
    [TestFixture]
    public class Tests
    {
        [Test]
        
        public void Test1()
        {   
            Registry.Subscribe<TestEnum>((e) =>
            {
                Assert.IsTrue(true); 
            }, 1);
            Registry.OnNext(TestEnum.A, 1);
        }
        
        [Test]
        public void Test2()
        {
            var actualValue = false;
            
            Registry.Public(new A{AValue = 2});
            
            Assert.IsTrue(Registry.GetValue<A>().AValue == 2);
        }
        
        [Test]
        public void Test3()
        {
            var actualValue = false;
            
            Registry.Public("123");
            Registry.Public("1234");
            
            Assert.IsTrue(Registry.GetValue<string>() == "1234");
        }
        
        [Test]
        public void Test4()
        {
            var actualValue = false;
            
            Registry.Public<string>(null);
            Registry.Public<string>("12w3");
            Registry.Public<int>(1);
            
            Assert.IsTrue(Registry.GetValue<string>() == "12w3");
            Assert.IsTrue(Registry.GetValue<int>() == 1);
        }
        
        [Test]
        public void EqualObjectWithInt()
        {
           Assert.IsTrue(((object)1) == ((object)1));
        }

        enum TestEnum
        {
            A,
            B
        }

        class B
        {
            
        }
        class  A
        {
            public int AValue = 1;
        }
    }
}