using Bunit;
using ExpenseTracker.UI.Components;
using ExpenseTracker.UI.Models;
using ExpenseTracker.UI.Services;
using ExpenseTracker.UI.UnitTests.Common;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor;
using Shouldly;
using System.Globalization;
using ExpenseTracker.UI.Languages;

namespace ExpenseTracker.UI.UnitTests.Components
{
    public class ExpensesComponentTests
    {
        [Fact]
        public void ShouldRenderComponent()
        {
            // Arrange Act
            var expensesComponent = _testContext.RenderComponent<ExpensesComponent>();

            // Assert
            expensesComponent.ShouldNotBeNull();
            expensesComponent.Markup.ShouldNotBeNullOrWhiteSpace();
        }


        [Fact]
        public void DataIsLoading_ShouldShowLoadingIndicator()
        {
            // Arrange
            var tcs = new TaskCompletionSource<Result<List<ExpenseDTO>>>();
            _expenseService.Setup(x => x.GetAll()).Returns(tcs.Task);

            // Act
            var expensesComponent = _testContext.RenderComponent<ExpensesComponent>();

            // Assert
            var loadingIndicatorComponent = expensesComponent.FindComponent<MudProgressLinear>();
            loadingIndicatorComponent.ShouldNotBeNull();
            loadingIndicatorComponent.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void DataLoaded_ShouldRenderTable()
        {
            // Arrange Act
            var expensesComponent = _testContext.RenderComponent<ExpensesComponent>();

            // Assert
            var expensesTable = expensesComponent.FindComponent<MudTable<ExpenseDTO>>();
            expensesTable.ShouldNotBeNull();
            expensesTable.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void DataLoaded_WithCollection_ShouldRenderTableWithRecords()
        {
            // Arrange
            var expenses = CreateExpenses();
            _expenseService.Setup(p => p.GetAll()).ReturnsAsync(Result<List<ExpenseDTO>>.Success(expenses));

            // Act
            var expensesComponent = _testContext.RenderComponent<ExpensesComponent>();

            // Assert
            var expensesTable = expensesComponent.FindComponent<MudTable<ExpenseDTO>>();
            expenses.ForEach(p => expensesTable.Find($"[data-name=\"expenses-column-id-{p.Id}\"]").ShouldNotBeNull());
        }

        [Fact]
        public void AddButtonClicked_ShouldOpenModalWithExpenseForm()
        {
            // Arrange
            var expensesComponent = _testContext.RenderComponent<ExpensesComponent>();
            var addButton = expensesComponent.Find("[data-name=\"expenses-add-button\"]");
            var expenseFormComponent = _testContext.RenderComponent<MudDialogProvider>();

            // Act
            addButton.Click();

            // Assert
            expenseFormComponent.ShouldNotBeNull();
            expenseFormComponent.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void AddNewExpense_EmptyForm_ShouldShowErrorMessages()
        {
            // Arrange
            var expensesComponent = _testContext.RenderComponent<ExpensesComponent>();
            var addButton = expensesComponent.Find("[data-name=\"expenses-add-button\"]");
            var expenseFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            addButton.Click();
            var submitButton = expenseFormComponent.Find("[data-name=\"expense-form-submit\"]");

            // Act
            submitButton.Click();

            // Assert
            var editForm = expenseFormComponent.FindComponent<EditForm>();
            editForm.ShouldNotBeNull();
            editForm.Instance.ShouldNotBeNull();
            editForm.Instance.EditContext.ShouldNotBeNull();
            var validationMessages = editForm.Instance.EditContext.GetValidationMessages();
            validationMessages.ShouldNotBeNull().ShouldNotBeEmpty();
        }

        [Fact]
        public async Task AddNewExpense_FillForm_ShouldAddAndFetchAllExpenses()
        {
            // Arrange
            var categories = CreateCategories();
            _categoryService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<CategoryDTO>>.Success(categories));
            _expenseService.Setup(p => p.Add(It.IsAny<ExpenseDTO>()))
                .ReturnsAsync(Result.Success());
            var expensesComponent = _testContext.RenderComponent<ExpensesComponent>();
            var addButton = expensesComponent.Find("[data-name=\"expenses-add-button\"]");
            var expenseFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            addButton.Click();
            var newExpense = new ExpenseDTO
            {
                Description = "NewExpense",
                Amount = 100,
                CategoryId = 1
            };
            await FillForm(expenseFormComponent, newExpense);
            var submitButton = expenseFormComponent.Find("[data-name=\"expense-form-submit\"]");

            // Act
            submitButton.Click();

            // Assert
            _expenseService.Verify(p => p.Add(It.Is<ExpenseDTO>(p => p.Description == newExpense.Description && p.Amount == newExpense.Amount && p.CategoryId == newExpense.CategoryId)), Times.Once());
            _expenseService.Verify(p => p.GetAll(), Times.Exactly(2));
            expenseFormComponent.Markup.ShouldBeEmpty();
        }

        [Fact]
        public void EditButtonClicked_ShouldOpenModalWithExpenseForm()
        {
            // Arrange
            var categories = CreateCategories();
            _categoryService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<CategoryDTO>>.Success(categories));
            var expenses = CreateExpenses();
            _expenseService.Setup(p => p.GetAll()).ReturnsAsync(Result<List<ExpenseDTO>>.Success(expenses));
            var firstExpense = expenses.First();
            var category = CreateCategories().First(c => c.Id == firstExpense.CategoryId);
            _expenseService.Setup(p => p.GetById(firstExpense.Id)).ReturnsAsync(Result<ExpenseDetailsDTO?>.Success(MapToDetailsDto(firstExpense, category)));
            var expensesComponent = _testContext.RenderComponent<ExpensesComponent>();
            var editButton = expensesComponent.Find($"[data-name=\"expenses-column-edit-with-id-{firstExpense.Id}\"]");
            var expenseFormComponent = _testContext.RenderComponent<MudDialogProvider>();

            // Act
            editButton.Click();

            // Assert
            expenseFormComponent.ShouldNotBeNull();
            expenseFormComponent.Markup.ShouldNotBeNullOrWhiteSpace();
            _expenseService.Verify(p => p.GetById(firstExpense.Id), Times.Once());
            var expenseDescriptionInput = expenseFormComponent.Find("input[data-name=\"expense-form-description\"]");
            var expenseAmountInput = expenseFormComponent.Find("input[data-name=\"expense-form-amount\"]");
            var expenseCategoryInput = expenseFormComponent.Find("input[data-name=\"expense-form-category\"]");
            expenseDescriptionInput.GetAttribute("value").ShouldBe(firstExpense.Description);
            expenseAmountInput.GetAttribute("value").ShouldBe(firstExpense.Amount.ToString(CultureInfo.InvariantCulture));
            expenseCategoryInput.GetAttribute("value").ShouldBe(firstExpense.CategoryId.ToString());
        }

        [Fact]
        public async Task EditExpense_InvalidValues_ShouldShowErrorMessages()
        {
            // Arrange
            var categories = CreateCategories();
            _categoryService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<CategoryDTO>>.Success(categories));
            var expenses = CreateExpenses();
            _expenseService.Setup(p => p.GetAll()).ReturnsAsync(Result<List<ExpenseDTO>>.Success(expenses));
            var firstExpense = expenses.First();
            var category = CreateCategories().First(c => c.Id == firstExpense.CategoryId);
            _expenseService.Setup(p => p.GetById(firstExpense.Id)).ReturnsAsync(Result<ExpenseDetailsDTO?>.Success(MapToDetailsDto(firstExpense, category)));
            var expensesComponent = _testContext.RenderComponent<ExpensesComponent>();
            var editButton = expensesComponent.Find($"[data-name=\"expenses-column-edit-with-id-{firstExpense.Id}\"]");
            var expenseFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            var newExpense = new ExpenseDTO
            {
                Description = string.Empty,
                Amount = -100,
                CategoryId = -1
            };
            editButton.Click();

            // Act
            await FillForm(expenseFormComponent, newExpense);

            // Assert
            _expenseService.Verify(p => p.GetById(firstExpense.Id), Times.Once());
            var editForm = expenseFormComponent.FindComponent<EditForm>();
            editForm.ShouldNotBeNull();
            editForm.Instance.ShouldNotBeNull();
            editForm.Instance.EditContext.ShouldNotBeNull();
            var validationMessages = editForm.Instance.EditContext.GetValidationMessages();
            validationMessages.ShouldNotBeNull().ShouldNotBeEmpty();
        }

