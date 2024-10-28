using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlySoft.RestResource.Client;
using TestUtils;

namespace ClientTest;

[TestClass]
public class ClientTests {
    private readonly RestClient _restClient = new("http://localhost:35093");

    [TestMethod]
    public void MustGetApplication() {
        //act
        var application = _restClient.Get<IApplicationResource>();

        //assert
        Assert.AreEqual("This is a test web service for demonstrating how to use Slysoft.RestResource and related libraries.", application.Information);
    }

    [TestMethod]
    public async Task MustGetApplicationAsync() {
        //act
        var application = await _restClient.GetAsync<IApplicationResource>();

        //assert
        Assert.AreEqual("This is a test web service for demonstrating how to use Slysoft.RestResource and related libraries.", application.Information);
    }

    [TestMethod]
    public async Task MustGetLinks() {
        //arrange
        var application = await _restClient.GetAsync<IApplicationResource>();

        //act
        var tests = await application.GetTests();

        //assert
        Assert.AreEqual("Tests used by the ClientTest app.", tests.Description);
    }

    [TestMethod]
    public async Task MustReturnNotFoundException() {
        //arrange
        var application = await _restClient.GetAsync<IApplicationResource>();
        var tests = await application.GetTests();

        //act
        try {
            await tests.NotFound();

        //Assert
            Assert.Fail("ResponseErrorCodeException exception not thrown");
        } catch (ResponseErrorCodeException e) {
            Assert.AreEqual("Resource not found.", e.Message);
            Assert.AreEqual(HttpStatusCode.NotFound, e.StatusCode);
        }
    }

    [TestMethod]
    public async Task MustGetNonResourceText() {
        //arrange
        var application = await _restClient.GetAsync<IApplicationResource>();
        var tests = await application.GetTests();

        //act
        var text = await tests.Text();

        //assert
        Assert.AreEqual("Non-Resource text.", text);
    }

    [TestMethod]
    public async Task MustPassQueryParameters() {
        //arrange
        var application = await _restClient.GetAsync<IApplicationResource>();
        var tests = await application.GetTests();
        var parameter1 = GenerateRandom.String();
        var parameter2 = GenerateRandom.String();

        //act
        var queryResult = await tests.Query(parameter1, parameter2);

        //assert
        Assert.AreEqual(parameter1, queryResult.Parameter1);
        Assert.AreEqual(parameter2, queryResult.Parameter2);
    }

    [TestMethod]
    public async Task MustBeAbleToPost() {
        //arrange
        var application = await _restClient.GetAsync<IApplicationResource>();
        var tests = await application.GetTests();
        var parameter1 = GenerateRandom.String();
        var parameter2 = GenerateRandom.String();

        //act
        var postResult = await tests.Post(parameter1, parameter2);

        //assert
        Assert.AreEqual(parameter1, postResult.Parameter1);
        Assert.AreEqual(parameter2, postResult.Parameter2);
    }

    [TestMethod]
    public async Task MustBeAbleToGetAList() {
        //arrange
        var application = await _restClient.GetAsync<IApplicationResource>();
        var tests = await application.GetTests();
        var list = new List<string> { GenerateRandom.String(), GenerateRandom.String(), GenerateRandom.String( )};

        //act
        var listResult = await tests.List(list);

        //assert
        Assert.AreEqual(list[0], listResult.List[0]);
        Assert.AreEqual(list[1], listResult.List[1]);
        Assert.AreEqual(list[2], listResult.List[2]);
    }

    [TestMethod]
    public async Task MustBeAbleToManuallyCallPost() {
        //arrange
        var application = await _restClient.GetAsync<IApplicationResource>();
        var tests = await application.GetTests();
        var postLink = tests.Resource.Links.FirstOrDefault(link => link.Verb == "POST");
        if (postLink == null) {
            throw new Exception("No POST link found");
        }

        var fields = postLink.Parameters.ToDictionary<LinkParameter?, string, object?>(parameter => parameter.Name, _ => GenerateRandom.String());

        //act
        var postResult = await tests.CallRestLinkAsync<ClientResource>(postLink.Name, fields);

        //assert
        foreach (var field in fields) {
            Assert.AreEqual(field.Value, postResult.Data[field.Key]);
        }
    }

    [TestMethod]
    public async Task MustBeAbleToPatchDateTime() {
        //arrange
        var application = await _restClient.GetAsync<IApplicationResource>();
        var tests = await application.GetTests();
        var testDateTime = DateTime.Today.AddDays(-1);

        //act
        var dateTimeResult = await tests.DateTime(testDateTime);

        //assert
        Assert.AreEqual(testDateTime, dateTimeResult.Value);
    }
}