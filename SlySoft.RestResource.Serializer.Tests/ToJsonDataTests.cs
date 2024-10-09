using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SlySoft.RestResource.Serializers;
using TestUtils;

#pragma warning disable IDE0037

// ReSharper disable RedundantAnonymousTypePropertyName

namespace SlySoft.RestResource.Serializer.Tests;

[TestClass]
public sealed class ToJsonDataTests {
    private class StringIntResource : Resource {
        public string StringValue { get; set; } = "";
        public int IntValue { get; set; }
    }
    
    [TestMethod]
    public void DataValuesMustBeConvertedToJson() {
        //arrange
        var stringValue = GenerateRandom.String();
        var intValue = GenerateRandom.Int();

        var resource = new StringIntResource { StringValue = stringValue, IntValue = intValue };

        //act
        var json = resource.ToSlySoftHalJson();

        //assert
        var expected = new {
            stringValue = stringValue,
            intValue = intValue
        };

        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }
    
    private class TestObjectResource : Resource {
        public TestObject TestObject { get; set; } = new TestObject();
    }

    [TestMethod]
    public void ObjectDataMustBeConvertedToJson() {
        //arrange
        var testObject = new TestObject();

        var resource = new TestObjectResource{TestObject = testObject};

        //act
        var json = resource.ToSlySoftHalJson();

        //assert
        var expected = new {
            testObject = new { 
                stringValue = testObject.StringValue,
                intValue = testObject.IntValue,
            }
        }; 
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }

    private class StringsResource : Resource {
        public List<string> Strings { get; set; } = new();
    }
    
    [TestMethod]
    public void ArrayDataValuesMustBeConvertedToJson() {
        //arrange
        var strings = new List<string> {
            GenerateRandom.String(),
            GenerateRandom.String(),
            GenerateRandom.String()
        };

        var resource = new StringsResource { Strings = strings };

        //act
        var json = resource.ToSlySoftHalJson();

        //assert
        var expected = new {
            strings = new[] { strings[0], strings[1], strings[2] }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);

        Assert.AreEqual(expectedJson, json);
    }

    private class TestDataObjectsResource : Resource {
        public List<TestObject> DataObjects { get; set; } = new();
    }
    
    [TestMethod]
    public void ArrayObjectsMustBeConvertedToJson() {
        //arrange
        var dataObjects = new List<TestObject> { new(), new(), new() };

        var resource = new TestDataObjectsResource { DataObjects = dataObjects };

        //act
        var json = resource.ToSlySoftHalJson();

        //assert
        var expected = new {
            dataObjects = new [] {
                new { stringValue = dataObjects[0].StringValue, intValue = dataObjects[0].IntValue },
                new { stringValue = dataObjects[1].StringValue, intValue = dataObjects[1].IntValue },
                new { stringValue = dataObjects[2].StringValue, intValue = dataObjects[2].IntValue },
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }
}