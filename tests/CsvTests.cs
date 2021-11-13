using Microsoft.Extensions.Configuration;
using shopper.Models;
using shopper.Data;
using Xunit;
using Shouldly;
using Bogus;
using Microsoft.Extensions.Options;
using shopper.tests.helpers;
using System.Collections.Generic;
using System.IO;

namespace shopper.tests
{
    public class CsvTests
    {
        private IOptionsSnapshot<Settings> _settings;
        private Faker _faker;
        private CsvStorage _csvStorage;

        public CsvTests()
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.Development.json")
                .Build();
            var settings = new Settings();
            config.GetSection("Settings").Bind(settings);
            _settings = new TestOptionsSnapshot<Settings>(settings);
            _faker = new Faker();
            _csvStorage = new CsvStorage(_settings.Value);
        }
        const string SEARCHTERM_TEST_FILE = "../../../Helpers/SearchCriteria.csv";
        const string SETTINGS_TEST_FILE = "../../../Helpers/Settings.csv";
        [Fact]
        public async void Creates_New_SearchCriteria_File_If_One_Doesnt_Exist()
        {
            if(File.Exists(SEARCHTERM_TEST_FILE))
                File.Delete(SEARCHTERM_TEST_FILE);

            List<Product> listOfProducts = await CsvStorage.GetProductListAsync(SEARCHTERM_TEST_FILE);
            listOfProducts.ShouldBeEmpty();

        }
        [Fact]
        public async void Creates_Settings_File_If_One_Doesnt_Exist()
        {
            if (File.Exists(SETTINGS_TEST_FILE))
                File.Delete(SETTINGS_TEST_FILE);

            Settings result = await _csvStorage.GetSettingsFromFile(SETTINGS_TEST_FILE);
            result.SearchTerm.ShouldContain("mountain");

        }
    }
}
