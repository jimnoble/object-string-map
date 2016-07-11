using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectStringMap.Implementation;

namespace ObjectStringMap.Tests
{
    [TestClass]
    public class StringMapTests
    {
        const string TestString = "quick-brown-fox";

        const string DateFormat = "yyyy/MM/dd";

        readonly DateTime TestDate = new DateTime(
            2016, 7, 9, 
            0, 0, 0, 
            DateTimeKind.Utc);

        // TODO: caching

        // DONE: this, Guid, DateTime, int and friends, immutable, string, Nullable<>

        [TestMethod]
        [TestCategory("Unit")]
        public void MapObject_WhenString_ThenSuccess()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                TestString,
                TestString);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapString_WhenString_ThenSuccess()
        {
            MapString_WhenSimpleType_ThenSuccess(
                TestString,
                TestString);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapObject_WhenDateTime_ThenSuccess()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                TestDate, 
                TestDate.ToString(DateFormat), 
                DateFormat);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapString_WhenDateTime_ThenSuccess()
        {
            MapString_WhenSimpleType_ThenSuccess(
                TestDate.ToString(DateFormat), 
                TestDate,
                DateFormat);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapObject_WhenInt_ThenSuccess()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                int.MaxValue, 
                int.MaxValue.ToString());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapString_WhenInt_ThenSuccess()
        {
            MapString_WhenSimpleType_ThenSuccess(
                int.MaxValue.ToString(),
                int.MaxValue);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapObject_WhenNullableInt_ThenSuccess()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                (int?)int.MaxValue,
                int.MaxValue.ToString());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapString_WhenNullableInt_ThenSuccess()
        {
            MapString_WhenSimpleType_ThenSuccess(
                int.MaxValue.ToString(),
                (int?)int.MaxValue);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapObject_WhenLong_ThenSuccess()
        {
            MapObject_WhenSimpleType_ThenSuccess(
                long.MaxValue, 
                long.MaxValue.ToString());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapString_WhenLong_ThenSuccess()
        {
            MapString_WhenSimpleType_ThenSuccess(
                long.MaxValue.ToString(), 
                long.MaxValue);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapObject_WhenGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapObject_WhenSimpleType_ThenSuccess(guid, guid.ToString("N"));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapString_WhenGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapString_WhenSimpleType_ThenSuccess(guid.ToString("N"), guid);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapObject_WhenDashedGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapObject_WhenSimpleType_ThenSuccess(guid, guid.ToString("D"), "D");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapString_WhenDashedGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapString_WhenSimpleType_ThenSuccess(guid.ToString("D"), guid);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapObject_WhenWrappedGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapObject_WhenWrappedType_ThenSuccess(guid, guid.ToString("N"));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapString_WhenWrappedGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapString_WhenWrappedType_ThenSuccess(guid.ToString("N"), guid);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapObject_WhenWrappedDashedGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapObject_WhenWrappedType_ThenSuccess(guid, guid.ToString("D"), "D");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapString_WhenWrappedDashedGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapString_WhenWrappedType_ThenSuccess(guid.ToString("D"), guid, "D");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapObject_WhenImmutableGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapObject_WhenImmutableType_ThenSuccess(guid, guid.ToString("N"));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MapString_WhenImmutableGuid_ThenSuccess()
        {
            var guid = Guid.NewGuid();

            MapString_WhenImmutableType_ThenSuccess(guid.ToString("N"), guid);
        }

        static void MapObject_WhenSimpleType_ThenSuccess<TSimpleType>(
            TSimpleType value,
            string stringValue,
            string format = null)
        {
            var map = new StringMap<TSimpleType>(string.IsNullOrWhiteSpace(format) ?
                "alfa/{this}/bravo" :
                $"alfa/{{this:{format}}}/bravo");

            var str = map.Map(value);

            Assert.AreEqual(
                $"alfa/{stringValue}/bravo",
                str);

            Console.WriteLine($"map: {map}");

            Console.WriteLine($"str: {str}");
        }

        static void MapString_WhenSimpleType_ThenSuccess<TSimpleType>(
            string stringValue,
            TSimpleType value,
            string format = null)
        {
            var map = new StringMap<TSimpleType>(string.IsNullOrWhiteSpace(format) ?
                "alfa/{this}/bravo" :
                $"alfa/{{this:{format}}}/bravo");

            var str = $"alfa/{stringValue}/bravo";

            var obj = map.Map(str);

            Assert.AreEqual(
                value,
                obj);

            Console.WriteLine($"map: {map}");

            Console.WriteLine($"str: {str}");
        }

        static void MapObject_WhenWrappedType_ThenSuccess<TType>(
            TType value,
            string stringValue,
            string format = null)
        {
            var map = new StringMap<Wrapper<TType>>(string.IsNullOrWhiteSpace(format) ?
                "alfa/{Property}/bravo" :
                $"alfa/{{Property:{format}}}/bravo");

            var obj = new Wrapper<TType>
            {
                Property = value
            };

            var str = map.Map(obj);

            Console.WriteLine($"str is {str}");

            Assert.AreEqual(
                $"alfa/{stringValue}/bravo",
                str);

            Console.WriteLine($"map: {map}");

            Console.WriteLine($"str: {str}");
        }

        static void MapString_WhenWrappedType_ThenSuccess<TType>(
            string stringValue,
            TType value,
            string format = null)
        {
            var map = new StringMap<Wrapper<TType>>(string.IsNullOrWhiteSpace(format) ?
                "alfa/{Property}/bravo" :
                $"alfa/{{Property:{format}}}/bravo");

            var str = $"alfa/{stringValue}/bravo";

            var obj = map.Map(str);

            Assert.AreEqual(
                value,
                obj.Property);

            Console.WriteLine($"map: {map}");

            Console.WriteLine($"str: {str}");
        }

        static void MapObject_WhenImmutableType_ThenSuccess<TType>(
            TType value,
            string stringValue,
            string format = null)
        {
            var map = new StringMap<Immutable<TType>>(string.IsNullOrWhiteSpace(format) ?
                "alfa/{Property}/bravo" :
                $"alfa/{{Property:{format}}}/bravo");

            var obj = new Immutable<TType>(value);

            var str = map.Map(obj);

            Console.WriteLine($"str is {str}");

            Assert.AreEqual(
                $"alfa/{stringValue}/bravo",
                str);

            Console.WriteLine($"map: {map}");

            Console.WriteLine($"str: {str}");
        }

        static void MapString_WhenImmutableType_ThenSuccess<TType>(
            string stringValue,
            TType value,
            string format = null)
        {
            var map = new StringMap<Immutable<TType>>(string.IsNullOrWhiteSpace(format) ?
                "alfa/{Property}/bravo" :
                $"alfa/{{Property:{format}}}/bravo");

            var str = $"alfa/{stringValue}/bravo";

            var obj = map.Map(str);

            Assert.AreEqual(
                value,
                obj.Property);

            Console.WriteLine($"map: {map}");

            Console.WriteLine($"str: {str}");
        }

        class Wrapper<TType> 
        {
            public TType Property { get; set; }
        }

        class Immutable<TType>
        {
            public Immutable(TType property)
            {
                Property = property;
            }

            public TType Property { get; }
        }
    }
}
