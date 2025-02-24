using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Repositories;
using ExpenseTracker.API.Services;
using ExpenseTracker.API.Validations;
using Moq;
using Shouldly;

namespace ExpenseTracker.UnitTests.Services
{
    public class ExpenseServiceTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task AddExpense_InvalidAmount_ShouldReturnBadRequest(decimal amount)
        {
            // Arrange
            var invalidExpense = new ExpenseDTO(0, amount, 1, "");
            var expectedError = ExpenseErrorMessages.AmountMustBeGreaterThanZero();

            // Act
            var result = await _service.AddExpense(invalidExpense);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldBeNull();
            _expenseRepository.Verify(c => c.Add(It.IsAny<Expense>()), Times.Never());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task AddExpense_InvalidDescription_ShouldReturnBadRequest(string? description)
        {
            // Arrange
            var invalidExpense = new ExpenseDTO(0, 100, 1, description!);
            var expectedError = ExpenseErrorMessages.DescriptionCannotBeEmpty();

            // Act
            var result = await _service.AddExpense(invalidExpense);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldBeNull();
            _expenseRepository.Verify(c => c.Add(It.IsAny<Expense>()), Times.Never());
        }

        [Fact]
        public async Task AddExpense_TooLongDescription_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidExpense = new ExpenseDTO(0, 100, 1, "DescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescription123456789");
            var expectedError = ExpenseErrorMessages.DescriptionTooLong(250, invalidExpense.Description.Length);

            // Act
            var result = await _service.AddExpense(invalidExpense);

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
            _expenseRepository.Verify(c => c.Add(It.IsAny<Expense>()), Times.Never());
        }

        [Fact]
        public async Task AddExpense_CategoryNotFound_ShouldReturnBadRequest()
        {
            // Arrange
            var validExpense = new ExpenseDTO(0, 100, 1, "Test");
            var expectedError = CategoryErrorMessages.NotFound(validExpense.CategoryId);

            // Act
            var result = await _service.AddExpense(validExpense);

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
            _expenseRepository.Verify(c => c.Add(It.IsAny<Expense>()), Times.Never());
        }

        [Fact]
        public async Task AddExpense_ExceedingBudget_ShouldReturnBadRequest()
        {
            // Arrange
            var category = new Category { Id = 1, Budget = 200 };
            var expenseDto = new ExpenseDTO(0, 300, 1, "Expensive");
            _categoryRepository.Setup(repo => repo.GetById(expenseDto.CategoryId)).ReturnsAsync(category);
            var totalExpenses = 100;
            _expenseRepository.Setup(repo => repo.GetTotalExpensesAmount(expenseDto.CategoryId)).ReturnsAsync(totalExpenses);
            decimal newTotalExpenses = totalExpenses + expenseDto.Amount;
            var expectedError = ExpenseErrorMessages.AmountExceedsBudget(expenseDto.Amount, category.Budget, newTotalExpenses);

            // Act
            var result = await _service.AddExpense(expenseDto);

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
            _expenseRepository.Verify(c => c.Add(It.IsAny<Expense>()), Times.Never());
        }