        [Fact]
        public async Task EditExpense_ValidData_ShouldUpdateAndFetchAllExpenses()
        {
            // Arrange
            var categories = CreateCategories();
            var expenses = CreateExpenses();
            _categoryService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<CategoryDTO>>.Success(categories));
            _expenseService.Setup(p => p.GetAll()).ReturnsAsync(Result<List<ExpenseDTO>>.Success(expenses));
            var firstExpense = expenses.First();
            var category = categories.First(c => c.Id == firstExpense.CategoryId);
            _expenseService.Setup(p => p.GetById(firstExpense.Id)).ReturnsAsync(Result<ExpenseDetailsDTO?>.Success(MapToDetailsDto(firstExpense, category)));
            _expenseService.Setup(p => p.Update(It.IsAny<ExpenseDTO>())).ReturnsAsync(Result.Success());
            var expensesComponent = _testContext.RenderComponent<ExpensesComponent>();
            var editButton = expensesComponent.Find($"[data-name=\"expenses-column-edit-with-id-{firstExpense.Id}\"]");
            var expenseFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            editButton.Click();
            var newExpense = new ExpenseDTO
            {
                Description = "NewEexpense",
                Amount = 150.25M,
                CategoryId = 1
            };
            await FillForm(expenseFormComponent, newExpense);
            var submitButton = expenseFormComponent.Find("[data-name=\"expense-form-submit\"]");

