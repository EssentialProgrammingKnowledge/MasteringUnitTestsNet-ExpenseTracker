using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Repositories;
using ExpenseTracker.API.Services;
using ExpenseTracker.API.Validations;
using Moq;
using Shouldly;

namespace ExpenseTracker.UnitTests.Services
{
    public class CategoryServiceTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task AddCategory_InvalidBudget_ShouldReturnBadRequest(decimal budget)
        {
            // Arrange
            var invalidExpense = new CategoryDTO(0, "category", budget);
            var expectedError = CategoryErrorMessages.BudgetMustBeGreaterThanZero();

            // Act
            var result = await _service.AddCategory(invalidExpense);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldBeNull();
            _categoryRepository.Verify(c => c.Add(It.IsAny<Category>()), Times.Never());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task AddCategory_InvalidDescription_ShouldReturnBadRequest(string? name)
        {
            // Arrange
            var invalidCategory = new CategoryDTO(0, name!, 100);
            var expectedError = CategoryErrorMessages.NameCannotBeEmpty();

            // Act
            var result = await _service.AddCategory(invalidCategory);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldBeNull();
            _categoryRepository.Verify(c => c.Add(It.IsAny<Category>()), Times.Never());
        }

        [Fact]
        public async Task AddCategory_TooLongDescription_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidCategory = new CategoryDTO(0, "NameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameName1", 100);
            var expectedError = CategoryErrorMessages.NameTooLong(100, invalidCategory.Name.Length);

            // Act
            var result = await _service.AddCategory(invalidCategory);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _categoryRepository.Verify(c => c.Add(It.IsAny<Category>()), Times.Never());
        }

