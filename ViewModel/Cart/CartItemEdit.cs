using System.ComponentModel.DataAnnotations;

namespace FakeStore.ViewModel;

public class CartItemEdit
{
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
