using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class AutoValuesForSubs
    {
        private ISample _sample;

        public interface ISample
        {
            int[] GetNumbers();
            IEnumerable<object> GetObjects();
            string Name { get; set; }
            List<string> ListOfStrings { get; set; }
            int? GetNullableNumber();
        }

        [SetUp]
        public void SetUp()
        {
            _sample = Substitute.For<ISample>();
        }

        [Test]
        public void Should_auto_return_empty_array()
        {
            Assert.That(_sample.GetNumbers().Length, Is.EqualTo(0));
        }

        [Test]
        public void Should_auto_return_empty_enumerable()
        {
            Assert.That(_sample.GetObjects(), Is.Not.Null);
            Assert.That(_sample.GetObjects().Count(), Is.EqualTo(0));
        }

        [Test]
        [Pending]
        public void Should_auto_return_empty_string()
        {
            Assert.That(_sample.Name.Length, Is.EqualTo(0)); 
        }

        [Test]
        [Pending]
        public void Should_auto_return_empty_string_list()
        {
            Assert.That(_sample.ListOfStrings, Is.Not.Null);
            Assert.That(_sample.ListOfStrings.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Should_return_null_for_nullables()
        {
            Assert.That(_sample.GetNullableNumber(), Is.Null); 
        }

        [Test]
        public void Should_return_same_empty_value_for_auto_values_for_reference_types()
        {
            var autoArrayValue = _sample.GetNumbers();
            Assert.That(_sample.GetNumbers(), Is.SameAs(autoArrayValue));
        }
    }
}