        [Fact]
        public async Task AddCategory_ValidData_ShouldReturnCreatedResult()
        {
            // Arrange
            var category = new CategoryDTO(0, "Name", 100);

            // Act
            var result = await _service.AddCategory(category);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.StatusCode.ShouldBe(StatusCode.Created);
            result.ErrorMessage.ShouldBeNull();
            _categoryRepository.Verify(c => c.Add(It.IsAny<Category>()), Times.Once());
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task UpdateCategory_InvalidBudget_ShouldReturnBadRequest(decimal budget)
        {
            // Arrange
            var invalidExpense = new CategoryDTO(1, "category", budget);
            var expectedError = CategoryErrorMessages.BudgetMustBeGreaterThanZero();

            // Act
            var result = await _service.UpdateCategory(invalidExpense);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldBeNull();
            _categoryRepository.Verify(c => c.Update(It.IsAny<Category>()), Times.Never());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task UpdateCategory_InvalidDescription_ShouldReturnBadRequest(string? name)
        {
            // Arrange
            var invalidCategory = new CategoryDTO(1, name!, 100);
            var expectedError = CategoryErrorMessages.NameCannotBeEmpty();

            // Act
            var result = await _service.UpdateCategory(invalidCategory);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldBeNull();
            _categoryRepository.Verify(c => c.Update(It.IsAny<Category>()), Times.Never());
        }

        [Fact]
        public async Task UpdateCategory_TooLongDescription_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidCategory = new CategoryDTO(1, "NameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameName1", 100);
            var expectedError = CategoryErrorMessages.NameTooLong(100, invalidCategory.Name.Length);

            // Act
            var result = await _service.UpdateCategory(invalidCategory);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _categoryRepository.Verify(c => c.Update(It.IsAny<Category>()), Times.Never());
        }

        [Fact]
        public async Task UpdateCategory_NotFoundCategory_ShouldReturnNotFound()
        {
            // Arrange
            var category = new CategoryDTO(1, "Name", 100);
            var expectedError = CategoryErrorMessages.NotFound(category.Id);

            // Act
            var result = await _service.UpdateCategory(category);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _categoryRepository.Verify(c => c.Update(It.IsAny<Category>()), Times.Never());
        }

        [Fact]
        public async Task UpdateCategory_BudgetLowerThanTotalExpensesCost_ShouldReturnBadRequest()
        {
            // Arrange
            var dto = new CategoryDTO(1, "Name", 100);
            var category = new Category
            {
                Id = dto.Id,
                Name = "Category",
                Budget = 1200
            };
            _categoryRepository.Setup(c => c.GetById(dto.Id)).ReturnsAsync(category);
            var totalExpensesCost = 1000;
            _categoryRepository.Setup(c => c.GetCategoriesTotalExpenses(dto.Id)).ReturnsAsync(totalExpensesCost);
            var expectedError = CategoryErrorMessages.LowerBudgetThanTotalExpenses(dto.Budget, totalExpensesCost);

            // Act
            var result = await _service.UpdateCategory(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _categoryRepository.Verify(c => c.Update(It.IsAny<Category>()), Times.Never());
        }

        [Fact]
        public async Task UpdateCategory_ValidData_ShouldReturnCreatedResult()
        {
            // Arrange
            var dto = new CategoryDTO(1, "Name", 100);
            var category = new Category
            {
                Id = dto.Id,
            };
            _categoryRepository.Setup(c => c.GetById(dto.Id)).ReturnsAsync(category);
            _categoryRepository.Setup(c => c.GetCategoriesTotalExpenses(dto.Id)).ReturnsAsync(10);

            // Act
            var result = await _service.UpdateCategory(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.StatusCode.ShouldBe(StatusCode.Ok);
            result.ErrorMessage.ShouldBeNull();
            _categoryRepository.Verify(c => c.Update(category), Times.Once());
        }

        [Fact]
        public async Task GetAllCategories_ShouldReturnEmptyCollection()
        {
            // Arrange Act
            var result = await _service.GetAllCategories();

            // Assert
            result.ShouldBeEmpty();
            _categoryRepository.Verify(c => c.GetAll(), Times.Once());
        }

        [Fact]
        public async Task GetCategoryById_NotFoundCategory_ShouldReturnNotFound()
        {
            // Arrange Act
            var result = await _service.GetCategoryById(1);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            result.ErrorMessage.ShouldNotBeNull();
            _categoryRepository.Verify(c => c.GetById(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task GetCategoryById_CategoryFound_ShouldReturnSuccessResult()
        {
            // Arrange
            var id = 1;
            _categoryRepository.Setup(c => c.GetById(id)).ReturnsAsync(new Category());

            // Act
            var result = await _service.GetCategoryById(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.StatusCode.ShouldBe(StatusCode.Ok);
            result.ErrorMessage.ShouldBeNull();
            _categoryRepository.Verify(c => c.GetById(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task DeleteCategory_CategoryNotFound_ShouldReturnNotFound()
        {
            // Arrange Act
            var result = await _service.DeleteCategory(1);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            result.ErrorMessage.ShouldNotBeNull();
        }

        [Fact]
        public async Task DeleteCategory_CategoryFoundContainsExpenses_ShouldReturnBadRequest()
        {
            // Arrange
            var id = 1;
            _categoryRepository.Setup(c => c.GetById(id)).ReturnsAsync(new Category());
            _categoryRepository.Setup(c => c.ContainExpenses(id)).ReturnsAsync(true);
            
            // Act
            var result = await _service.DeleteCategory(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
        }

        [Fact]
        public async Task DeleteCategory_CategoryFoundNotDeleteFromDatabase_ShouldReturnNotFound()
        {
            // Arrange
            var id = 1;
            var category = new Category();
            _categoryRepository.Setup(c => c.GetById(id)).ReturnsAsync(category);
            _categoryRepository.Setup(c => c.ContainExpenses(id)).ReturnsAsync(false);
            _categoryRepository.Setup(c => c.Delete(category)).ReturnsAsync(false);
            
            // Act
            var result = await _service.DeleteCategory(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            result.ErrorMessage.ShouldNotBeNull();
        }

        [Fact]
        public async Task DeleteCategory_CategoryFound_ShouldReturnSuccessResult()
        {
            // Arrange
            var id = 1;
            var category = new Category();
            _categoryRepository.Setup(c => c.GetById(id)).ReturnsAsync(category);
            _categoryRepository.Setup(c => c.ContainExpenses(id)).ReturnsAsync(false);
            _categoryRepository.Setup(c => c.Delete(category)).ReturnsAsync(true);
            
            // Act
            var result = await _service.DeleteCategory(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.StatusCode.ShouldBe(StatusCode.NoContent);
            result.ErrorMessage.ShouldBeNull();
        }

        private readonly CategoryService _service;
        private readonly Mock<ICategoryRepository> _categoryRepository;

        public CategoryServiceTests()
        {
            _categoryRepository = new Mock<ICategoryRepository>();
            _service = new CategoryService(_categoryRepository.Object);
        }
    }
}
