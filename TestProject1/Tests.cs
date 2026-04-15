using System;
using DreamPlace.Lib.Rx;
using NUnit.Framework;

namespace TestProject1
{
    //ToDo нужно сделать тестов для дженерик типов
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

        // Remove с контрактом подтверждения

        [Test]
        public void Remove_ReturnsContractWithCorrectInfo()
        {
            Registry.Public("to_remove", 100);

            var contract = Registry.Remove<string>(100);

            Assert.IsNotNull(contract);
            Assert.AreEqual(1, contract.Count);
            Assert.AreEqual("to_remove", contract.Values[0]);
            Assert.IsFalse(contract.IsConfirmed);

            // Значение ещё в реестре — контракт не подтверждён
            Assert.AreEqual("to_remove", Registry.GetValue<string>(100));
        }

        [Test]
        public void Remove_ConfirmActuallyRemoves()
        {
            Registry.Public("will_die", 101);

            var contract = Registry.Remove<string>(101);
            contract.Confirm();

            Assert.IsTrue(contract.IsConfirmed);
            Assert.IsNull(Registry.GetValue<string>(101));
        }

        [Test]
        public void Remove_WithoutConfirm_ValueSurvives()
        {
            Registry.Public("survivor", 102);

            var contract = Registry.Remove<string>(102);
            // Не вызываем Confirm()

            Assert.AreEqual("survivor", Registry.GetValue<string>(102));
        }

        [Test]
        public void Remove_NonExistent_ReturnsNull()
        {
            var contract = Registry.Remove<string>(999);

            Assert.IsNull(contract);
        }

        [Test]
        public void Remove_DoubleConfirm_Throws()
        {
            Registry.Public("once", 103);

            var contract = Registry.Remove<string>(103);
            contract.Confirm();

            Assert.Throws<InvalidOperationException>(() => contract.Confirm());
        }

        [Test]
        public void Remove_ContractShowsSubscriberCount()
        {
            Registry.Subscribe<long>((e) => { }, 200);
            Registry.Subscribe<long>((e) => { }, 200);

            var contract = Registry.Remove<long>(200);

            Assert.IsNotNull(contract);
            Assert.AreEqual(2, contract.SubscriberCount);

            contract.Confirm();
        }

        // Clear с контрактом подтверждения

        [Test]
        public void Clear_ReturnsContractWithAllElements()
        {
            Registry.Public<double>(1.0, 300);
            Registry.Public<double>(2.0, 301);
            Registry.Public<double>(3.0, 302);

            var contract = Registry.Clear<double>();

            Assert.IsNotNull(contract);
            Assert.IsTrue(contract.Count >= 3);
            Assert.IsFalse(contract.IsConfirmed);
        }

        [Test]
        public void Clear_ConfirmRemovesAll()
        {
            Registry.Public<float>(1.0f, 400);
            Registry.Public<float>(2.0f, 401);

            var contract = Registry.Clear<float>();
            contract.Confirm();

            Assert.AreEqual(default(float), Registry.GetValue<float>(400));
            Assert.AreEqual(default(float), Registry.GetValue<float>(401));
        }

        // RegistryScope

        [Test]
        public void Scope_ValuesAvailableDuringScope()
        {
            using (var scope = Registry.CreateScope())
            {
                scope.Public<string>("scoped_value", 500);

                Assert.AreEqual("scoped_value", Registry.GetValue<string>(500));
            }
        }

        [Test]
        public void Scope_ValuesRemovedAfterDispose()
        {
            using (var scope = Registry.CreateScope())
            {
                scope.Public<string>("temporary", 501);
            }

            Assert.IsNull(Registry.GetValue<string>(501));
        }

        [Test]
        public void Scope_MultipleValues_AllRemovedAfterDispose()
        {
            using (var scope = Registry.CreateScope())
            {
                scope.Public<int>(10, 600);
                scope.Public<int>(20, 601);
                scope.Public<string>("temp", 602);
            }

            Assert.AreEqual(default(int), Registry.GetValue<int>(600));
            Assert.AreEqual(default(int), Registry.GetValue<int>(601));
            Assert.IsNull(Registry.GetValue<string>(602));
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