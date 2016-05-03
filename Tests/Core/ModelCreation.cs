using System;
using NUnit.Framework;
using SMART.Core.DomainModel;


namespace SMART.Test.Core
{
    [TestFixture]
    public class ModelCreation
    {

        Model model;

        [SetUp]
        public void setup()
        {
            model = new Model();

        }
        [Test]
        public void should_create_a_new_model_that_is_fully_initiated()
        {

            Assert.AreEqual(string.Empty, model.Name);
            Assert.IsNotNull(model.Id);

            Assert.IsNotNull(model.StartState);
            Assert.IsNotNull(model.StopState);
        }

        [Test]
        public void should_correctly_set_name_and_id_from_cotr()
        {
            Guid id = Guid.NewGuid();
            string name = "name";

            model = new Model(name, id);
            Assert.AreEqual(name, model.Name);
            Assert.AreEqual(id, model.Id);

        }

        [Test]
        public void should_init_states_list_and_transition_list()
        {
            Assert.IsNotNull(model.Transitions);
            Assert.IsNotNull(model.States);
            Assert.AreEqual(0, model.Transitions.Count);
            Assert.AreEqual(2, model.States.Count);
        }

        [Test]
        public void propertychanged_fired_when_setting_name_and_id()
        {
            // Assign
            Guid id = Guid.NewGuid();
            string name = "name";

            bool firedid = false;
            bool firedname = false;
            model.PropertyChanged += (s, e) =>
                                         {
                                             if (e.PropertyName.Equals("Id")) firedid = true;
                                             if (e.PropertyName.Equals("Name")) firedname = true;

                                         };

            // Act
            model.Id = id;
            model.Name = name;
            // Assert
            Assert.True(firedid & firedname);
        }
    }
}