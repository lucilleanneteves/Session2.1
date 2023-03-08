using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

[assembly: Parallelize(Workers = 10, Scope = ExecutionScope.MethodLevel)]

namespace HomeworkSession2._1_LT
{
    [TestClass]
    public class HttpClientTest
    {
        private static HttpClient httpClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string PetsEndpoint = "pet";

        private static string GetURL(string enpoint) => $"{BaseURL}{enpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetModel> cleanUpList = new List<PetModel>();

        [TestInitialize]
        public void TestInitialize()
        {
            httpClient = new HttpClient();
        }

        [TestCleanup]
        public async Task TestCleanUp()
        {
            foreach (var data in cleanUpList)
            {
                var httpResponse = await httpClient.DeleteAsync(GetURL($"{PetsEndpoint}/{data.Id}"));
            }
        }

        [TestMethod]
        public async Task PutPetMethod()
        {
            #region create data

            // Create Json Object
            Tags petTag1 = new Tags()
            {
                Id = 30,
                Name = "Tag1"
            };
            Tags petTag2 = new Tags()
            {
                Id = 31,
                Name = "Tag2"
            };

            PetModel petData = new PetModel()
            {
                Id = 40,
                PetCategory = new Category()
                {
                    Id = 98,
                    Name = "Chihuahua"
                },
                Name = "Choco",
                PhotoUrls = new List<string> { "photoUrl1", "photoUrl2" },
                PetTags = new List<Tags> { petTag1, petTag2 },
                Status = "available",
            };

            // Serialize Content
            var request = JsonConvert.SerializeObject(petData);
            var postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            // Send Post Request
            await httpClient.PostAsync(GetURL(PetsEndpoint), postRequest);

            #endregion

            #region get Username of the created data

            // Get Request
            var getResponse = await httpClient.GetAsync(GetURI($"{PetsEndpoint}/{petData.Id}"));

            // Deserialize Content
            var listPetData = JsonConvert.DeserializeObject<PetModel>(getResponse.Content.ReadAsStringAsync().Result);

            // filter created data
            var createdPetData = listPetData;

            #endregion

            #region send put request to update data

            Tags updatedPetTag1 = new Tags()
            {
                Id = 30,
                Name = "updatedTag1"
            };
            Tags updatedPetTag2 = new Tags()
            {
                Id = 31,
                Name = "updatedTag2"
            };

            petData = new PetModel()
            {
                Id = listPetData.Id,
                PetCategory = new Category()
                {
                    Id = 99,
                    Name = "Shih-tzu"
                },
                Name = "updatedNameSinag",
                PhotoUrls = new List<string> { "updatedPhotoUrl1", "UpdatedPhotoUrl2" },
                PetTags = new List<Tags> { updatedPetTag1, updatedPetTag2 },
                Status = "sold"
            };


            // Serialize Content
            request = JsonConvert.SerializeObject(petData);
            postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            // Send Put Request
            var httpResponse = await httpClient.PutAsync(GetURL($"{PetsEndpoint}"), postRequest);

            // Get Status Code
            var statusCode = httpResponse.StatusCode;

            #endregion

            #region get updated data

            // Get Request
            getResponse = await httpClient.GetAsync(GetURI($"{PetsEndpoint}/{petData.Id}"));

            // Deserialize Content
            listPetData = JsonConvert.DeserializeObject<PetModel>(getResponse.Content.ReadAsStringAsync().Result);

            // filter created data
            createdPetData = listPetData;

            #endregion

            #region cleanup data

            // Add data to cleanup list
            cleanUpList.Add(listPetData);

            #endregion

            #region assertion
            // Assertion
            Assert.AreEqual(HttpStatusCode.OK, statusCode, "Status code is not equal to 201");
            Assert.AreEqual(petData.Id, listPetData.Id, "Id does not match");
            Assert.AreEqual(petData.Name, listPetData.Name, "Name does not match");
            Assert.AreEqual(petData.Status, listPetData.Status, "Status does not match");
            Assert.AreEqual(petData.PetCategory.Id, listPetData.PetCategory.Id, "Pet Category Id does not match");
            Assert.AreEqual(petData.PetCategory.Name, listPetData.PetCategory.Name, "Pet Category Name does not match");
            #endregion
        }

        [TestMethod]
        public async Task DeletePetMethod()
        {
            #region create data

            // Create Json Object
            Category category = new Category();
            category.Id = 78;
            category.Name = "Category1";

            Tags petTag1 = new Tags()
            {
                Id = 25,
                Name = "Tag1"
            };
            Tags petTag2 = new Tags()
            {
                Id = 26,
                Name = "Tag2"
            };
            PetModel petData = new PetModel()
            {
                Id = 78,
                PetCategory = new Category()
                {
                    Id = 98,
                    Name = "Cat"
                },
                Name = "Marites",
                PhotoUrls = new List<string> { "url3", "url4" },
                PetTags = new List<Tags> { petTag1, petTag2 },
                Status = "available",
            };
            // Serialize Content
            var request = JsonConvert.SerializeObject(petData);
            var postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            // Send Post Request
            await httpClient.PostAsync(GetURL(PetsEndpoint), postRequest);

            #endregion

            #region get Username of the created data

            // Get Request
            var getResponse = await httpClient.GetAsync(GetURI($"{PetsEndpoint}/{petData.Id}"));

            // Deserialize Content
            var listPetData = JsonConvert.DeserializeObject<PetModel>(getResponse.Content.ReadAsStringAsync().Result);

            // filter created data
            var createdPetDataId = listPetData.Id;

            #endregion

            #region send delete request

            // Send Delete Request
            var httpResponse = await httpClient.DeleteAsync(GetURL($"{PetsEndpoint}/{createdPetDataId}"));

            // Get Status Code
            var statusCode = httpResponse.StatusCode;

            #endregion

            #region assertion

            // Assertion
            Assert.AreEqual(HttpStatusCode.OK, statusCode, "Status code is not equal to 201");

            #endregion
        }

    }
}