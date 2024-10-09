using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlySoft.RestResource.Serializers;
using TestUtils;

namespace SlySoft.RestResource.Serializer.Tests;

[TestClass]
public sealed class ToXmlDataTests {
    private const string XmlHeader = "<?xml version=\"1.0\" encoding=\"utf-16\"?>";

    private class StringIntResource : Resource {
        public string StringValue { get; set; } = "";
        public int IntValue { get; set; }
    }

    [TestMethod]
    public void DataValuesMustBeConvertedToXml() {
        //arrange
        var stringValue = GenerateRandom.String();
        var intValue = GenerateRandom.Int();

        var resource = new StringIntResource { StringValue = stringValue, IntValue = intValue };

        //assert
        var xml = resource.ToSlySoftHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource rel=\"self\"><stringValue>{stringValue}</stringValue><intValue>{intValue}</intValue></resource>";
        Assert.AreEqual(expectedXml, xml);
    }

    private class TestObjectResource : Resource {
        public TestObject TestObject { get; set; } = new TestObject();
    }
    
    [TestMethod]
    public void ObjectDataMustBeConvertedToXml() {
        //arrange
        var testObject = new TestObject();

        var resource = new TestObjectResource{TestObject = testObject};

        //act
        var xml = resource.ToSlySoftHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource rel=\"self\"><testObject><stringValue>{testObject.StringValue}</stringValue><intValue>{testObject.IntValue}</intValue></testObject></resource>";
        Assert.AreEqual(expectedXml, xml);
    }

    private class StringsResource : Resource {
        public List<string> Strings { get; set; } = new();
    }
    
    [TestMethod]
    public void ArrayDataValuesMustBeConvertedToXml() {
        //arrange
        var strings = new List<string> {
            GenerateRandom.String(),
            GenerateRandom.String(),
            GenerateRandom.String()
        };

        var resource = new StringsResource { Strings = strings };

        //act
        var xml = resource.ToSlySoftHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource rel=\"self\"><strings><value>{strings[0]}</value><value>{strings[1]}</value><value>{strings[2]}</value></strings></resource>";
        Assert.AreEqual(expectedXml, xml);
    }

    private class TestDataObjectsResource : Resource {
        public List<TestObject> DataObjects { get; set; } = new();
    }
    
    [TestMethod]
    public void ArrayObjectsMustBeConvertedToXml() {
        //arrange
        var dataObjects = new List<TestObject> { new(), new(), new() };

        var resource = new TestDataObjectsResource { DataObjects = dataObjects };

        //assert
        var xml = resource.ToSlySoftHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource rel=\"self\"><dataObjects><value><stringValue>{dataObjects[0].StringValue}</stringValue><intValue>{dataObjects[0].IntValue}</intValue></value><value><stringValue>{dataObjects[1].StringValue}</stringValue><intValue>{dataObjects[1].IntValue}</intValue></value><value><stringValue>{dataObjects[2].StringValue}</stringValue><intValue>{dataObjects[2].IntValue}</intValue></value></dataObjects></resource>";
        Assert.AreEqual(expectedXml, xml);
    }
}