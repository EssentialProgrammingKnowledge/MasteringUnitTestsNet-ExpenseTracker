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
    public class CategoryFormComponentTests
    {
        [Fact]
        public async Task ShouldRenderComponent()
        {
            // Arrange Act
            var result = await CreateComponent();

            result.CategoryFormComponent.ShouldNotBeNull();
            result.CategoryFormComponent.Markup.ShouldNotBeNullOrWhiteSpace();
            result.DialogReference.ShouldNotBeNull();
        }

        [Fact]
        public async Task FillForm_CategoryIsProvided_ShouldPopulateFields()
        {
            // Arrange
            var existingCategory = new CategoryDTO
            {
                Id = 1,
                Name = "Category",
                Budget = 120.25M
            };
            var expectedTitle = "Edytuj kategorię";
            var parameters = new DialogParameters { ["Category"] = existingCategory, ["Title"] = expectedTitle };

            // Act
            var createdComponent = await CreateComponent(dialogParameters: parameters);

            // Assert
            var nameField = createdComponent.CategoryFormComponent.Find("input[data-name=\"category-form-name\"]");
            var budgetField = createdComponent.CategoryFormComponent.Find("input[data-name=\"category-form-budget\"]");
            nameField.GetAttribute("value").ShouldBe(existingCategory.Name);
            decimal.TryParse(budgetField.GetAttribute("value"), CultureInfo.InvariantCulture, out var budgetChanged).ShouldBeTrue();
            budgetChanged.ShouldBe(existingCategory.Budget);
            createdComponent.CategoryFormComponent.Markup.ShouldContain(expectedTitle);
        }

        [Fact]
        public async Task FillForm_CategoryProvidedAndInvalidValues_ShouldShowErrors()
        {
            // Arrange
            var existingCategory = new CategoryDTO
            {
                Id = 1,
                Name = "Category",
                Budget = 120.25M
            };
            var parameters = new DialogParameters { ["Category"] = existingCategory };
            var createdComponent = await CreateComponent(dialogParameters: parameters);

            // Act
            FillForm(createdComponent.CategoryFormComponent, new CategoryDTO { Name = string.Empty, Budget = 0 });

            // Assert
            var editForm = createdComponent.CategoryFormComponent.FindComponent<EditForm>();
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
            createdComponent.CategoryFormComponent.Find("button[data-name=\"category-form-cancel\"]").Click();

            // Assert
            var result = await createdComponent.DialogReference.Result;
            result.ShouldNotBeNull();
            result.Canceled.ShouldBeTrue();
        }

        [Fact]
        public async Task SubmitForm_CategoryNotProvidedAndEmptyForm_ShouldShowErrors()
        {
            // Arrange
            var createdComponent = await CreateComponent();

            // Act
            createdComponent.CategoryFormComponent.Find("button[data-name=\"category-form-submit\"]").Click();

            // Assert
            var editForm = createdComponent.CategoryFormComponent.FindComponent<EditForm>();
            editForm.ShouldNotBeNull();
            editForm.Instance.EditContext.ShouldNotBeNull();
            var validationMessages = editForm.Instance.EditContext.GetValidationMessages();
            validationMessages.ShouldNotBeNull().ShouldNotBeEmpty();
        }

        [Fact]
        public async Task SubmitForm_CategoryProvidedAndInvalidValues_ShouldShowErrors()
        {
            // Arrange
            var existingCategory = new CategoryDTO
            {
                Id = 1,
                Name = "Category#1",
                Budget = 200.55M
            };
            var parameters = new DialogParameters { ["Category"] = existingCategory };
            var createdComponent = await CreateComponent(dialogParameters: parameters);

            // Act
            FillFormAndSubmit(createdComponent.CategoryFormComponent, new CategoryDTO { Budget = 0, Name = string.Empty });

            // Assert
            var editForm = createdComponent.CategoryFormComponent.FindComponent<EditForm>();
            editForm.ShouldNotBeNull();
            editForm.Instance.EditContext.ShouldNotBeNull();
            var validationMessages = editForm.Instance.EditContext.GetValidationMessages();
            validationMessages.ShouldNotBeNull().ShouldNotBeEmpty();
        }

        [Fact]
        public async Task SubmitForm_UserModifiesData_ShouldUpdateValues()
        {
            // Arrange
            var createdComponent = await CreateComponent("Dodaj kategorię");
            var newCategory = new CategoryDTO
            {
                Id = 1,
                Name = "Category#Modified",
                Budget = 950.99M,
            };

            // Act
            FillFormAndSubmit(createdComponent.CategoryFormComponent, newCategory);

            // Assert
            var result = await createdComponent.DialogReference.Result;
            result.ShouldNotBeNull();
            var data = result.Data as CategoryDTO;
            data.ShouldNotBeNull();
            data.Name.ShouldBe(newCategory.Name);
            data.Budget.ShouldBe(newCategory.Budget);
            createdComponent.CategoryFormComponent.Markup.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task SubmitForm_EditingCategoryUserModifiesData_ShouldUpdateValuesAndCloseDialogWithCorrectData()
        {
            // Arrange
            var existingCategory = new CategoryDTO
            {
                Id = 1,
                Name = "Name#1",
                Budget = 100M
            };
            var parameters = new DialogParameters { ["Category"] = existingCategory };
            var createdComponent = await CreateComponent("Edytuj kategorię", parameters);
            var newCategory = new CategoryDTO
            {
                Id = 1,
                Name = "Name#Modified",
                Budget = 200.25M,
            };

            // Act
            FillFormAndSubmit(createdComponent.CategoryFormComponent, newCategory);

            // Assert
            var result = await createdComponent.DialogReference.Result;
            result.ShouldNotBeNull();
            var data = result.Data as CategoryDTO;
            data.ShouldNotBeNull();
            data.Name.ShouldBe(newCategory.Name);
            data.Budget.ShouldBe(newCategory.Budget);
            createdComponent.CategoryFormComponent.Markup.ShouldBe(string.Empty);
        }

        private void FillFormAndSubmit(IRenderedComponent<MudDialogProvider> categoryFormComponent, CategoryDTO dto)
        {
            FillForm(categoryFormComponent, dto);
            categoryFormComponent.Find("button[data-name=\"category-form-submit\"]").Click();
        }

        private void FillForm(IRenderedComponent<MudDialogProvider> categoryFormComponent, CategoryDTO dto)
        {
            categoryFormComponent.Find("input[data-name=\"category-form-name\"]").Input(new ChangeEventArgs() { Value = dto.Name });
            categoryFormComponent.Find("input[data-name=\"category-form-budget\"]").Input(new ChangeEventArgs() { Value = dto.Budget.ToString(new CultureInfo("en-US")) });
        }

        private async Task<(IRenderedComponent<MudDialogProvider> CategoryFormComponent, IDialogReference DialogReference)> CreateComponent(string? title = null, DialogParameters? dialogParameters = null)
        {
            var dialogService = _testContext.Services.GetRequiredService<IDialogService>();
            var categoryFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            var dialogReference = dialogParameters is not null ?
                await dialogService.ShowAsync<CategoryFormComponent>(title, dialogParameters)
                : await dialogService.ShowAsync<CategoryFormComponent>(title);
            return (categoryFormComponent, dialogReference);
        }

        private readonly TestContext _testContext;

        public CategoryFormComponentTests()
        {
            _testContext = new ConfiguredTestContext();
        }
    }
}
