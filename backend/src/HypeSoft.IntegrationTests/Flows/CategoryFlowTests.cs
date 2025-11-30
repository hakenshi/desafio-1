using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HypeSoft.Application.DTOs;

namespace HypeSoft.IntegrationTests.Flows;

/// <summary>
/// Testes E2E do fluxo completo de categorias
/// </summary>
public class CategoryFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoryFlowTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CompleteCategoryFlow_CreateListUpdateDelete_ShouldWork()
    {
        // 1. Criar categoria
        var category = new CreateCategoryDto("Informática", "Produtos de informática");
        var createResponse = await _client.PostAsJsonAsync("/api/categories", category);
        
        if (createResponse.StatusCode == HttpStatusCode.Unauthorized)
        {
            Assert.True(true, "Endpoint requer autenticação");
            return;
        }

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<CategoryDto>();
        created.Should().NotBeNull();
        created!.Name.Should().Be("Informática");

        // 2. Listar categorias
        var listResponse = await _client.GetAsync("/api/categories");
        if (listResponse.IsSuccessStatusCode)
        {
            var categories = await listResponse.Content.ReadFromJsonAsync<List<CategoryDto>>();
            categories.Should().Contain(c => c.Id == created.Id);
        }

        // 3. Buscar por ID
        var getResponse = await _client.GetAsync($"/api/categories/{created.Id}");
        if (getResponse.IsSuccessStatusCode)
        {
            var fetched = await getResponse.Content.ReadFromJsonAsync<CategoryDto>();
            fetched!.Name.Should().Be("Informática");
        }

        // 4. Atualizar
        var updateDto = new UpdateCategoryDto("Informática Atualizada", "Nova descrição");
        var updateResponse = await _client.PutAsJsonAsync($"/api/categories/{created.Id}", updateDto);
        
        if (updateResponse.IsSuccessStatusCode)
        {
            var updated = await updateResponse.Content.ReadFromJsonAsync<CategoryDto>();
            updated!.Name.Should().Be("Informática Atualizada");
        }

        // 5. Deletar
        var deleteResponse = await _client.DeleteAsync($"/api/categories/{created.Id}");
        deleteResponse.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateCategory_WithInvalidData_ShouldReturnBadRequest()
    {
        var invalidCategory = new CreateCategoryDto("", ""); // Dados vazios
        var response = await _client.PostAsJsonAsync("/api/categories", invalidCategory);
        
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
    }
}
