using System;
using System.Collections.Generic;
using System.Linq;
using System.Technology.BDD.Nunit;
using System.Text;
using NUnit.Framework;
using SMART.Core.Interfaces.Repository;
using SMART.Core.Metadata;
using SMART.Core.Services;
using SMART.IOC;
using SMART.Core.Interfaces;

namespace SMART.Test.Services {
    [TestFixture]
    public class Repository_get_all_descriptions  : ContextSpecification
    {
        IRepository repository;


        IEnumerable<ClassDescription> result;

        public override void Given()
        {
            repository = new StatisticsRepository();
        }

        public override void When()
        {
            result = repository.GetAll();
        }

        [Test]
        public void should_return_a_class_description()
        {
            result.Count().should_be_equal_to(2);
        }

        [Test]
        public void should_contain_a_type_that_can_be_created()
        {
            new StatisticsFactory().Create(result.First().Type).should_be_an_instance_of<IStatistic>();
        }
    }
}
