using System;
using System.Collections.Generic;
using Disaster_Alleviation.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;

namespace TestProject1
{
    public class MonetaryDonationPageTests
    {
        private readonly string _connectionString = "Server=tcp:djadmin2.database.windows.net,1433;Initial Catalog=DjPromoDatabase;Persist Security Info=False;User ID=djadmin;Password=Enclogspean@24;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        [Test]
        public void OnPost_InsertsDataIntoDatabase()
        {
            // Arrange
            var request = new Mock<HttpRequest>();
            var pageModel = new monetaryDonationModel(request.Object);

            var monetaryDate = DateTime.Now.ToShortDateString();
            var amount = "100";
            var anonymous = "John Doe";

            // Mock Request.Form
            var formValues = new Dictionary<string, StringValues>
            {
                { "monetarydate", new StringValues(monetaryDate) },
                { "amount", new StringValues(amount) },
                { "anonymous", new StringValues(anonymous) }
            };
            request.Setup(req => req.Form).Returns(new FormCollection(formValues));

            // Act
            pageModel.OnPost();

            // Assert
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand($"SELECT COUNT(*) FROM monetary WHERE monetarydate = '{monetaryDate}' AND amount = '{amount}' AND anonymous = '{anonymous}'", connection);
                var result = Convert.ToInt32(command.ExecuteScalar());

                Assert.AreEqual(1, result);
            }
        }

        [Test]
        public void GetMonetaryData_ReturnsListOfData()
        {
            // Arrange
            var pageModel = new monetaryDonationModel();

            // Act
            var monetaryData = pageModel.GetMonetaryData();

            // Assert
            Assert.IsInstanceOf<List<string[]>>(monetaryData);
        }
    }
}
