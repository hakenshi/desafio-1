using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HypeSoft.Application.DTOs;

namespace HypeSoft.IntegrationTests.Controllers;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/products?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
        products.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_ValidProduct_ShouldReturnCreated()
    {
        // Arrange
        var newProduct = new CreateProductDto(
            "Integration Test Product",
            "Test Description",
            99.99m,
            await GetOrCreateCategoryId(),
            50
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", newProduct);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProduct.Should().NotBeNull();
        createdProduct!.Name.Should().Be("Integration Test Product");
        createdProduct.Price.Should().Be(99.99m);
    }

    [Fact]
    public async Task GetById_ExistingProduct_ShouldReturnOk()
    {
        // Arrange
        var productId = await CreateTestProduct();

        // Act
        var response = await _client.GetAsync($"/api/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        product.Should().NotBeNull();
        product!.Id.Should().Be(productId);
    }

    [Fact]
    public async Task GetById_NonExistingProduct_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/products/non-existing-id");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ExistingProduct_ShouldReturnOk()
    {
        // Arrange
        var productId = await CreateTestProduct();
        var updateDto = new UpdateProductDto(
            "Updated Name",
            "Updated Description",
            199.99m,
            await GetOrCreateCategoryId(),
            100
        );

        // Act
        var response = await _client.PutAsJsonAsync($"/api/products/{productId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        updatedProduct.Should().NotBeNull();
        updatedProduct!.Name.Should().Be("Updated Name");
        updatedProduct.Price.Should().Be(199.99m);
    }

    [Fact]
    public async Task Delete_ExistingProduct_ShouldReturnNoContent()
    {
        // Arrange
        var productId = await CreateTestProduct();

        // Act
        var response = await _client.DeleteAsync($"/api/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/products/{productId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Search_ByName_ShouldReturnMatchingProducts()
    {
        // Arrange
        await CreateTestProduct("Searchable Product");

        // Act
        var response = await _client.GetAsync("/api/products/search?name=Searchable");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
        products.Should().NotBeNull();
        products.Should().Contain(p => p.Name.Contains("Searchable"));
    }

    [Fact]
    public async Task GetLowStock_ShouldReturnProductsWithLowStock()
    {
        // Arrange
        var categoryId = await GetOrCreateCategoryId();
        var lowStockProduct = new CreateProductDto("Low Stock", "Desc", 10m, categoryId, 5);
        await _client.PostAsJsonAsync("/api/products", lowStockProduct);

        // Act
        var response = await _client.GetAsync("/api/products/low-stock");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
        products.Should().NotBeNull();
        products.Should().Contain(p => p.IsLowStock);
    }

    private async Task<string> CreateTestProduct(string name = "Test Product")
    {
        var categoryId = await GetOrCreateCategoryId();
        var product = new CreateProductDto(name, "Test Description", 50m, categoryId, 20);
        var response = await _client.PostAsJsonAsync("/api/products", product);
        var created = await response.Content.ReadFromJsonAsync<ProductDto>();
        return created!.Id;
    }

    private async Task<string> GetOrCreateCategoryId()
    {
        var categoriesResponse = await _client.GetAsync("/api/categories");
        var categories = await categoriesResponse.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>();
        
        if (categories?.Any() == true)
        {
            return categories.First().Id;
        }

        var newCategory = new CreateCategoryDto("Test Category", "Test Description");
        var response = await _client.PostAsJsonAsync("/api/categories", newCategory);
        var created = await response.Content.ReadFromJsonAsync<CategoryDto>();
        return created!.Id;
    }
}
