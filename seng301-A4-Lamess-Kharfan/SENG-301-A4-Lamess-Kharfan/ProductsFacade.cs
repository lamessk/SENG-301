using Frontend4.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ProductEventArgs : EventArgs
{
    public string productName { get; set; }
}

public class ProductsFacade
{
    VendingMachine vm;
    ProductRack[] prodRacks;
    Dictionary<ProductRack, int> ProductRackToIndex;

    public event EventHandler<ProductEventArgs> ProductRackEmpty;

    public ProductsFacade(VendingMachine vMachine)
    {
        vm = vMachine;
        prodRacks = vm.Hardware.ProductRacks;

        ProductRackToIndex = new Dictionary<ProductRack, int>();
        for (int i = 0; i < prodRacks.Length; i++)
        {
            prodRacks[i].ProductRackEmpty += ProductsFacade_ProductRackEmpty;
            this.ProductRackToIndex[prodRacks[i]] = i;
        }

    }
    private void ProductsFacade_ProductRackEmpty(object sender, EventArgs e)
    {
        int selected = this.ProductRackToIndex[(ProductRack)sender];
        string name = vm.Hardware.ProductKinds[selected].Name;
        this.ProductRackEmpty(this, new ProductEventArgs() { productName = name });
    }

    public void Dispense(int index)
    {
        vm.Hardware.ProductRacks[index].DispenseProduct();
    }
}