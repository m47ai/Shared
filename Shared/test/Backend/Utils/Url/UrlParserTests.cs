namespace M47.Shared.Tests.Utils.Url;

using M47.Shared.Utils.Url;

public sealed class UrlParserTests
{
    [Theory]
    [InlineData("mermaid-js.github.io", "https://mermaid-js.github.io/mermaid/#/./usage")]
    [InlineData("bbcchildreninneed.co.uk", "https://www.bbcchildreninneed.co.uk/schools/")]
    [InlineData("marca.com", "https://www.marca.com/futbol/real-madrid/2022/03/02/621f1e65ca474191438b4629.html")]
    [InlineData("comunicaciontucuman.gob.ar", "https://www.comunicaciontucuman.gob.ar/noticia/interior/209510/invertiran-100-millones-obras-para-interior-tucumano")]
    [InlineData("propertycouncil.com.au", "https://campaign.propertycouncil.com.au/8waystoactivate")]
    [InlineData("mexicotravelchannel.com.mx", "https://mexicotravelchannel.com.mx/mundo/20220224/rusia-y-ucrania-empresas-mexicanas-peligro-economico-conflico/")]
    [InlineData("hola.com", "https://hola.com")]
    [InlineData("hola.com", "hola.com")]
    public void Should_GetHostFRomUrl_When_UrlProvided(string expected, string url)
    {
        // Act
        var actual = UrlParser.GetMainDomain(url);

        // Assert
        actual.Should().Be(expected);
    }
}