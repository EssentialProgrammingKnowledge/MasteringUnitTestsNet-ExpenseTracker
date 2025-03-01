using Bunit;
using ExpenseTracker.UI.Components;
using ExpenseTracker.UI.Models;
using ExpenseTracker.UI.UnitTests.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Shouldly;
using System.Globalization;

namespace ExpenseTracker.UI.UnitTests.Components
{
    public class ExpenseFormComponentTests
    {
        [Fact]
        public async Task ShouldRenderComponent()
        {
            // Arrange Act
            var result = await CreateComponent();

            result.ExpenseFormComponent.ShouldNotBeNull();
            result.ExpenseFormComponent.Markup.ShouldNotBeNullOrWhiteSpace();
            result.DialogReference.ShouldNotBeNull();
        }

        [Fact]
        public async Task FillForm_ExpenseIsProvided_ShouldPopulateFields()
        {
            // Arrange
            var existingExpense = new ExpenseDTO
            {
                Id = 1,
                Amount = 200,
                CategoryId = 1,
                Description = "Description"
            };
            var parameters = new DialogParameters { ["Expense"] = existingExpense };

            // Act
            var createdComponent = await CreateComponent("Edytuj wydatek", parameters);

            // Assert
            var descriptionField = createdComponent.ExpenseFormComponent.Find("input[data-name=\"expense-form-description\"]");
            var categoryIdField = createdComponent.ExpenseFormComponent.Find("input[data-name=\"expense-form-category\"]");
            var amountField = createdComponent.ExpenseFormComponent.Find("input[data-name=\"expense-form-amount\"]");
            descriptionField.GetAttribute("value").ShouldBe(existingExpense.Description);
            int.TryParse(categoryIdField.GetAttribute("value"), CultureInfo.InvariantCulture, out var categoryIdChanged).ShouldBeTrue();
            categoryIdChanged.ShouldBe(existingExpense.CategoryId);
            decimal.TryParse(amountField.GetAttribute("value"), CultureInfo.InvariantCulture, out var amountChanged).ShouldBeTrue();
            amountChanged.ShouldBe(existingExpense.Amount);
        }

        [Fact]
        public async Task FillForm_ExpenseProvidedAndInvalidValues_ShouldShowErrors()
        {
            // Arrange
            var existingExpense = new ExpenseDTO
            {
                Id = 1,
                Amount = 200,
                CategoryId = 2,
                Description = "Description",
            };
            var categories = CreateCategories();
            var category = categories.First();
            var parameters = new DialogParameters { ["Expense"] = existingExpense, ["Categories"] = categories };
            var createdComponent = await CreateComponent(dialogParameters: parameters);

            // Act
            await FillForm(createdComponent.ExpenseFormComponent, new ExpenseDTO { Amount = 0, Description = string.Empty, CategoryId = 0 });

            // Assert
            var editForm = createdComponent.ExpenseFormComponent.FindComponent<EditForm>();
            editForm.ShouldNotBeNull();
            editForm.Instance.EditContext.ShouldNotBeNull();
            var validationMessages = editForm.Instance.EditContext.GetValidationMessages();
            validationMessages.ShouldNotBeNull().ShouldNotBeEmpty();
        }

        [Fact]
        public async Task Cancel_ShouldCloseDialogWithoutSaving()
        {
            // Arrange
            var createdComponent = await CreateComponent();

            // Act
            createdComponent.ExpenseFormComponent.Find("button[data-name=\"expense-form-cancel\"]").Click();

            // Assert
            var result = await createdComponent.DialogReference.Result;
            result.ShouldNotBeNull();
            result.Canceled.ShouldBeTrue();
        }

        [Fact]
        public async Task SubmitForm_ExpenseNotProvidedAndEmptyForm_ShouldShowErrors()
        {
            // Arrange
            var createdComponent = await CreateComponent();

            // Act
            createdComponent.ExpenseFormComponent.Find("button[data-name=\"expense-form-submit\"]").Click();

            // Assert
            var editForm = createdComponent.ExpenseFormComponent.FindComponent<EditForm>();
            editForm.ShouldNotBeNull();
            editForm.Instance.EditContext.ShouldNotBeNull();
            var validationMessages = editForm.Instance.EditContext.GetValidationMessages();
            validationMessages.ShouldNotBeNull().ShouldNotBeEmpty();
        }

