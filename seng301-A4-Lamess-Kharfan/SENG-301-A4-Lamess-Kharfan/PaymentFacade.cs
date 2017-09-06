using Frontend4;
using Frontend4.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PaymentFacade
{
    public int totalFunds;
    VendingMachine vendingmachine;
    Dictionary<int, int> coinKindToCoinRackIndex;

    public PaymentFacade(VendingMachine vm)
    {
        this.vendingmachine = vm;
        CoinSlot coinSlot = vendingmachine.Hardware.CoinSlot;

        this.coinKindToCoinRackIndex = new Dictionary<int, int>();
        for (int i = 0; i < vendingmachine.Hardware.CoinRacks.Length; i++)
        {
            this.coinKindToCoinRackIndex[(vendingmachine.Hardware.GetCoinKindForCoinRack(i)).Value] = i;
        }

        coinSlot.CoinAccepted += new EventHandler<CoinEventArgs> (CoinSlot_CoinAccepted);
    }

    public void CoinSlot_CoinAccepted(object sender, CoinEventArgs e)
    {
        Cents cents = e.Coin.Value;
        totalFunds += cents.Value;
    }

    public int getFunds()
    {
        return totalFunds;
    }

    public void StorePayment()
    {
        vendingmachine.Hardware.CoinReceptacle.StoreCoins();
    }

    public int deliverChange(int cost, int availableFunds)
    {
        var changeNeeded = availableFunds - cost;

        while (changeNeeded > 0)
        {
            var coinRacksWithMoney = this.coinKindToCoinRackIndex.Where(ck => ck.Key <= changeNeeded && vendingmachine.Hardware.CoinRacks[ck.Value].Count > 0).OrderByDescending(ck => ck.Key);

            if (coinRacksWithMoney.Count() == 0)
            {
                return changeNeeded; // this is what's left as available funds
            }

            var biggestCoinRackCoinKind = coinRacksWithMoney.First().Key;
            var biggestCoinRackIndex = coinRacksWithMoney.First().Value;
            var biggestCoinRack = vendingmachine.Hardware.CoinRacks[biggestCoinRackIndex];

            changeNeeded = changeNeeded - biggestCoinRackCoinKind;
            biggestCoinRack.ReleaseCoin();
        }

        return 0;
    }



}