            // Act
            submitButton.Click();

            // Assert
            _expenseService.Verify(p => p.GetById(firstExpense.Id), Times.Once());
            _expenseService.Verify(p => p.Update(It.Is<ExpenseDTO>(p => p.Description == newExpense.Description && p.Amount == newExpense.Amount && p.CategoryId == newExpense.CategoryId)), Times.Once());
            _expenseService.Verify(p => p.GetAll(), Times.Exactly(2));
            expenseFormComponent.Markup.ShouldBeEmpty();
        }

        [Fact]
        public void DeleteButtonClicked_ShouldOpenModal()
        {
            // Arrange
            var expenses = CreateExpenses();
            _expenseService.Setup(p => p.GetAll()).ReturnsAsync(Result<List<ExpenseDTO>>.Success(expenses));
            var firstExpense = expenses.First();
            var category = CreateCategories().First(c => c.Id == firstExpense.CategoryId);
            _expenseService.Setup(p => p.GetById(firstExpense.Id)).ReturnsAsync(Result<ExpenseDetailsDTO?>.Success(MapToDetailsDto(firstExpense, category)));
            var expensesComponent = _testContext.RenderComponent<ExpensesComponent>();
            var deleteButton = expensesComponent.Find($"[data-name=\"expenses-column-delete-with-id-{firstExpense.Id}\"]");
            var deleteExpenseComponent = _testContext.RenderComponent<MudDialogProvider>();

            // Act
            deleteButton.Click();

            // Assert
            deleteExpenseComponent.ShouldNotBeNull();
            deleteExpenseComponent.Markup.ShouldNotBeNullOrWhiteSpace();
            var yesButton = deleteExpenseComponent.Find(".mud-message-box__yes-button");
            yesButton.ShouldNotBeNull();
            var noButton = deleteExpenseComponent.Find(".mud-message-box__no-button");
            noButton.ShouldNotBeNull();
        }

