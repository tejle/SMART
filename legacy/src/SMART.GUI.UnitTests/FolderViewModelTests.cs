using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SMART.Gui.ViewModel;

namespace SMART.GUI.UnitTests
{
    [TestFixture]
    public class FolderViewModelTests
    {
        [Test]
        public void create_a_folder_view_model()
        {
            Func<string> name = () => "TEST";
            FolderViewModel vm = new FolderViewModel(name);
            Assert.AreEqual(name(), vm.Name);
        }
    }
}