        [Fact]
        public async Task AddExpense_ValidData_ShouldReturnCreatedResult()
        {
            // Arrange
            var category = new Category { Id = 1, Budget = 500 };
            var expenseDto = new ExpenseDTO(0, 100, 1, "Valid Expense");
            var expense = new Expense { Amount = 100, CategoryId = 1, Description = "Valid Expense", Category = category };
            _categoryRepository.Setup(repo => repo.GetById(expenseDto.CategoryId)).ReturnsAsync(category);
            _expenseRepository.Setup(repo => repo.GetTotalExpensesAmount(expenseDto.CategoryId)).ReturnsAsync(100);
            _expenseRepository.Setup(repo => repo.Add(It.IsAny<Expense>())).ReturnsAsync(expense);

            // Act
            var result = await _service.AddExpense(expenseDto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.StatusCode.ShouldBe(StatusCode.Created);
            result.ErrorMessage.ShouldBeNull();
            _expenseRepository.Verify(c => c.Add(It.IsAny<Expense>()), Times.Once());
        }

        [Fact]
        public async Task DeleteExpense_ExpenseFound_ShouldReturnNoContent()
        {
            // Arrange
            _expenseRepository.Setup(repo => repo.Delete(It.IsAny<int>())).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteExpense(1);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.StatusCode.ShouldBe(StatusCode.NoContent);
            result.ErrorMessage.ShouldBeNull();
            _expenseRepository.Verify(c => c.Delete(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task DeleteExpense_ExpenseNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var id = 1;
            var expectedError = ExpenseErrorMessages.NotFound(id);

            // Act
            var result = await _service.DeleteExpense(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _expenseRepository.Verify(c => c.Delete(id), Times.Once());
        }

        [Fact]
        public async Task GetExpenseById_ExpenseFound_ShouldReturnExpense()
        {
            // Arrange
            var expense = new Expense { Amount = 100, Description = "Test", CategoryId = 1 };
            _expenseRepository.Setup(repo => repo.GetById(It.IsAny<int>())).ReturnsAsync(expense);

            // Act
            var result = await _service.GetExpenseById(1);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.StatusCode.ShouldBe(StatusCode.Ok);
            result.ErrorMessage.ShouldBeNull();
            _expenseRepository.Verify(c => c.GetById(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task GetExpenseById_ExpenseNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var id = 1;
            var expectedError = ExpenseErrorMessages.NotFound(id);

            // Act
            var result = await _service.GetExpenseById(id);

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
            _expenseRepository.Verify(c => c.GetById(id), Times.Once());
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task UpdateExpense_InvalidAmount_ShouldReturnBadRequest(decimal amount)
        {
            // Arrange
            var invalidExpense = new ExpenseDTO(1, amount, 1, "");
            var expectedError = ExpenseErrorMessages.AmountMustBeGreaterThanZero();

            // Act
            var result = await _service.UpdateExpense(invalidExpense);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldBeNull();
            _expenseRepository.Verify(c => c.Update(It.IsAny<Expense>()), Times.Never());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task UpdateExpense_InvalidDescription_ShouldReturnBadRequest(string? description)
        {
            // Arrange
            var invalidExpense = new ExpenseDTO(1, 100, 1, description!);
            var expectedError = ExpenseErrorMessages.DescriptionCannotBeEmpty();

            // Act
            var result = await _service.UpdateExpense(invalidExpense);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldBeNull();
            _expenseRepository.Verify(c => c.Update(It.IsAny<Expense>()), Times.Never());
        }

        [Fact]
        public async Task UpdateExpense_TooLongDescription_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidExpense = new ExpenseDTO(1, 100, 1, "DescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescriptionDescription123456789");
            var expectedError = ExpenseErrorMessages.DescriptionTooLong(250, invalidExpense.Description.Length);

            // Act
            var result = await _service.UpdateExpense(invalidExpense);

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
            _expenseRepository.Verify(c => c.Update(It.IsAny<Expense>()), Times.Never());
        }

        [Fact]
        public async Task UpdateExpense_ExpenseNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var expenseDto = new ExpenseDTO(1, 200, 1, "Updated");
            var expectedError = ExpenseErrorMessages.NotFound(expenseDto.Id);

            // Act
            var result = await _service.UpdateExpense(expenseDto);

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
            _expenseRepository.Verify(c => c.Update(It.IsAny<Expense>()), Times.Never());
        }

        [Fact]
        public async Task UpdateExpense_CategoryNotFound_ShouldReturnBadRequest()
        {
            // Arrange
            var expenseDto = new ExpenseDTO(1, 200, 1, "Updated");
            _expenseRepository.Setup(repo => repo.GetById(expenseDto.Id)).ReturnsAsync(new Expense { Id = 1, Amount = 100, Description = "Old", CategoryId = 1 });
            var expectedError = CategoryErrorMessages.NotFound(expenseDto.CategoryId);

            // Act
            var result = await _service.UpdateExpense(expenseDto);

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
            _expenseRepository.Verify(c => c.Update(It.IsAny<Expense>()), Times.Never());
        }

        [Fact]
        public async Task UpdateExpense_ExceedingBudget_ShouldReturnBadRequest()
        {
            // Arrange
            var category = new Category { Id = 1, Budget = 500 };
            var existingExpense = new Expense { Id = 1, Amount = 100, Description = "Old", CategoryId = 1, Category = category };
            var expenseDto = new ExpenseDTO(1, 600, 1, "Updated");
            _expenseRepository.Setup(repo => repo.GetById(expenseDto.Id)).ReturnsAsync(existingExpense);
            _categoryRepository.Setup(repo => repo.GetById(expenseDto.CategoryId)).ReturnsAsync(category);
            var totalExpenses = 100;
            _expenseRepository.Setup(repo => repo.GetTotalExpensesAmount(expenseDto.CategoryId)).ReturnsAsync(totalExpenses);
            decimal newTotalExpenses = totalExpenses - existingExpense.Amount + expenseDto.Amount;
            var expectedError = ExpenseErrorMessages.AmountExceedsBudget(expenseDto.Amount, category.Budget, newTotalExpenses);

            // Act
            var result = await _service.UpdateExpense(expenseDto);

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
            _expenseRepository.Verify(c => c.Update(existingExpense), Times.Never());
        }

        [Fact]
        public async Task UpdateExpense_ValidData_ShouldReturnOk()
        {
            // Arrange
            var category = new Category { Id = 1, Budget = 500 };
            var existingExpense = new Expense { Id = 1, Amount = 100, Description = "Old", CategoryId = 1, Category = category };
            var expenseDto = new ExpenseDTO(1, 200, 1, "Updated");
            _expenseRepository.Setup(repo => repo.GetById(expenseDto.Id)).ReturnsAsync(existingExpense);
            _categoryRepository.Setup(repo => repo.GetById(expenseDto.CategoryId)).ReturnsAsync(category);
            _expenseRepository.Setup(repo => repo.GetTotalExpensesAmount(expenseDto.CategoryId)).ReturnsAsync(100);

            // Act
            var result = await _service.UpdateExpense(expenseDto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.StatusCode.ShouldBe(StatusCode.Ok);
            result.ErrorMessage.ShouldBeNull();
            _expenseRepository.Verify(c => c.Update(existingExpense), Times.Once());
        }

        private readonly ExpenseService _service;
        private readonly Mock<IExpenseRepository> _expenseRepository;
        private readonly Mock<ICategoryRepository> _categoryRepository;

        public ExpenseServiceTests()
        {
            _expenseRepository = new Mock<IExpenseRepository>();
            _categoryRepository = new Mock<ICategoryRepository>();
            _service = new ExpenseService(_expenseRepository.Object, _categoryRepository.Object);
        }
    }
}