        [Fact]
        public void DeleteExpense_ShouldDeleteAndFetchAllExpenses()
        {
            // Arrange
            var expenses = CreateExpenses();
            _expenseService.Setup(p => p.GetAll()).ReturnsAsync(Result<List<ExpenseDTO>>.Success(expenses));
            var firstExpense = expenses.First();
            _expenseService.Setup(p => p.Delete(firstExpense.Id)).ReturnsAsync(Result.Success());
            var expensesComponent = _testContext.RenderComponent<ExpensesComponent>();
            var deleteButton = expensesComponent.Find($"[data-name=\"expenses-column-delete-with-id-{firstExpense.Id}\"]");
            var deleteExpenseComponent = _testContext.RenderComponent<MudDialogProvider>();
            deleteButton.Click();
            var confirmDeleteButton = deleteExpenseComponent.Find(".mud-message-box__yes-button");

            // Act
            confirmDeleteButton.Click();

            // Assert
            deleteExpenseComponent.Markup.ShouldBeEmpty();
            _expenseService.Verify(p => p.Delete(firstExpense.Id), Times.Once());
            _expenseService.Verify(p => p.GetAll(), Times.Exactly(2));
        }

        private async Task FillForm(IRenderedComponent<MudDialogProvider> expenseFormComponent, ExpenseDTO dto)
        {
            expenseFormComponent.Find("input[data-name=\"expense-form-description\"]").Input(new ChangeEventArgs() { Value = dto.Description });
            expenseFormComponent.Find("input[data-name=\"expense-form-amount\"]").Input(new ChangeEventArgs() { Value = dto.Amount.ToString(new CultureInfo("en-US")) });
            var selectListComponent = expenseFormComponent.FindComponents<MudSelect<int>>()
                                                          .First(c => c.Find("[data-name=\"expense-form-category\"]") is not null);
            await selectListComponent.InvokeAsync(async () =>
            {
                await selectListComponent.Instance.ValueChanged.InvokeAsync(dto.CategoryId);
            });
        }

        private ExpenseDetailsDTO MapToDetailsDto(ExpenseDTO dto, CategoryDTO category)
        {
            return new ExpenseDetailsDTO { Id = dto.Id, Amount = dto.Amount, Description = dto.Description, Category = category };
        }

        private List<CategoryDTO> CreateCategories()
        {
            return [
                new CategoryDTO { Id = 1, Name = "Category#1", Budget = 1000 },
                new CategoryDTO { Id = 1, Name = "Category#2", Budget = 5000 },
                new CategoryDTO { Id = 1, Name = "Category#3", Budget = 10000 }
            ];
        }

        private List<ExpenseDTO> CreateExpenses()
        {
            return [
                new ExpenseDTO { Id = 1, Description = "Category#1", Amount = 100, CategoryId = 1 },
                new ExpenseDTO { Id = 1, Description = "Category#2", Amount = 500, CategoryId = 2 },
                new ExpenseDTO { Id = 1, Description = "Category#3", Amount = 1000, CategoryId = 3 }
            ];
        }

        private readonly TestContext _testContext;
        private readonly Mock<IExpenseService> _expenseService;
        private readonly Mock<ICategoryService> _categoryService;

        public ExpensesComponentTests()
        {
            _expenseService = new Mock<IExpenseService>();
            _categoryService = new Mock<ICategoryService>();
            _testContext = new ConfiguredTestContext();
            _testContext.Services.AddScoped((_) => _expenseService.Object);
            _testContext.Services.AddScoped((_) => _categoryService.Object);
            _categoryService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<CategoryDTO>>.Success([]));
            _expenseService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<ExpenseDTO>>.Success([]));
            _testContext.Services.AddSingleton<ITranslateService, TranslateService>();
            var popoverProvider = _testContext.RenderComponent<MudPopoverProvider>();
            popoverProvider.ShouldNotBeNull();
            popoverProvider.Markup.ShouldNotBeNull();
        }
    }
}
