using FakeStore.ViewModel;
using FakeStore.WebApp.Configuration;
using FakeStore.WebApp.Models;
using FakeStore.WebApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeStore.WebApp.Controllers;

public class ProductController : Controller
{
    private const string ProductCreateSuccessKey = "ProductCreateSuccess";
    private const string ProductEditSuccessKey = "ProductEditSuccess";
    private const string ProductDeleteSuccessKey = "ProductDeleteSuccess";
    private const string ProductReviewSuccessKey = "ProductReviewSuccess";

    private readonly IProductService _productService;
    private readonly ApiRuntimeMode _apiRuntimeMode;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(
        IProductService productService,
        ApiRuntimeMode apiRuntimeMode,
        IWebHostEnvironment webHostEnvironment)
    {
        _productService = productService;
        _apiRuntimeMode = apiRuntimeMode;
        _webHostEnvironment = webHostEnvironment;
    }

    public ActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> TabData()
    {
        if (!CanAccessTabScreen())
        {
            return StatusCode(StatusCodes.Status403Forbidden, "You need to sign-up to view this page");
        }

        try
        {
            var products = await _productService.GetAll();
            var vm = new ProductTabCardsViewModel
            {
                Products = products,
                HasFullAccessRole = HasFullAccessRole()
            };

            return PartialView("_ProductCards", vm);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not load products.");
        }
    }