        [Fact]
        public async Task SubmitForm_ExpenseProvidedAndInvalidValues_ShouldShowErrors()
        {
            // Arrange
            var existingExpense = new ExpenseDTO
            {
                Id = 1,
                Amount = 200,
                CategoryId = 2,
                Description = "Description",
            };
            var categories = CreateCategories();
            var category = categories.First();
            var parameters = new DialogParameters { ["Expense"] = existingExpense, ["Categories"] = categories };
            var createdComponent = await CreateComponent(dialogParameters: parameters);

            // Act
            await FillFormAndSubmit(createdComponent.ExpenseFormComponent, new ExpenseDTO { Amount = 0, Description = string.Empty, CategoryId = 0 });

            // Assert
            var editForm = createdComponent.ExpenseFormComponent.FindComponent<EditForm>();
            editForm.ShouldNotBeNull();
            editForm.Instance.EditContext.ShouldNotBeNull();
            var validationMessages = editForm.Instance.EditContext.GetValidationMessages();
            validationMessages.ShouldNotBeNull().ShouldNotBeEmpty();
        }

        [Fact]
        public async Task SubmitForm_UserModifiesData_ShouldUpdateValues()
        {
            // Arrange
            var categories = CreateCategories();
            var category = categories.First();
            var parameters = new DialogParameters { ["Categories"] = categories };
            var createdComponent = await CreateComponent("Edytuj wydatek", parameters);
            var newExpense = new ExpenseDTO
            {
                Id = 1,
                Amount = 200,
                CategoryId = category.Id,
                Description = "NewDescription",
            };

            // Act
            await FillFormAndSubmit(createdComponent.ExpenseFormComponent, newExpense);

            // Assert
            var result = await createdComponent.DialogReference.Result;
            result.ShouldNotBeNull();
            var data = result.Data as ExpenseDTO;
            data.ShouldNotBeNull();
            data.Description.ShouldBe(newExpense.Description);
            data.Amount.ShouldBe(newExpense.Amount);
            data.CategoryId.ShouldBe(newExpense.CategoryId);
            createdComponent.ExpenseFormComponent.Markup.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task SubmitForm_EditingExpenseUserModifiesData_ShouldUpdateValuesAndCloseDialogWithCorrectData()
        {
            // Arrange
            var existingExpense = new ExpenseDTO
            {
                Id = 1,
                Amount = 200,
                CategoryId = 2,
                Description = "Description",
            };
            var categories = CreateCategories();
            var category = categories.First();
            var parameters = new DialogParameters { ["Expense"] = existingExpense, ["Categories"] = categories };
            var createdComponent = await CreateComponent("Edytuj wydatek", parameters);
            var newExpense = new ExpenseDTO
            {
                Id = 1,
                Amount = 200,
                CategoryId = category.Id,
                Description = "NewDescription",
            };

            // Act
            await FillFormAndSubmit(createdComponent.ExpenseFormComponent, newExpense);

            // Assert
            var result = await createdComponent.DialogReference.Result;
            result.ShouldNotBeNull();
            var data = result.Data as ExpenseDTO;
            data.ShouldNotBeNull();
            data.Description.ShouldBe(newExpense.Description);
            data.Amount.ShouldBe(newExpense.Amount);
            data.CategoryId.ShouldBe(newExpense.CategoryId);
            createdComponent.ExpenseFormComponent.Markup.ShouldBe(string.Empty);
        }

        private async Task FillFormAndSubmit(IRenderedComponent<MudDialogProvider> expenseFormComponent, ExpenseDTO dto)
        {
            await FillForm(expenseFormComponent, dto);
            expenseFormComponent.Find("button[data-name=\"expense-form-submit\"]").Click();
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

        private async Task<(IRenderedComponent<MudDialogProvider> ExpenseFormComponent, IDialogReference DialogReference)> CreateComponent(string? title = null, DialogParameters? dialogParameters = null)
        {
            var dialogService = _testContext.Services.GetRequiredService<IDialogService>();
            var expenseFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            var dialogReference = dialogParameters is not null ?
                await dialogService.ShowAsync<ExpenseFormComponent>(title, dialogParameters)
                : await dialogService.ShowAsync<ExpenseFormComponent>(title);
            return (expenseFormComponent, dialogReference);
        }

        private List<CategoryDTO> CreateCategories()
        {
            return [
                new CategoryDTO { Id = 1, Name = "Category#1", Budget = 1000 },
                new CategoryDTO { Id = 1, Name = "Category#2", Budget = 5000 },
                new CategoryDTO { Id = 1, Name = "Category#3", Budget = 10000 }
            ];
        }

        private readonly TestContext _testContext;

        public ExpenseFormComponentTests()
        {
            _testContext = new ConfiguredTestContext();
        }
    }
}
