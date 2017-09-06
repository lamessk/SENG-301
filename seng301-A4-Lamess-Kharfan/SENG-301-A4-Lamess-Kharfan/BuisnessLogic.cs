using Frontend4.Hardware;
using Frontend4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BuisnessLogic
{
    public PaymentFacade paymentFacade;
    public ProductsFacade productFacade;
    public CommunicationFacade communicationFacade;

    public BuisnessLogic(PaymentFacade payFacade, CommunicationFacade comFacade, ProductsFacade prodFacade)
    {
        paymentFacade = payFacade;
        productFacade = prodFacade;
        communicationFacade = comFacade;
        comFacade.selectionMade += new EventHandler<SelectionEventArgs>(ComFacade_SelectionMade);
        prodFacade.ProductRackEmpty += new EventHandler<ProductEventArgs>(ProdFacade_ProductRackEmpty);
    }

    private void ProdFacade_ProductRackEmpty(object sender, ProductEventArgs e)
    {
        string prodName = e.productName;
        communicationFacade.displayRackEmptyMessage(prodName);
    }

    private void ComFacade_SelectionMade(object sender, SelectionEventArgs e)
    {
        if (paymentFacade.getFunds() >= e.productCost.Value)
        {
            productFacade.Dispense(e.index);

            paymentFacade.StorePayment();

            paymentFacade.totalFunds = paymentFacade.deliverChange(e.productCost.Value, paymentFacade.getFunds());
        }
        else
        {
            communicationFacade.displayInsufficientMessage(e.productName, e.productCost, paymentFacade.getFunds());
        }
    }

}

