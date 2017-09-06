using System;
using System.Collections.Generic;
using Frontend2;
using Frontend2.Hardware;
using System.Collections;

public class VendingMachineFactory : IVendingMachineFactory
{
    public List<VendingMachine> vmFactory;
    private List<VendingMachineSoftware> vmSoftwares;

    public VendingMachineFactory()
    {
        vmFactory = new List<VendingMachine>();
        vmSoftwares = new List<VendingMachineSoftware>();
    }

    public int CreateVendingMachine(List<int> coinKinds, int selectionButtonCount, int coinRackCapacity, int popRackCapcity, int receptacleCapacity)
    {
        //Move coin kinds to an array to use it in creating a vending machine
        int[] coinArray = coinKinds.ToArray();

        //Create a vending machine 
        VendingMachine vm = new VendingMachine(coinArray, selectionButtonCount, coinRackCapacity, popRackCapcity, receptacleCapacity);

        //Add created vending machine to vending machine factory
        vmFactory.Add(vm);

        //Get index of vending machine within vending machine factory
        int vmIndex = vmFactory.IndexOf(vm);

        //Use index of vending machine to creat software for the vending machine
        VendingMachineSoftware vms = new VendingMachineSoftware(vmFactory, vmIndex);

        //Add software to a corresponding index for vending machine
        vmSoftwares.Add(vms);

        //Listen for events 
        vm.CoinSlot.CoinAccepted += vms.CoinSlot_CoinAccepted;

        return vmIndex;
    }

    public void ConfigureVendingMachine(int vmIndex, List<string> popNames, List<int> popCosts)
    {
        VendingMachine vm = vmFactory[vmIndex];
        vm.Configure(popNames, popCosts);
    }

    public void LoadCoins(int vmIndex, int coinKindIndex, List<Coin> coins)
    {
        //Get indicated vending maching
        VendingMachine vm = vmFactory[vmIndex];

        //Get coin kind from the given index
        int coinKind = vm.GetCoinKindForCoinRack(coinKindIndex);

        //Get the coin chute based on the coin kind
        CoinRack loadingCoinRack = vm.GetCoinRackForCoinKind(coinKind);

        //Load the rack with the coins in the list
        loadingCoinRack.LoadCoins(coins);
    }

    public void LoadPops(int vmIndex, int popKindIndex, List<PopCan> pops)
    {
        VendingMachine vm = vmFactory[vmIndex];
        PopCanRack loadingPopRack = vm.PopCanRacks[popKindIndex];
        loadingPopRack.LoadPops(pops);
    }

    public void InsertCoin(int vmIndex, Coin coin)
    {
        VendingMachine vm = vmFactory[vmIndex];
        vm.CoinSlot.AddCoin(coin);
    }

    public void PressButton(int vmIndex, int value)
    {
        VendingMachine vm = vmFactory[vmIndex];
        VendingMachineSoftware vms = vmSoftwares[vmIndex];
        PopCanRack toDispense = vm.PopCanRacks[value];
        int popCost = vm.PopCanCosts[value];

        if(vms.totalInserted >= popCost)
        {
            toDispense.DispensePopCan();

            List<Coin> unloaded = vm.CoinReceptacle.Unload();
            foreach (Coin c in unloaded)
            {
                int coinVal = c.Value;
                CoinRack addingTo = vm.GetCoinRackForCoinKind(coinVal);
                if (addingTo.Capacity <= addingTo.Count)
                {
                    vm.StorageBin.AcceptCoin(c);   
                }
                else
                {
                    addingTo.AcceptCoin(c);
                }
            }
            if (vms.totalInserted > popCost)
                vms.makeChange(vms.totalInserted, popCost);
        }
    }

    public List<IDeliverable> ExtractFromDeliveryChute(int vmIndex)
    {
        VendingMachine vm = vmFactory[vmIndex];
        IDeliverable[] extracted = vm.DeliveryChute.RemoveItems();

        List<IDeliverable> extractedList = new List<IDeliverable>();
        foreach(var i in extracted)
        { 
            extractedList.Add(i);
        }
        return extractedList;
    }

    public VendingMachineStoredContents UnloadVendingMachine(int vmIndex)
    {
        VendingMachine vm = vmFactory[vmIndex];
        VendingMachineStoredContents vmsc = new VendingMachineStoredContents();

        foreach(CoinRack cr in vm.CoinRacks)
        {
            List<Coin> unloadingC = cr.Unload();
            vmsc.CoinsInCoinRacks.Add(unloadingC);
        }

        foreach(PopCanRack pcr in vm.PopCanRacks)
        {
            List<PopCan> unloadingP = pcr.Unload();
            vmsc.PopCansInPopCanRacks.Add(unloadingP);
        }

        List<Coin> sbCoins = vm.StorageBin.Unload();
        foreach(Coin c in sbCoins)
        {
            vmsc.PaymentCoinsInStorageBin.Add(c);
        }
        return vmsc;
    }
}

internal class VendingMachineSoftware
{
    public int totalInserted = 0;
    VendingMachine vm;
    public VendingMachineSoftware(List<VendingMachine> vmf, int vmIndex)
    {
        vm = vmf[vmIndex];
    }

    public void CoinSlot_CoinAccepted(object sender, CoinEventArgs e)
    {
        int addAmount = e.Coin.Value;
        totalInserted = totalInserted + addAmount;
    }

    public void makeChange(int inserted, int cost)
    {
        var coinsInChute = new List<Coin>();
        var changeNeeded = inserted - cost;
        List<int> changeCoins = new List<int>();
        int count = 0;
        while (count < vm.CoinRacks.Length)
        {
            int coinKind = vm.GetCoinKindForCoinRack(count);
            CoinRack cr = vm.GetCoinRackForCoinKind(coinKind);

            if ((coinKind <= changeNeeded) && (cr.Count > 0))
            {
                changeCoins.Add(coinKind);
            }
            count++;
        }
        changeCoins.Sort();
        changeCoins.Reverse();
        if (changeCoins.Count == 0)
        {
            //Console.WriteLine("Woohoo! Customer is screwed out of " + changeNeeded + " pennies!");
            return;
        }
        foreach(int i in changeCoins)
        {
            CoinRack current = vm.GetCoinRackForCoinKind(i);
            while((changeNeeded >= i) && (current.Count != 0))
            {
                current.ReleaseCoin();
                changeNeeded = changeNeeded - i;
            }
        }
        totalInserted = 0;
    }
}