    [HttpGet]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public IActionResult Create()
    {
        var vm = BuildCreatePageVm(
            successMessage: ReadTempDataMessage(ProductCreateSuccessKey));
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> Create(ProductFormInputModel form)
    {
        var vm = BuildCreatePageVm(form);
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var image = await ResolveImageSourceAsync(form);
        if (image is null)
        {
            return View(vm);
        }

        try
        {
            await _productService.CreateProduct(new ProductCreate
            {
                Name = form.Name.Trim(),
                Description = form.Description.Trim(),
                Price = form.Price,
                Unit = form.Unit.Trim(),
                Image = image,
                Discount = form.Discount,
                Available = form.Available,
                Brand = form.Brand.Trim(),
                Category = form.Category.Trim()
            });

            TempData[ProductCreateSuccessKey] = "Product created successfully.";
            return RedirectToAction(nameof(Create));
        }
        catch (Exception ex)
        {
            vm.ErrorMessage = BuildErrorMessage("Product creation failed.", ex);
            return View(vm);
        }
    }

    [HttpGet]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> Edit(int id)
    {
        ProductRead? product;
        try
        {
            product = await _productService.GetById(id);
        }
        catch (Exception ex)
        {
            return View(BuildEditPageVm(
                id,
                new ProductFormInputModel(),
                errorMessage: BuildErrorMessage("Could not load product data.", ex)));
        }

        if (product is null)
        {
            return NotFound();
        }

        var vm = BuildEditPageVm(
            id,
            MapToProductForm(product),
            successMessage: ReadTempDataMessage(ProductEditSuccessKey));
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> Edit(int id, ProductFormInputModel form)
    {
        var vm = BuildEditPageVm(id, form);
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        ProductRead? existingProduct;
        try
        {
            existingProduct = await _productService.GetById(id);
        }
        catch (Exception ex)
        {
            vm.ErrorMessage = BuildErrorMessage("Could not load product data.", ex);
            return View(vm);
        }

        if (existingProduct is null)
        {
            vm.ErrorMessage = "Product not found.";
            return View(vm);
        }

        var image = await ResolveImageSourceAsync(form, existingProduct.image);
        if (image is null)
        {
            return View(vm);
        }

        var previousStoredImage = IsStoredProductImage(existingProduct.image) ? existingProduct.image : null;
        var newStoredImage = IsStoredProductImage(image) ? image : null;
        var shouldDeletePreviousStoredImage = !string.IsNullOrWhiteSpace(previousStoredImage)
            && !string.Equals(previousStoredImage, image, StringComparison.OrdinalIgnoreCase);

        try
        {
            var updated = await _productService.UpdateProduct(id, new ProductUpdate
            {
                Name = form.Name.Trim(),
                Description = form.Description.Trim(),
                Price = form.Price,
                Unit = form.Unit.Trim(),
                Image = image,
                Discount = form.Discount,
                Available = form.Available,
                Brand = form.Brand.Trim(),
                Category = form.Category.Trim(),
                Rating = existingProduct.rating
            });

            if (updated is null)
            {
                if (!string.IsNullOrWhiteSpace(newStoredImage)
                    && !string.Equals(newStoredImage, existingProduct.image, StringComparison.OrdinalIgnoreCase))
                {
                    TryDeleteStoredProductImage(newStoredImage);
                }

                vm.ErrorMessage = "Product not found.";
                return View(vm);
            }

            if (shouldDeletePreviousStoredImage)
            {
                TryDeleteStoredProductImage(previousStoredImage);
            }

            TempData[ProductEditSuccessKey] = "Product updated successfully.";
            return RedirectToAction(nameof(Edit), new { id });
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrWhiteSpace(newStoredImage)
                && !string.Equals(newStoredImage, existingProduct.image, StringComparison.OrdinalIgnoreCase))
            {
                TryDeleteStoredProductImage(newStoredImage);
            }

            vm.ErrorMessage = BuildErrorMessage("Product update failed.", ex);
            return View(vm);
        }
    }

    [HttpGet]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> Delete(int id, bool deleted = false)
    {
        if (deleted)
        {
            return View(new ProductDeletePageViewModel
            {
                Deleted = true,
                SuccessMessage = ReadTempDataMessage(ProductDeleteSuccessKey) ?? "Product deleted successfully."
            });
        }

        try
        {
            var product = await _productService.GetById(id);
            if (product is null)
            {
                return NotFound();
            }

            return View(new ProductDeletePageViewModel
            {
                Product = product
            });
        }
        catch (Exception ex)
        {
            return View(new ProductDeletePageViewModel
            {
                ErrorMessage = BuildErrorMessage("Could not load product data.", ex)
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var deleted = await _productService.DeleteProduct(id);
            if (!deleted)
            {
                return View("Delete", new ProductDeletePageViewModel
                {
                    ErrorMessage = "Product not found."
                });
            }

            TempData[ProductDeleteSuccessKey] = "Product deleted successfully.";
            return RedirectToAction(nameof(Delete), new { id, deleted = true });
        }
        catch (Exception ex)
        {
            return View("Delete", new ProductDeletePageViewModel
            {
                ErrorMessage = BuildErrorMessage("Product deletion failed.", ex)
            });
        }
    }

    [HttpGet]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> ReviewCreate(int id)
    {
        var vm = await BuildReviewPageVm(
            id,
            successMessage: ReadTempDataMessage(ProductReviewSuccessKey));

        if (vm.Product is null)
        {
            return NotFound();
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> ReviewCreate(int id, ProductReviewFormInputModel form)
    {
        if (!ModelState.IsValid)
        {
            var invalidVm = await BuildReviewPageVm(id, form);
            return View(invalidVm);
        }

        try
        {
            await _productService.AddReview(id, new ReviewProductCreate
            {
                Rating = form.Rating,
                Comment = form.Comment.Trim()
            });

            TempData[ProductReviewSuccessKey] = "Review added successfully.";
            return RedirectToAction(nameof(ReviewCreate), new { id });
        }
        catch (Exception ex)
        {
            var failedVm = await BuildReviewPageVm(
                id,
                form,
                errorMessage: BuildErrorMessage("Review creation failed.", ex));
            return View(failedVm);
        }
    }

    private ProductFormPageViewModel BuildCreatePageVm(
        ProductFormInputModel? form = null,
        string? successMessage = null,
        string? errorMessage = null)
    {
        return new ProductFormPageViewModel
        {
            Title = "Create Product",
            SubmitLabel = "Create Product",
            Form = form ?? new ProductFormInputModel { Available = true },
            SuccessMessage = successMessage,
            ErrorMessage = errorMessage
        };
    }

    private ProductFormPageViewModel BuildEditPageVm(
        int id,
        ProductFormInputModel form,
        string? successMessage = null,
        string? errorMessage = null)
    {
        return new ProductFormPageViewModel
        {
            ProductId = id,
            Title = $"Edit Product #{id}",
            SubmitLabel = "Save Changes",
            Form = form,
            SuccessMessage = successMessage,
            ErrorMessage = errorMessage
        };
    }

    private async Task<ProductReviewPageViewModel> BuildReviewPageVm(
        int id,
        ProductReviewFormInputModel? form = null,
        string? successMessage = null,
        string? errorMessage = null)
    {
        ProductRead? product = null;

        try
        {
            product = await _productService.GetById(id);
            if (product is null)
            {
                errorMessage ??= "Product not found.";
            }
        }
        catch (Exception ex)
        {
            errorMessage ??= BuildErrorMessage("Could not load product data.", ex);
        }

        return new ProductReviewPageViewModel
        {
            Product = product,
            Form = form ?? new ProductReviewFormInputModel(),
            SuccessMessage = successMessage,
            ErrorMessage = errorMessage
        };
    }

    private async Task<string?> ResolveImageSourceAsync(ProductFormInputModel form, string? existingImage = null)
    {
        if (form.ImageFile is not null && form.ImageFile.Length > 0)
        {
            return await SaveUploadedImageAsync(form.ImageFile);
        }

        if (!string.IsNullOrWhiteSpace(form.ImageUrl))
        {
            return form.ImageUrl.Trim();
        }

        if (!string.IsNullOrWhiteSpace(existingImage))
        {
            return existingImage;
        }

        ModelState.AddModelError(nameof(ProductFormInputModel.ImageUrl), "Provide an image URL or upload an image file.");
        return null;
    }

    private async Task<string?> SaveUploadedImageAsync(IFormFile imageFile)
    {
        var extension = Path.GetExtension(imageFile.FileName);
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp"
        };

        if (!allowedExtensions.Contains(extension))
        {
            ModelState.AddModelError(
                nameof(ProductFormInputModel.ImageFile),
                "Unsupported image type. Allowed: .jpg, .jpeg, .png, .gif, .webp, .bmp.");
            return null;
        }

        const long maxSizeInBytes = 5 * 1024 * 1024;
        if (imageFile.Length > maxSizeInBytes)
        {
            ModelState.AddModelError(
                nameof(ProductFormInputModel.ImageFile),
                "Image file is too large. Maximum size is 5 MB.");
            return null;
        }

        if (string.IsNullOrWhiteSpace(_webHostEnvironment.WebRootPath))
        {
            ModelState.AddModelError(nameof(ProductFormInputModel.ImageFile), "Image upload is not available.");
            return null;
        }

        var folder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var filePath = Path.Combine(folder, fileName);
        await using var stream = System.IO.File.Create(filePath);
        await imageFile.CopyToAsync(stream);

        return $"/images/products/{fileName}";
    }

    private static bool IsStoredProductImage(string? imagePath)
    {
        return !string.IsNullOrWhiteSpace(imagePath)
               && imagePath.StartsWith("/images/products/", StringComparison.OrdinalIgnoreCase);
    }

    private void TryDeleteStoredProductImage(string? imagePath)
    {
        if (!IsStoredProductImage(imagePath) || string.IsNullOrWhiteSpace(_webHostEnvironment.WebRootPath))
        {
            return;
        }

        var productsFolder = Path.GetFullPath(Path.Combine(_webHostEnvironment.WebRootPath, "images", "products"));
        var relativePath = imagePath!.TrimStart('/')
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);
        var filePath = Path.GetFullPath(Path.Combine(_webHostEnvironment.WebRootPath, relativePath));

        if (!filePath.StartsWith(productsFolder, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        try
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        catch
        {
            // Best-effort cleanup; update success must not depend on image file deletion.
        }
    }

    private static ProductFormInputModel MapToProductForm(ProductRead product)
    {
        return new ProductFormInputModel
        {
            Name = product.name,
            Description = product.description,
            Price = (decimal)product.price,
            Unit = product.unit,
            ImageUrl = product.image,
            ExistingImage = product.image,
            Discount = product.discount,
            Available = product.availability,
            Brand = product.brand,
            Category = product.category
        };
    }

    private string? ReadTempDataMessage(string key)
    {
        return TempData[key]?.ToString();
    }

    private static string BuildErrorMessage(string prefix, Exception ex)
    {
        if (ex is HttpRequestException requestException && !string.IsNullOrWhiteSpace(requestException.Message))
        {
            return $"{prefix} {requestException.Message}";
        }

        return prefix;
    }

    private bool CanAccessTabScreen()
    {
        var isSignedIn = User?.Identity?.IsAuthenticated ?? false;

        return _apiRuntimeMode.IsPublicMode || (_apiRuntimeMode.IsCustomMode && isSignedIn);
    }

    private bool HasFullAccessRole()
    {
        return User?.Claims.Any(c =>
            c.Type == System.Security.Claims.ClaimTypes.Role
            && c.Value.Equals("full access", StringComparison.OrdinalIgnoreCase)) ?? false;
    }
}
