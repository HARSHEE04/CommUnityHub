using Xunit;
using CsvHelper.Configuration;
using CommUnityHub.Services;
using CommUnityHub.Models;

namespace CommUnityHub.Tests
{
    public class ResourcesCSVMapTests
    {
        [Fact]
        public void CSVMap_MapsAllExpectedColumns()
        {
            // Arrange
            var map = new ResourceCSVMap();

            // Act
            var mappings = map.MemberMaps;

            // Assert (single behavior check)
            Assert.Equal(9, mappings.Count);
            Assert.Contains(mappings, m => m.Data.Member.Name == "AgencyName");
            Assert.Contains(mappings, m => m.Data.Member.Name == "DescriptionService");
        }

        [Fact]
        public void CSVMap_HasCorrectColumnNames()
        {
            // Arrange
            var map = new ResourceCSVMap();

            // Act
            var columnNames = map.MemberMaps.Select(m => m.Data.Names.First()).ToList();

            // Assert
            Assert.Contains("AgencyName", columnNames);
            Assert.Contains("OfficePhone", columnNames);
            Assert.DoesNotContain("RandomColumn", columnNames);
        }
    }

}

