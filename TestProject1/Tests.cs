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
                if(e.Value == TestEnum.B)
                Assert.IsTrue(true);
                else
                Assert.IsTrue(false);
            }, 1);
            
            Registry.OnNext(TestEnum.B, 1);
        }
        
        [Test]
        public void Test1_UsePublishedValueInOnNext()
        {   
            Registry.Subscribe<TestEnum>((e) =>
            {
                if(e.Value == TestEnum.B && Registry.GetValue<int>() == 1)
                    Assert.IsTrue(true);
                else
                    Assert.IsTrue(false);
            }, 1);
            Registry.Public(1);
            Registry.OnNext(TestEnum.B, 1);
        }
        
        [Test]
        public void Test2()
        {
            Registry.Public(new A{AValue = 2});
         
            var actualValue = Registry.GetValue<A>().AValue;
            
            Assert.IsTrue(actualValue == 2);
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
        public void Test3_1()
        {
            var actualValue = false;
            
            Registry.Public("123", 0);
            Registry.Public("1234", 1);

            var actualValue0 = Registry.GetValue<string>(0);
            var actualValue1 = Registry.GetValue<string>(1);
            
            Assert.IsTrue(actualValue0 == "123");
            Assert.IsTrue(actualValue1 == "1234");
        }
        
        [Test]
        public void Test3_1_1()
        {
            var actualValue = false;
            
            Registry.Public("1263");
            // Registry.Public("1234");
            
            Registry.Public("123", 0);
            Registry.Public("1234", 1);

            var actualValue0 = Registry.GetValue<string>(0);
            var actualValue1 = Registry.GetValue<string>(1);
            
            Assert.IsTrue(actualValue0 == "123");
            Assert.IsTrue(actualValue1 == "1234");
        }
        
        [Test]
        public void Test3_2()
        {
            Registry.Public<int>(1, 0);
            Registry.Public<object>(1, 1);

            var actualValue0 = Registry.GetValue<int>(0);
            var actualValue1 = Registry.GetValue<object>(1);
            
            Assert.IsTrue(actualValue0 == (int)actualValue1);
        }
        
        [Test]
        public void Test3_3()
        {
            Registry.Public<int>(1, 0);
            Registry.Public<object>(1, 1);

            var actualValue0 = Registry.GetValue<int>(0);
            var actualValue1 = Registry.GetValue<object>(1);
            
            Assert.IsFalse(((object)actualValue0) == actualValue1);
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