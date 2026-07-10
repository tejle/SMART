using NUnit.Framework;
using SMART.Gui.ViewModel;
using System;

namespace SMART.Test.Gui
{   
    public class DeriveBase : ViewModelBase
    {
        public DeriveBase() : base()
        {

        }

        public override string Icon
        {
            get { throw new System.NotImplementedException(); }
        }

        public override Guid Id
        {
            get;
            set;
        }
    }

    [TestFixture]
    public class ViewModelBaseTests
    {
        private DeriveBase derive;

        [SetUp]
        public void Setup()
        {
            derive = new DeriveBase();
        }

        [Test]
        public void create_view_model_base_object_and_assert_empty_Name()
        {
            Assert.IsEmpty(derive.Name);
        }


    }


}
