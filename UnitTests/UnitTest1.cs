using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViewModel;
using Model;
using View;
using System.Windows.Input;
using System.Collections;
namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AddItem()
        {
            VM vm = new VM(new TestUI());
            int n = vm.OMD.Count;
            vm.MD.NodesAmount = "10";
            vm.MD.P = "0,3";
            Assert.IsTrue(vm.Add.CanExecute(null));
            vm.Add.Execute(null);
            Assert.AreEqual(n + 1, vm.OMD.Count);
        }
        [TestMethod]
        public void AddWrongItem1()
        {
            VM vm = new VM(new TestUI());
            int n = vm.OMD.Count;
            vm.MD.NodesAmount = "10";
            vm.MD.P = "-1";
            Assert.IsFalse(vm.Add.CanExecute(null));
        }
        [TestMethod]
        public void AddWrongItem2()
        {
            VM vm = new VM(new TestUI());
            int n = vm.OMD.Count;
            vm.MD.NodesAmount = "10";
            vm.MD.P = "3.3"; //comma required
            Assert.IsFalse(vm.Add.CanExecute(null));
        }
        [TestMethod]
        public void AddWrongItem3()
        {
            VM vm = new VM(new TestUI());
            int n = vm.OMD.Count;
            vm.MD.NodesAmount = "1";
            vm.MD.P = "3";
            Assert.IsFalse(vm.Add.CanExecute(null));
        }
        [TestMethod]
        public void Serialization() //whole process
        {
            VM vm = new VM(new TestUI());
            vm.MD.NodesAmount = "10";
            vm.MD.P = "3";
            Assert.IsTrue(vm.Add.CanExecute(null));
            vm.Add.Execute(null);
            int k = vm.OMD.Count;
            Assert.IsTrue(vm.Save.CanExecute(null));
            vm.Save.Execute(null);
            Assert.IsTrue(vm.New.CanExecute(null));
            vm.New.Execute(null);
            int n = vm.OMD.Count;
            Assert.AreNotEqual(k, n);
            Assert.IsTrue(vm.Open.CanExecute(null));
            vm.Open.Execute(null);
            n = vm.OMD.Count;
            Assert.AreEqual(k, n);
        }
        [TestMethod]
        public void Remove()
        {
            VM vm = new VM(new TestUI());
            int n = vm.OMD.Count;
            vm.SelectedIndex = -1;
            Assert.IsFalse(vm.Remove.CanExecute(null));
            Assert.AreNotEqual(vm.OMD.Count, 0);
            vm.SelectedIndex = 0;
            Assert.IsTrue(vm.Remove.CanExecute(null));
            vm.Remove.Execute(null);
            Assert.AreNotEqual(n, vm.OMD.Count);
        }
        [TestMethod]
        public void New()
        {
            VM vm = new VM(new TestUI());
            int n = vm.OMD.Count;
            Assert.AreNotEqual(n, 0);
            Assert.IsTrue(vm.New.CanExecute(null));
            vm.New.Execute(null);
            Assert.AreEqual(vm.OMD.Count, 0);
        }
    }
    

    class TestUI : IServices
    {
        public bool UIChooseOpenLoc(ref string str)
        {
            str = AppDomain.CurrentDomain.BaseDirectory + "\\lalala";
            return true;
        }

        public bool UIChooseSaveLoc(ref string str)
        {
            str = AppDomain.CurrentDomain.BaseDirectory + "\\lalala";
            return true;
        }

        public void UICustomMessage(string header, string body)
        {
            throw new NotImplementedException();
        }

        public bool UISaveMessage()
        {
            return false;
        }
    }
}
