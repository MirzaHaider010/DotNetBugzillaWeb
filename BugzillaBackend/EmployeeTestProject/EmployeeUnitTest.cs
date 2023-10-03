using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;



namespace EmployeeTestProject
{
    public class EmployeeUnitTest : IClassFixture<WebApplicationFactory<YourStartupClass>>
    {
        [Fact]
        public void Test1()
        {

        }
    }
}