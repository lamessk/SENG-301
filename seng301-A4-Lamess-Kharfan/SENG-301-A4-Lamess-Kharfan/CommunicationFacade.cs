using Frontend4;
using Frontend4.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SelectionEventArgs : EventArgs
{
    public int index { get; set; }
    public string productName { get; set; }
    public Cents productCost { get; set; }
}

public class CommunicationFacade
{

    public event EventHandler<SelectionEventArgs> selectionMade;
    Dictionary<SelectionButton, int> selectionButtonToIndex;
    VendingMachine vm;

    public CommunicationFacade(VendingMachine vm)
    {
        this.vm = vm;
        SelectionButton[] selectionButtons = vm.Hardware.SelectionButtons;

        selectionButtonToIndex = new Dictionary<SelectionButton, int>();
        for (int i = 0; i < selectionButtons.Length; i++)
        {
            selectionButtons[i].Pressed += new EventHandler(SelectionButton_Pressed);
            this.selectionButtonToIndex[selectionButtons[i]] = i;
        }

    }

    private void SelectionButton_Pressed(object sender, EventArgs e)
    {
        int selected = this.selectionButtonToIndex[(SelectionButton)sender];
        var prodName = vm.Hardware.ProductKinds[selected].Name;
        var prodCost = vm.Hardware.ProductKinds[selected].Cost;
        if (this.selectionMade != null)
        {
            this.selectionMade(this, new SelectionEventArgs() { index = selected, productCost = prodCost, productName = prodName });
        }
    }

    public void displayInsufficientMessage(string prName, Cents prCost, int inserted)
    {
        vm.Hardware.Display.DisplayMessage("Cost for " + prName + ": " + prCost + "; Available funds: " + inserted); 
    }

    public void displayRackEmptyMessage(string name)
    {
        vm.Hardware.Display.DisplayMessage("The vending machine is all out of " + name + ", please make another selection");
    }
}

