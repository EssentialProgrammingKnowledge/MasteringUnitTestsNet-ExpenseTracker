using Bunit;
using ExpenseTracker.UI.Components;
using ExpenseTracker.UI.Languages;
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

namespace ExpenseTracker.UI.UnitTests.Components
{
    public class CategoriesComponentTests
    {
        [Fact]
        public void ShouldRenderComponent()
        {
            // Arrange Act
            var categoriesComponent = _testContext.RenderComponent<CategoriesComponent>();

            // Assert
            categoriesComponent.ShouldNotBeNull();
            categoriesComponent.Markup.ShouldNotBeNullOrWhiteSpace();
        }


        [Fact]
        public void DataIsLoading_ShouldShowLoadingIndicator()
        {
            // Arrange
            var tcs = new TaskCompletionSource<List<CategoryDTO>>();
            _categoryService.Setup(x => x.GetAll()).Returns(tcs.Task);

            // Act
            var categoriesComponent = _testContext.RenderComponent<CategoriesComponent>();

            // Assert
            var loadingIndicatorComponent = categoriesComponent.FindComponent<MudProgressCircular>();
            loadingIndicatorComponent.ShouldNotBeNull();
            loadingIndicatorComponent.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void DataLoaded_ShouldRenderTable()
        {
            // Arrange Act
            var categoriesComponent = _testContext.RenderComponent<CategoriesComponent>();

            // Assert
            var categoriesTable = categoriesComponent.FindComponent<MudTable<CategoryDTO>>();
            categoriesTable.ShouldNotBeNull();
            categoriesTable.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void DataLoaded_WithCollection_ShouldRenderTableWithRecords()
        {
            // Arrange
            var categories = CreateCategories();
            _categoryService.Setup(p => p.GetAll()).ReturnsAsync(categories);

            // Act
            var categoriesComponent = _testContext.RenderComponent<CategoriesComponent>();

            // Assert
            var categoriesTable = categoriesComponent.FindComponent<MudTable<CategoryDTO>>();
            categories.ForEach(p => categoriesTable.Find($"[data-name=\"categories-column-id-{p.Id}\"]").ShouldNotBeNull());
        }

        [Fact]
        public void AddButtonClicked_ShouldOpenModalWithCategoryForm()
        {
            // Arrange
            var categoriesComponent = _testContext.RenderComponent<CategoriesComponent>();
            var addButton = categoriesComponent.Find("[data-name=\"categories-add-button\"]");
            var categoryFormComponent = _testContext.RenderComponent<MudDialogProvider>();

            // Act
            addButton.Click();

            // Assert
            categoryFormComponent.ShouldNotBeNull();
            categoryFormComponent.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void AddNewCategory_EmptyForm_ShouldShowErrorMessages()
        {
            // Arrange
            var categoriesComponent = _testContext.RenderComponent<CategoriesComponent>();
            var addButton = categoriesComponent.Find("[data-name=\"categories-add-button\"]");
            var categoryFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            addButton.Click();
            var submitButton = categoryFormComponent.Find("[data-name=\"category-form-submit\"]");

            // Act
            submitButton.Click();

            // Assert
            var editForm = categoryFormComponent.FindComponent<EditForm>();
            editForm.ShouldNotBeNull();
            editForm.Instance.ShouldNotBeNull();
            editForm.Instance.EditContext.ShouldNotBeNull();
            var validationMessages = editForm.Instance.EditContext.GetValidationMessages();
            validationMessages.ShouldNotBeNull().ShouldNotBeEmpty();
        }

        [Fact]
        public void AddNewCategory_FillForm_ShouldAddAndFetchAllCategories()
        {
            // Arrange
            var categories = CreateCategories();
            _categoryService.Setup(c => c.GetAll()).ReturnsAsync(categories);
            var categoriesComponent = _testContext.RenderComponent<CategoriesComponent>();
            var addButton = categoriesComponent.Find("[data-name=\"categories-add-button\"]");
            var categoryFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            addButton.Click();
            var newCategory = new CategoryDTO
            {
                Name = "NewCategory",
                Budget = 100
            };
            FillForm(categoryFormComponent, newCategory);
            var submitButton = categoryFormComponent.Find("[data-name=\"category-form-submit\"]");

            // Act
            submitButton.Click();

            // Assert
            _categoryService.Verify(p => p.Add(It.Is<CategoryDTO>(c => c.Name == newCategory.Name && c.Budget == newCategory.Budget)), Times.Once());
            _categoryService.Verify(p => p.GetAll(), Times.Exactly(2));
            categoryFormComponent.Markup.ShouldBeEmpty();
        }

        [Fact]
        public void EditButtonClicked_ShouldOpenModalWithCategoryForm()
        {
            // Arrange
            var categories = CreateCategories();
            _categoryService.Setup(c => c.GetAll()).ReturnsAsync(categories);
            var firstCategory = categories.First();
            _categoryService.Setup(p => p.GetById(firstCategory.Id)).ReturnsAsync(firstCategory);
            var categoriesComponent = _testContext.RenderComponent<CategoriesComponent>();
            var editButton = categoriesComponent.Find($"[data-name=\"categories-column-edit-with-id-{firstCategory.Id}\"]");
            var categoryFormComponent = _testContext.RenderComponent<MudDialogProvider>();

            // Act
            editButton.Click();

            // Assert
            categoryFormComponent.ShouldNotBeNull();
            categoryFormComponent.Markup.ShouldNotBeNullOrWhiteSpace();
            _categoryService.Verify(p => p.GetById(firstCategory.Id), Times.Once());
            var categoryNameInput = categoryFormComponent.Find("input[data-name=\"category-form-name\"]");
            var categoryBudgetInput = categoryFormComponent.Find("input[data-name=\"category-form-budget\"]");
            categoryNameInput.GetAttribute("value").ShouldBe(firstCategory.Name);
            categoryBudgetInput.GetAttribute("value").ShouldBe(firstCategory.Budget.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void EditCategory_InvalidValues_ShouldShowErrorMessages()
        {
            // Arrange
            var categories = CreateCategories();
            _categoryService.Setup(c => c.GetAll()).ReturnsAsync(categories);
            var firstCategory = categories.First();
            _categoryService.Setup(p => p.GetById(firstCategory.Id)).ReturnsAsync(firstCategory);
            var categoriesComponent = _testContext.RenderComponent<CategoriesComponent>();
            var editButton = categoriesComponent.Find($"[data-name=\"categories-column-edit-with-id-{firstCategory.Id}\"]");
            var categoryFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            var newCategory = new CategoryDTO
            {
                Name = string.Empty,
                Budget = -100
            };
            editButton.Click();

            // Act
            FillForm(categoryFormComponent, newCategory);

            // Assert
            _categoryService.Verify(p => p.GetById(firstCategory.Id), Times.Once());
            var editForm = categoryFormComponent.FindComponent<EditForm>();
            editForm.ShouldNotBeNull();
            editForm.Instance.ShouldNotBeNull();
            editForm.Instance.EditContext.ShouldNotBeNull();
            var validationMessages = editForm.Instance.EditContext.GetValidationMessages();
            validationMessages.ShouldNotBeNull().ShouldNotBeEmpty();
        }

        [Fact]
        public void EditCategory_ValidData_ShouldUpdateAndFetchAllCategories()
        {
            // Arrange
            var categories = CreateCategories();
            _categoryService.Setup(c => c.GetAll()).ReturnsAsync(categories);
            var firstCategory = categories.First();
            _categoryService.Setup(p => p.GetById(firstCategory.Id)).ReturnsAsync(firstCategory);
            var categoriesComponent = _testContext.RenderComponent<CategoriesComponent>();
            var editButton = categoriesComponent.Find($"[data-name=\"categories-column-edit-with-id-{firstCategory.Id}\"]");
            var categoryFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            editButton.Click();
            var newCategory = new CategoryDTO
            {
                Name = "NewCategory",
                Budget = 150.25M
            };
            FillForm(categoryFormComponent, newCategory);
            var submitButton = categoryFormComponent.Find("[data-name=\"category-form-submit\"]");

            // Act
            submitButton.Click();

            // Assert
            _categoryService.Verify(p => p.GetById(firstCategory.Id), Times.Once());
            _categoryService.Verify(p => p.Update(It.Is<CategoryDTO>(c => c.Name == newCategory.Name && c.Budget == newCategory.Budget)), Times.Once());
            _categoryService.Verify(p => p.GetAll(), Times.Exactly(2));
            categoryFormComponent.Markup.ShouldBeEmpty();
        }

        [Fact]
        public void DeleteButtonClicked_ShouldOpenModal()
        {
            // Arrange
            var categories = CreateCategories();
            _categoryService.Setup(p => p.GetAll()).ReturnsAsync(categories);
            var firstCategory = categories.First();
            _categoryService.Setup(p => p.GetById(firstCategory.Id)).ReturnsAsync(firstCategory);
            var categoryComponent = _testContext.RenderComponent<CategoriesComponent>();
            var deleteButton = categoryComponent.Find($"[data-name=\"categories-column-delete-with-id-{firstCategory.Id}\"]");
            var deleteCategoryComponent = _testContext.RenderComponent<MudDialogProvider>();

            // Act
            deleteButton.Click();

            // Assert
            deleteCategoryComponent.ShouldNotBeNull();
            deleteCategoryComponent.Markup.ShouldNotBeNullOrWhiteSpace();
            var yesButton = deleteCategoryComponent.Find(".mud-message-box__yes-button");
            yesButton.ShouldNotBeNull();
            var noButton = deleteCategoryComponent.Find(".mud-message-box__no-button");
            noButton.ShouldNotBeNull();
        }

        [Fact]
        public void DeleteCategory_ShouldDeleteAndFetchAllCategories()
        {
            // Arrange
            var categories = CreateCategories();
            _categoryService.Setup(p => p.GetAll()).ReturnsAsync(categories);
            var firstCategory = categories.First();
            var categoryComponent = _testContext.RenderComponent<CategoriesComponent>();
            var deleteButton = categoryComponent.Find($"[data-name=\"categories-column-delete-with-id-{firstCategory.Id}\"]");
            var deleteCategoryComponent = _testContext.RenderComponent<MudDialogProvider>();
            deleteButton.Click();
            var confirmDeleteButton = deleteCategoryComponent.Find(".mud-message-box__yes-button");

            // Act
            confirmDeleteButton.Click();

            // Assert
            deleteCategoryComponent.Markup.ShouldBeEmpty();
            _categoryService.Verify(p => p.Delete(firstCategory.Id), Times.Once());
            _categoryService.Verify(p => p.GetAll(), Times.Exactly(2));
        }

        private void FillForm(IRenderedComponent<MudDialogProvider> categoryFormComponent, CategoryDTO dto)
        {
            categoryFormComponent.Find("input[data-name=\"category-form-name\"]").Input(new ChangeEventArgs() { Value = dto.Name });
            categoryFormComponent.Find("input[data-name=\"category-form-budget\"]").Input(new ChangeEventArgs() { Value = dto.Budget.ToString(new CultureInfo("en-US")) });
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
        private readonly Mock<ICategoryService> _categoryService;

        public CategoriesComponentTests()
        {
            _categoryService = new Mock<ICategoryService>();
            _testContext = new ConfiguredTestContext();
            _testContext.Services.AddScoped((_) => _categoryService.Object);
            _categoryService.Setup(c => c.GetAll()).ReturnsAsync([]);
            _testContext.Services.AddSingleton<ITranslateService, TranslateService>();
            var popoverProvider = _testContext.RenderComponent<MudPopoverProvider>();
            popoverProvider.ShouldNotBeNull();
            popoverProvider.Markup.ShouldNotBeNull();
        }
    }
}
