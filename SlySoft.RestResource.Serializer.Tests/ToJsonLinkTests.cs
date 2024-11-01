using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SlySoft.RestResource.Serializers;
using TestUtils;

// ReSharper disable RedundantAnonymousTypePropertyName
#pragma warning disable IDE0037

namespace SlySoft.RestResource.Serializer.Tests;

[TestClass]
public class ToJsonLinkTests {
    [TestMethod]
    public void GetMustBeConvertedToLinkInJson() {
        //arrange
        var href = GenerateRandom.String();
        var resource = new Resource()
            .Get("getLink", href);

        //act
        var json = resource.ToJson();

        //assert
        var expected = new {
            _links = new {
                getLink = new {
                    href = href
                }
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void MultipleGetsMustBeConvertedToLinksInJson() {
        //arrange
        var href1 = GenerateRandom.String();
        var href2 = GenerateRandom.String();
        var resource = new Resource()
            .Get("getLink1", href1)
            .Get("getLink2", href2);

        //act
        var json = resource.ToJson();

        //assert
        var expected = new {
            _links = new {
                getLink1 = new {
                    href = href1
                },
                getLink2 = new {
                    href = href2
                },
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void GetMustIncludeTemplatedInJson() {
        //arrange
        var href = GenerateRandom.String();
        var resource = new Resource()
            .Get("getLink", href, templated: true);

        //act
        var json = resource.ToJson();

        //assert
        var expected = new {
            _links = new {
                getLink = new {
                    href = href,
                    templated = true
                }
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void GetMustIncludeTimeoutInJson() {
        //arrange
        var href = GenerateRandom.String();
        var resource = new Resource()
            .Get("getLink", href, timeout: 60);

        //act
        var json = resource.ToJson();

        //assert
        var expected = new {
            _links = new {
                getLink = new {
                    href = href,
                    timeout = 60
                }
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void QueryMustIncludeParametersInJson() {
        //arrange
        var href = GenerateRandom.String();
        var resource = new Resource()
            .Query<User>("getLink", href)
                .Parameter(x => x.FirstName)
                .Parameter(x => x.LastName)
            .EndQuery();


        //act
        var json = resource.ToJson();

        //assert
        var expected = new {
            _links = new {
                getLink = new {
                    href = href,
                    parameters = new {
                        firstName = new {},
                        lastName = new {}
                    }
                }
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void QueryMustIncludeParametersPropertiesInJson() {
        //arrange
        var defaultDate = DateTime.Now.AddDays(-1);
        var defaultDob = new DateOnly(2005, 03, 27);
        var defaultShiftStart = new TimeOnly(9, 30, 0);
        var href = GenerateRandom.String();
        var resource = new Resource()
            .Query<User>("getLink", href)
                .Parameter(x => x.Position, defaultValue: UserPosition.Admin, listOfValues: new []{ UserPosition.Standard, UserPosition.Admin })
                .Parameter(x => x.YearsEmployed, type: "number")
                .Parameter(x => x.DateCreated, defaultValue: defaultDate)
                .Parameter(x => x.DateOfBirth, defaultValue: defaultDob)
                .Parameter(x => x.ShiftStart, defaultValue: defaultShiftStart)
            .EndQuery();

        //act
        var json = resource.ToJson();

        //assert
        var expected = new {
            _links = new {
                getLink = new {
                    href = href,
                    parameters = new {
                        position = new { defaultValue = "Admin", listOfValues = new[]{"Standard", "Admin"}},
                        yearsEmployed = new { type = "number" },
                        dateCreated = new { defaultValue = defaultDate.ToString("s")},
                        dateOfBirth = new { defaultValue = defaultDob.ToString("yyyy-MM-dd")},
                        shiftStart = new { defaultValue = defaultShiftStart.ToString("HH:mm:ss")}
                    }
                }
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void PostLinkMustContainVerb() {
        //arrange
        var href = GenerateRandom.String();
        var resource = new Resource()
            .Post("postLink", href)
            .EndBody();

        //act
        var json = resource.ToJson();

        //assert
        var expected = new {
            _links = new {
                postLink = new {
                    href = href,
                    verb = "POST"
                }
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void PutLinkMustContainVerb() {
        //arrange
        var href = GenerateRandom.String();
        var resource = new Resource()
            .Put("putLink", href)
            .EndBody();

        //act
        var json = resource.ToJson();

        //assert
        var expected = new {
            _links = new {
                putLink = new {
                    href = href,
                    verb = "PUT"
                }
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void PatchLinkMustContainVerb() {
        //arrange
        var href = GenerateRandom.String();
        var resource = new Resource()
            .Patch("patchLink", href)
            .EndBody();

        //act
        var json = resource.ToJson();

        //assert
        var expected = new {
            _links = new {
                patchLink = new {
                    href = href,
                    verb = "PATCH"
                }
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void DeleteLinkMustContainVerb() {
        //arrange
        var href = GenerateRandom.String();
        var resource = new Resource()
            .Delete("deleteLink", href);

        //act
        var json = resource.ToJson();

        //assert
        var expected = new {
            _links = new {
                deleteLink = new {
                    href = href,
                    verb = "DELETE"
                }
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }
}