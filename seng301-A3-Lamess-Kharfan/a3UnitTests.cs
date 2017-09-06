using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Frontend2;
using Frontend2.Hardware;

[TestClass]
public class a3UnitTests
{
    /*GoodTestScript1() mimicks T01-good-insert-and-press-exact-change test script */
    [TestMethod]
    public void GoodTestScript1()
    {
        //CREATE(5, 10, 25, 100; 3; 10; 10; 10)
        int[] coinKinds = { 5, 10, 25, 100 };
        int numPops = 3;
        int capacity = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //CONFIGURE([0] "Coke", 250; "water", 250; "stuff", 205)
        List<string> popNames = new List<string> { "Coke", "water", "stuff" };
        List<int> popCosts = new List<int> { 250, 250, 205 };
        vm.Configure(popNames, popCosts);

        //COIN_LOAD([0] 0; 5, 1)
        Coin c1 = new Coin(5);
        List<Coin> coinLoad1 = new List<Coin> { c1 };
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 10, 1)
        Coin c2 = new Coin(10);
        List<Coin> coinLoad2 = new List<Coin> { c2 };
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 2)
        Coin c3 = new Coin(25);
        List<Coin> coinLoad3 = new List<Coin> { c3, c3 };
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 100, 0)
        Coin c4 = new Coin(100);
        List<Coin> coinLoad4 = new List<Coin> { };
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "Coke", 1)
        PopCan p1 = new PopCan("Coke");
        List<PopCan> popLoad1 = new List<PopCan> { p1 };
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //POP_LOAD([0] 1; "water", 1)
        PopCan p2 = new PopCan("water");
        List<PopCan> popLoad2 = new List<PopCan> { p2 };
        vm.PopCanRacks[1].LoadPops(popLoad2);

        //POP_LOAD([0] 2; "stuff", 1)
        PopCan p3 = new PopCan("stuff");
        List<PopCan> popLoad3 = new List<PopCan> { p3 };
        vm.PopCanRacks[2].LoadPops(popLoad3);

        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);
        //INSERT([0] 25)
        vm.CoinSlot.AddCoin(c3);
        //INSERT([0] 25)
        vm.CoinSlot.AddCoin(c3);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        var items = vm.DeliveryChute.RemoveItems();
        var itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(0, "Coke")
        int coinTotalValue = 0;
        List<PopCan> popsDelivered = new List<PopCan> { p1 };
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else if (item is PopCan)
            {
                if (popsDelivered.Contains((PopCan)item))
                {
                    popsDelivered.Remove((PopCan)item);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(coinTotalValue, 0);
        Assert.AreEqual(popsDelivered.Count, 0);

        //UNLOAD([0])
        var vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(315; 0; "water", "stuff")
        var coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        var coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        var unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        int totalChangeRemaining = 315;
        int totalCoinsUsed = 0;
        List<PopCan> popsRemaining = new List<PopCan> { p2, p3 };

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        foreach (var popCanRack in unsoldPopCanRacks)
        {
            foreach (var popCan in popCanRack)
            {
                if (popsRemaining.Contains((PopCan)popCan))
                {
                    popsRemaining.Remove((PopCan)popCan);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
        Assert.AreEqual(popsRemaining.Count, 0);
    }

    /*GoodTestScript2() mimicks T02-good-insert-and-pres-change-expected test script */
    [TestMethod]
    public void GoodTestScript2()
    {
        //CREATE(5, 10, 25, 100; 3; 10; 10; 10)
        int[] coinKinds = { 5, 10, 25, 100 };
        int numPops = 3;
        int capacity = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //CONFIGURE([0] "Coke", 250; "water", 250; "stuff", 205)
        List<string> popNames = new List<string> { "Coke", "water", "stuff" };
        List<int> popCosts = new List<int> { 250, 250, 205 };
        vm.Configure(popNames, popCosts);

        //COIN_LOAD([0] 0; 5, 1)
        Coin c1 = new Coin(5);
        List<Coin> coinLoad1 = new List<Coin> { c1 };
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 10, 1)
        Coin c2 = new Coin(10);
        List<Coin> coinLoad2 = new List<Coin> { c2 };
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 2)
        Coin c3 = new Coin(25);
        List<Coin> coinLoad3 = new List<Coin> { c3, c3 };
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 100, 0)
        Coin c4 = new Coin(100);
        List<Coin> coinLoad4 = new List<Coin> { };
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "Coke", 1)
        PopCan p1 = new PopCan("Coke");
        List<PopCan> popLoad1 = new List<PopCan> { p1 };
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //POP_LOAD([0] 1; "water", 1)
        PopCan p2 = new PopCan("water");
        List<PopCan> popLoad2 = new List<PopCan> { p2 };
        vm.PopCanRacks[1].LoadPops(popLoad2);

        //POP_LOAD([0] 2; "stuff", 1)
        PopCan p3 = new PopCan("stuff");
        List<PopCan> popLoad3 = new List<PopCan> { p3 };
        vm.PopCanRacks[2].LoadPops(popLoad3);

        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        var items = vm.DeliveryChute.RemoveItems();
        var itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(50, "Coke")
        int coinTotalValue = 50;
        List<PopCan> popsDelivered = new List<PopCan> { p1 };
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else if (item is PopCan)
            {
                if (popsDelivered.Contains((PopCan)item))
                {
                    popsDelivered.Remove((PopCan)item);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(coinTotalValue, 0);
        Assert.AreEqual(popsDelivered.Count, 0);

        //UNLOAD([0])
        var vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(315; 0; "water", "stuff")
        var coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        var coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        var unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        int totalChangeRemaining = 315;
        int totalCoinsUsed = 0;
        List<PopCan> popsRemaining = new List<PopCan> { p2, p3 };

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        foreach (var popCanRack in unsoldPopCanRacks)
        {
            foreach (var popCan in popCanRack)
            {
                if (popsRemaining.Contains((PopCan)popCan))
                {
                    popsRemaining.Remove((PopCan)popCan);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
        Assert.AreEqual(popsRemaining.Count, 0);
    }

    /*GoodTestScript3() mimicks T03-good-teardown-without-configure-or-load test script */
    [TestMethod]
    public void GoodTestScript3()
    {
        //CREATE(5, 10, 25, 100; 3; 10; 10; 10)
        int[] coinKinds = { 5, 10, 25, 100 };
        int numPops = 3;
        int capacity = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //EXTRACT([0])
        var items = vm.DeliveryChute.RemoveItems();
        var itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(0)
        int coinTotalValue = 0;
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else
            {
                break;
            }

        }
        Assert.AreEqual(coinTotalValue, 0);

        //UNLOAD([0])
        var vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(0; 0)
        var coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        var coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        int totalChangeRemaining = 0;
        int totalCoinsUsed = 0;

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
    }

    /*GoodTestScript4() mimicks T04-good-press-without-insert test script */
    [TestMethod]
    public void GoodTestScript4()
    {

        //CREATE(5, 10, 25, 100; 3; 10; 10; 10)
        int[] coinKinds = { 5, 10, 25, 100 };
        int numPops = 3;
        int capacity = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //CONFIGURE([0] "Coke", 250; "water", 250; "stuff", 205)
        List<string> popNames = new List<string> { "Coke", "water", "stuff" };
        List<int> popCosts = new List<int> { 250, 250, 205 };
        vm.Configure(popNames, popCosts);

        //COIN_LOAD([0] 0; 5, 1)
        Coin c1 = new Coin(5);
        List<Coin> coinLoad1 = new List<Coin> { c1 };
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 10, 1)
        Coin c2 = new Coin(10);
        List<Coin> coinLoad2 = new List<Coin> { c2 };
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 2)
        Coin c3 = new Coin(25);
        List<Coin> coinLoad3 = new List<Coin> { c3, c3 };
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 100, 0)
        Coin c4 = new Coin(100);
        List<Coin> coinLoad4 = new List<Coin> { };
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "Coke", 1)
        PopCan p1 = new PopCan("Coke");
        List<PopCan> popLoad1 = new List<PopCan> { p1 };
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //POP_LOAD([0] 1; "water", 1)
        PopCan p2 = new PopCan("water");
        List<PopCan> popLoad2 = new List<PopCan> { p2 };
        vm.PopCanRacks[1].LoadPops(popLoad2);

        //POP_LOAD([0] 2; "stuff", 1)
        PopCan p3 = new PopCan("stuff");
        List<PopCan> popLoad3 = new List<PopCan> { p3 };
        vm.PopCanRacks[2].LoadPops(popLoad3);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        var items = vm.DeliveryChute.RemoveItems();
        var itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(0)
        int coinTotalValue = 0;
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else
            {
                break;
            }
        }
        Assert.AreEqual(coinTotalValue, 0);

        //UNLOAD([0])
        var vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(65; 0; "Coke", "water", "stuff")
        var coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        var coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        var unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        int totalChangeRemaining = 65;
        int totalCoinsUsed = 0;
        List<PopCan> popsRemaining = new List<PopCan> { p1, p2, p3 };

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        foreach (var popCanRack in unsoldPopCanRacks)
        {
            foreach (var popCan in popCanRack)
            {
                if (popsRemaining.Contains((PopCan)popCan))
                {
                    popsRemaining.Remove((PopCan)popCan);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
        Assert.AreEqual(popsRemaining.Count, 0);
    }

    /*GoodTestScript5() mimicks T05-good-scrambled-coin-kinds test script */
    [TestMethod]
    public void GoodTestScript5()
    {
        //CREATE(100, 5, 25, 10; 3; 2; 10; 10)
        int[] coinKinds = { 100, 5, 25, 10 };
        int numPops = 3;
        int capacity1 = 2;
        int capacity2 = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity1, capacity2, capacity2);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //CONFIGURE([0] "Coke", 250; "water", 250; "stuff", 205)
        List<string> popNames = new List<string> { "Coke", "water", "stuff" };
        List<int> popCosts = new List<int> { 250, 250, 205 };
        vm.Configure(popNames, popCosts);

        //COIN_LOAD([0] 0; 100, 0)
        Coin c1 = new Coin(100);
        List<Coin> coinLoad1 = new List<Coin> {};
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 5, 1)
        Coin c2 = new Coin(5);
        List<Coin> coinLoad2 = new List<Coin> { c2 };
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 2)
        Coin c3 = new Coin(25);
        List<Coin> coinLoad3 = new List<Coin> { c3, c3 };
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 10, 1)
        Coin c4 = new Coin(10);
        List<Coin> coinLoad4 = new List<Coin> { c4 };
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "Coke", 1)
        PopCan p1 = new PopCan("Coke");
        List<PopCan> popLoad1 = new List<PopCan> { p1 };
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //POP_LOAD([0] 1; "water", 1)
        PopCan p2 = new PopCan("water");
        List<PopCan> popLoad2 = new List<PopCan> { p2 };
        vm.PopCanRacks[1].LoadPops(popLoad2);

        //POP_LOAD([0] 2; "stuff", 1)
        PopCan p3 = new PopCan("stuff");
        List<PopCan> popLoad3 = new List<PopCan> { p3 };
        vm.PopCanRacks[2].LoadPops(popLoad3);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        var items = vm.DeliveryChute.RemoveItems();
        var itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(0)
        int coinTotalValue = 0;
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else
            {
                break;
            }
        }
        Assert.AreEqual(coinTotalValue, 0);

        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c1);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c1);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c1);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        var items2 = vm.DeliveryChute.RemoveItems();
        var itemsAsList2 = new List<IDeliverable>(items2);

        //CHECK_DELIVERY(50, "Coke")
        int coinTotalValue2 = 50;
        List<PopCan> popsDelivered2 = new List<PopCan> { p1 };
        foreach (var item in itemsAsList2)
        {
            if (item is Coin)
            {
                coinTotalValue2 -= ((Coin)item).Value;
            }
            else if (item is PopCan)
            {
                if (popsDelivered2.Contains((PopCan)item))
                {
                    popsDelivered2.Remove((PopCan)item);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(coinTotalValue2, 0);
        Assert.AreEqual(popsDelivered2.Count, 0);

        //UNLOAD([0])
        var vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(215; 100; "water", "stuff")
        var coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        var coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        var unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        int totalChangeRemaining = 215;
        int totalCoinsUsed = 100;
        List<PopCan> popsRemaining = new List<PopCan> { p2, p3 };

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        foreach (var popCanRack in unsoldPopCanRacks)
        {
            foreach (var popCan in popCanRack)
            {
                if (popsRemaining.Contains((PopCan)popCan))
                {
                    popsRemaining.Remove((PopCan)popCan);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
        Assert.AreEqual(popsRemaining.Count, 0);
    }
    /*GoodTestScript6() mimicks T06-good-extract-before-sale test script */
    [TestMethod]
    public void GoodTestScript6()
    {
        //CREATE(100, 5, 25, 10; 3; 10; 10; 10)
        int[] coinKinds = { 100, 5, 25, 10 };
        int numPops = 3;
        int capacity = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //CONFIGURE([0] "Coke", 250; "water", 250; "stuff", 205)
        List<string> popNames = new List<string> { "Coke", "water", "stuff" };
        List<int> popCosts = new List<int> { 250, 250, 205 };
        vm.Configure(popNames, popCosts);

        //COIN_LOAD([0] 0; 100, 0)
        Coin c1 = new Coin(100);
        List<Coin> coinLoad1 = new List<Coin> { };
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 5, 1)
        Coin c2 = new Coin(5);
        List<Coin> coinLoad2 = new List<Coin> { c2 };
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 2)
        Coin c3 = new Coin(25);
        List<Coin> coinLoad3 = new List<Coin> { c3, c3 };
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 10, 1)
        Coin c4 = new Coin(10);
        List<Coin> coinLoad4 = new List<Coin> { c4 };
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "Coke", 1)
        PopCan p1 = new PopCan("Coke");
        List<PopCan> popLoad1 = new List<PopCan> { p1 };
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //POP_LOAD([0] 1; "water", 1)
        PopCan p2 = new PopCan("water");
        List<PopCan> popLoad2 = new List<PopCan> { p2 };
        vm.PopCanRacks[1].LoadPops(popLoad2);

        //POP_LOAD([0] 2; "stuff", 1)
        PopCan p3 = new PopCan("stuff");
        List<PopCan> popLoad3 = new List<PopCan> { p3 };
        vm.PopCanRacks[2].LoadPops(popLoad3);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        var items = vm.DeliveryChute.RemoveItems();
        var itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(0)
        int coinTotalValue = 0;
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else
            {
                break;
            }
        }
        Assert.AreEqual(coinTotalValue, 0);

        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c1);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c1);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c1);

        //EXTRACT([0])
        var items2 = vm.DeliveryChute.RemoveItems();
        var itemsAsList2 = new List<IDeliverable>(items2);

        //CHECK_DELIVERY(0)
        int coinTotalValue2 = 0;
        foreach (var item in itemsAsList2)
        {
            if (item is Coin)
            {
                coinTotalValue2 -= ((Coin)item).Value;
            }
            else
            {
                break;
            }
        }
        Assert.AreEqual(coinTotalValue2, 0);

        //UNLOAD([0])
        var vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(65; 0; "Coke", "water", "stuff")
        var coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        var coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        var unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        int totalChangeRemaining = 65;
        int totalCoinsUsed = 0;
        List<PopCan> popsRemaining = new List<PopCan> { p1, p2, p3 };

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        foreach (var popCanRack in unsoldPopCanRacks)
        {
            foreach (var popCan in popCanRack)
            {
                if (popsRemaining.Contains((PopCan)popCan))
                {
                    popsRemaining.Remove((PopCan)popCan);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
        Assert.AreEqual(popsRemaining.Count, 0);
    }

    /*GoodTestScript7() mimicks T07-good-changing-configuration test script */
    [TestMethod]
    public void GoodTestScript7()
    {
        //CREATE(5, 10, 25, 100; 3; 10; 10; 10)
        int[] coinKinds = { 5, 10, 25, 100 };
        int numPops = 3;
        int capacity = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //CONFIGURE([0] "A", 5; "B", 10; "C", 25)
        List<string> popNames = new List<string> { "A", "B", "C" };
        List<int> popCosts = new List<int> { 5, 10, 25 };
        vm.Configure(popNames, popCosts);

        //COIN_LOAD([0] 0; 5, 1)
        Coin c1 = new Coin(5);
        List<Coin> coinLoad1 = new List<Coin> { c1 };
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 10, 1)
        Coin c2 = new Coin(10);
        List<Coin> coinLoad2 = new List<Coin> { c2 };
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 2)
        Coin c3 = new Coin(25);
        List<Coin> coinLoad3 = new List<Coin> { c3, c3 };
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 100, 0)
        Coin c4 = new Coin(100);
        List<Coin> coinLoad4 = new List<Coin> { };
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "A", 1)
        PopCan p1 = new PopCan("A");
        List<PopCan> popLoad1 = new List<PopCan> { p1 };
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //POP_LOAD([0] 1; "B", 1)
        PopCan p2 = new PopCan("B");
        List<PopCan> popLoad2 = new List<PopCan> { p2 };
        vm.PopCanRacks[1].LoadPops(popLoad2);

        //POP_LOAD([0] 2; "C", 1)
        PopCan p3 = new PopCan("C");
        List<PopCan> popLoad3 = new List<PopCan> { p3 };
        vm.PopCanRacks[2].LoadPops(popLoad3);

        //CONFIGURE([0] "Coke", 250; "water", 250; "stuff", 205)
        popNames = new List<string> { "Coke", "water", "stuff" };
        popCosts = new List<int> { 250, 250, 205 };
        vm.Configure(popNames, popCosts);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        var items = vm.DeliveryChute.RemoveItems();
        var itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(0)
        int coinTotalValue = 0;
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else
            {
                break;
            }
        }
        Assert.AreEqual(coinTotalValue, 0);

        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        items = vm.DeliveryChute.RemoveItems();
        itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(50, "A")
        coinTotalValue = 50;
        List<PopCan> popsDelivered = new List<PopCan> { p1 };
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else if (item is PopCan)
            {
                if (popsDelivered.Contains((PopCan)item))
                {
                    popsDelivered.Remove((PopCan)item);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(coinTotalValue, 0);
        Assert.AreEqual(popsDelivered.Count, 0);

        //UNLOAD([0])
        var vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(315; 0; "B", "C")
        var coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        var coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        var unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        int totalChangeRemaining = 315;
        int totalCoinsUsed = 0;
        List<PopCan> popsRemaining = new List<PopCan> { p2, p3 };

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        foreach (var popCanRack in unsoldPopCanRacks)
        {
            foreach (var popCan in popCanRack)
            {
                if (popsRemaining.Contains((PopCan)popCan))
                {
                    popsRemaining.Remove((PopCan)popCan);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
        Assert.AreEqual(popsRemaining.Count, 0);

        //COIN_LOAD([0] 0; 5, 1)
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 10, 1)
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 2)
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 100, 0)
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "Coke", 1)
        PopCan pop1 = new PopCan("Coke");
        popLoad1 = new List<PopCan> { pop1 };
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //POP_LOAD([0] 1; "water", 1)
        PopCan pop2 = new PopCan("water");
        popLoad2 = new List<PopCan> { pop2 };
        vm.PopCanRacks[1].LoadPops(popLoad2);

        //POP_LOAD([0] 2; "stuff", 1)
        PopCan pop3 = new PopCan("stuff");
        popLoad3 = new List<PopCan> { pop3 };
        vm.PopCanRacks[2].LoadPops(popLoad3);

        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        items = vm.DeliveryChute.RemoveItems();
        itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(50, "Coke")
        coinTotalValue = 50;
        popsDelivered = new List<PopCan> { pop1 };
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else if (item is PopCan)
            {
                if (popsDelivered.Contains((PopCan)item))
                {
                    popsDelivered.Remove((PopCan)item);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(coinTotalValue, 0);
        Assert.AreEqual(popsDelivered.Count, 0);

        //UNLOAD([0])
        vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(315; 0; "water", "stuff")
        coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        totalChangeRemaining = 315;
        totalCoinsUsed = 0;
        popsRemaining = new List<PopCan> { pop2, pop3 };

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        foreach (var popCanRack in unsoldPopCanRacks)
        {
            foreach (var popCan in popCanRack)
            {
                if (popsRemaining.Contains((PopCan)popCan))
                {
                    popsRemaining.Remove((PopCan)popCan);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
        Assert.AreEqual(popsRemaining.Count, 0);
    }

    /*GoodTestScript8() mimicks T08-good-approximate-change test script */
    [TestMethod]
    public void GoodTestScript8()
    {
        //CREATE(5, 10, 25, 100; 1; 10; 10; 10)
        int[] coinKinds = { 5, 10, 25, 100 };
        int numPops = 1;
        int capacity = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //CONFIGURE([0] "stuff", 140)
        List<string> popNames = new List<string> { "stuff" };
        List<int> popCosts = new List<int> { 140 };
        vm.Configure(popNames, popCosts);

        //COIN_LOAD([0] 0; 5, 0)
        Coin c1 = new Coin(5);
        List<Coin> coinLoad1 = new List<Coin> { };
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 10, 5)
        Coin c2 = new Coin(10);
        List<Coin> coinLoad2 = new List<Coin> { c2, c2, c2, c2, c2 };
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 1)
        Coin c3 = new Coin(25);
        List<Coin> coinLoad3 = new List<Coin> { c3 };
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 100, 1)
        Coin c4 = new Coin(100);
        List<Coin> coinLoad4 = new List<Coin> { c4 };
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "stuff", 1)
        PopCan p1 = new PopCan("stuff");
        List<PopCan> popLoad1 = new List<PopCan> { p1 };
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        var items = vm.DeliveryChute.RemoveItems();
        var itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(155, "stuff")
        int coinTotalValue = 155;
        List<PopCan> popsDelivered = new List<PopCan> { p1 };
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else if (item is PopCan)
            {
                if (popsDelivered.Contains((PopCan)item))
                {
                    popsDelivered.Remove((PopCan)item);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(coinTotalValue, 0);
        Assert.AreEqual(popsDelivered.Count, 0);

        //UNLOAD([0])
        var vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(320; 0)
        var coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        var coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        var unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        var totalChangeRemaining = 320;
        var totalCoinsUsed = 0;
        var popsRemaining = new List<PopCan> { };

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        foreach (var popCanRack in unsoldPopCanRacks)
        {
            foreach (var popCan in popCanRack)
            {
                if (popsRemaining.Contains((PopCan)popCan))
                {
                    popsRemaining.Remove((PopCan)popCan);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
        Assert.AreEqual(popsRemaining.Count, 0);
    }

    /*GoodTestScript9() mimicks T09-good-hard-for-change test script */
    [TestMethod]
    public void GoodTestScript9()
    {
        //CREATE(5, 10, 25, 100; 1; 10; 10; 10)
        int[] coinKinds = { 5, 10, 25, 100 };
        int numPops = 1;
        int capacity = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //CONFIGURE([0] "stuff", 140)
        List<string> popNames = new List<string> { "stuff" };
        List<int> popCosts = new List<int> { 140 };
        vm.Configure(popNames, popCosts);

        //COIN_LOAD([0] 0; 5, 1)
        Coin c1 = new Coin(5);
        List<Coin> coinLoad1 = new List<Coin> { c1 };
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 10, 6)
        Coin c2 = new Coin(10);
        List<Coin> coinLoad2 = new List<Coin> { c2, c2, c2, c2, c2, c2 };
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 1)
        Coin c3 = new Coin(25);
        List<Coin> coinLoad3 = new List<Coin> { c3 };
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 100, 1)
        Coin c4 = new Coin(100);
        List<Coin> coinLoad4 = new List<Coin> { c4 };
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "stuff", 1)
        PopCan p1 = new PopCan("stuff");
        List<PopCan> popLoad1 = new List<PopCan> { p1 };
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        var items = vm.DeliveryChute.RemoveItems();
        var itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(160, "stuff")
        int coinTotalValue = 160;
        List<PopCan> popsDelivered = new List<PopCan> { p1 };
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else if (item is PopCan)
            {
                if (popsDelivered.Contains((PopCan)item))
                {
                    popsDelivered.Remove((PopCan)item);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(coinTotalValue, 0);
        Assert.AreEqual(popsDelivered.Count, 0);

        //UNLOAD([0])
        var vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(330; 0)
        var coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        var coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        var unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        var totalChangeRemaining = 330;
        var totalCoinsUsed = 0;
        var popsRemaining = new List<PopCan> { };

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        foreach (var popCanRack in unsoldPopCanRacks)
        {
            foreach (var popCan in popCanRack)
            {
                if (popsRemaining.Contains((PopCan)popCan))
                {
                    popsRemaining.Remove((PopCan)popCan);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
        Assert.AreEqual(popsRemaining.Count, 0);
    }

    /*GoodTestScript10() mimicks T10-good-invalid-coin test script */
    [TestMethod]
    public void GoodTestScript10()
    {
        //CREATE(5, 10, 25, 100; 1; 10; 10; 10)
        int[] coinKinds = { 5, 10, 25, 100 };
        int numPops = 1;
        int capacity = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //CONFIGURE([0] "stuff", 140)
        List<string> popNames = new List<string> { "stuff" };
        List<int> popCosts = new List<int> { 140 };
        vm.Configure(popNames, popCosts);

        //COIN_LOAD([0] 0; 5, 1)
        Coin c1 = new Coin(5);
        List<Coin> coinLoad1 = new List<Coin> { c1 };
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 10, 6)
        Coin c2 = new Coin(10);
        List<Coin> coinLoad2 = new List<Coin> { c2, c2, c2, c2, c2, c2 };
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 1)
        Coin c3 = new Coin(25);
        List<Coin> coinLoad3 = new List<Coin> { c3 };
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 100, 1)
        Coin c4 = new Coin(100);
        List<Coin> coinLoad4 = new List<Coin> { c4 };
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "stuff", 1)
        PopCan p1 = new PopCan("stuff");
        List<PopCan> popLoad1 = new List<PopCan> { p1 };
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //INSERT([0] 1)
        Coin c5 = new Coin(1);
        vm.CoinSlot.AddCoin(c5);

        //INSERT([0] 139)
        Coin c6 = new Coin(139);
        vm.CoinSlot.AddCoin(c6);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        var items = vm.DeliveryChute.RemoveItems();
        var itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(140)
        int coinTotalValue = 140;
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else
            {
                break;
            }
            
        }
        Assert.AreEqual(coinTotalValue, 0);

        //UNLOAD([0])
        var vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(190; 0; "stuff")
        var coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        var coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        var unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        var totalChangeRemaining = 190;
        var totalCoinsUsed = 0;
        var popsRemaining = new List<PopCan> { p1 };

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        foreach (var popCanRack in unsoldPopCanRacks)
        {
            foreach (var popCan in popCanRack)
            {
                if (popsRemaining.Contains((PopCan)popCan))
                {
                    popsRemaining.Remove((PopCan)popCan);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
        Assert.AreEqual(popsRemaining.Count, 0);
    }

    /*GoodTestScript11() mimicks T11-good-extract-before-sale-complex test script */
    [TestMethod]
    public void GoodTestScript11()
    {
        //CREATE(100, 5, 25, 10; 3; 10; 10; 10)
        int[] coinKinds = { 100, 5, 25, 10 };
        int numPops = 3;
        int capacity = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //CONFIGURE([0] "Coke", 250; "water", 250; "stuff", 205)
        List<string> popNames = new List<string> { "Coke", "water", "stuff" };
        List<int> popCosts = new List<int> { 250, 250, 205 };
        vm.Configure(popNames, popCosts);

        //COIN_LOAD([0] 0; 100, 0)
        Coin c1 = new Coin(100);
        List<Coin> coinLoad1 = new List<Coin> { };
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 5, 1)
        Coin c2 = new Coin(5);
        List<Coin> coinLoad2 = new List<Coin> { c2 };
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 2)
        Coin c3 = new Coin(25);
        List<Coin> coinLoad3 = new List<Coin> { c3, c3 };
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 10, 1)
        Coin c4 = new Coin(10);
        List<Coin> coinLoad4  = new List<Coin> { c4 };
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "Coke", 1)
        PopCan p1 = new PopCan("Coke");
        List<PopCan> popLoad1 = new List<PopCan> { p1 };
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //POP_LOAD([0] 1; "water", 1)
        PopCan p2 = new PopCan("water");
        List<PopCan> popLoad2 = new List<PopCan> { p2 };
        vm.PopCanRacks[1].LoadPops(popLoad2);

        //POP_LOAD([0] 2; "stuff", 1)
        PopCan p3 = new PopCan("stuff");
        List<PopCan> popLoad3 = new List<PopCan> { p3 };
        vm.PopCanRacks[2].LoadPops(popLoad3);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        var items = vm.DeliveryChute.RemoveItems();
        var itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(0)
        int coinTotalValue = 0;
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else
            {
                break;
            }
        }
        Assert.AreEqual(coinTotalValue, 0);

        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c1);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c1);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c1);

        //EXTRACT([0])
        items = vm.DeliveryChute.RemoveItems();
        itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(0)
        coinTotalValue = 0;
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else
            {
                break;
            }
        }
        Assert.AreEqual(coinTotalValue, 0);

        //UNLOAD([0])
        var vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(65; 0; "Coke", "water", "stuff")
        var coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        var coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        var unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        var totalChangeRemaining = 65;
        var totalCoinsUsed = 0;
        var popsRemaining = new List<PopCan> { p1, p2, p3 };

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        foreach (var popCanRack in unsoldPopCanRacks)
        {
            foreach (var popCan in popCanRack)
            {
                if (popsRemaining.Contains((PopCan)popCan))
                {
                    popsRemaining.Remove((PopCan)popCan);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
        Assert.AreEqual(popsRemaining.Count, 0);

        //COIN_LOAD([0] 0; 100, 0)
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 5, 1)
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 2)
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 10, 1)
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "Coke", 1)
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //POP_LOAD([0] 1; "water", 1)
        vm.PopCanRacks[1].LoadPops(popLoad2);

        //POP_LOAD([0] 2; "stuff", 1)
        vm.PopCanRacks[2].LoadPops(popLoad3);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        items = vm.DeliveryChute.RemoveItems();
        itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(50, "Coke")
        coinTotalValue = 50;
        List<PopCan> popsDelivered = new List<PopCan> { p1 };
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else if (item is PopCan)
            {
                if (popsDelivered.Contains((PopCan)item))
                {
                    popsDelivered.Remove((PopCan)item);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(coinTotalValue, 0);
        Assert.AreEqual(popsDelivered.Count, 0);

        //UNLOAD([0])
        vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(315; 0; "water", "stuff")
        coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        totalChangeRemaining = 315;
        totalCoinsUsed = 0;
        popsRemaining = new List<PopCan> { p2, p3 };

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        foreach (var popCanRack in unsoldPopCanRacks)
        {
            foreach (var popCan in popCanRack)
            {
                if (popsRemaining.Contains((PopCan)popCan))
                {
                    popsRemaining.Remove((PopCan)popCan);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
        Assert.AreEqual(popsRemaining.Count, 0);

        //CREATE(100, 5, 25, 10; 3; 10; 10; 10)
        VendingMachine vm2 = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml2 = new VendingMachineLogic(vm2);

        //CONFIGURE([1] "Coke", 250; "water", 250; "stuff", 205)
        vm2.Configure(popNames, popCosts);

        //CONFIGURE([1] "A", 5; "B", 10; "C", 25)
        popNames = new List<string> { "A", "B", "C" };
        popCosts = new List<int> { 5, 10, 25 };
        vm2.Configure(popNames, popCosts);

        //UNLOAD([1])
        vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm2.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm2.StorageBin.Unload());
        foreach (var popCanRack in vm2.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(0; 0)
        coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        totalChangeRemaining = 0;
        totalCoinsUsed = 0;

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);

        //COIN_LOAD([1] 0; 100, 0)
        vm2.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([1] 1; 5, 1)
        vm2.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([1] 2; 25, 2)
        vm2.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([1] 3; 10, 1)
        vm2.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([1] 0; "A", 1)
        PopCan pop1 = new PopCan("A");
        popLoad1 = new List<PopCan> { pop1 };
        vm2.PopCanRacks[0].LoadPops(popLoad1);

        //POP_LOAD([1] 1; "B", 1)
        PopCan pop2 = new PopCan("B");
        popLoad2 = new List<PopCan> { pop2 };
        vm2.PopCanRacks[1].LoadPops(popLoad2);

        //POP_LOAD([1] 2; "C", 1)
        PopCan pop3 = new PopCan("C");
        popLoad3 = new List<PopCan> { pop3 };
        vm2.PopCanRacks[2].LoadPops(popLoad3);

        //INSERT([1] 10)
        vm2.CoinSlot.AddCoin(c4);
        //INSERT([1] 5)
        vm2.CoinSlot.AddCoin(c2);
        //INSERT([1] 10)
        vm2.CoinSlot.AddCoin(c4);

        //PRESS([1] 2)
        vm2.SelectionButtons[2].Press();

        //EXTRACT([1])
        items = vm2.DeliveryChute.RemoveItems();
        itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(0, "C")
        coinTotalValue = 0;
        popsDelivered = new List<PopCan> { pop3 };
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else if (item is PopCan)
            {
                if (popsDelivered.Contains((PopCan)item))
                {
                    popsDelivered.Remove((PopCan)item);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(coinTotalValue, 0);
        Assert.AreEqual(popsDelivered.Count, 0);

        //UNLOAD([1])
        //UNLOAD([1])
        vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm2.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm2.StorageBin.Unload());
        foreach (var popCanRack in vm2.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(90; 0; "A", "B")
        coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        totalChangeRemaining = 90;
        totalCoinsUsed = 0;
        popsRemaining = new List<PopCan> { pop1, pop2 };

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        foreach (var popCanRack in unsoldPopCanRacks)
        {
            foreach (var popCan in popCanRack)
            {
                if (popsRemaining.Contains((PopCan)popCan))
                {
                    popsRemaining.Remove((PopCan)popCan);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
        Assert.AreEqual(popsRemaining.Count, 0);
    }

    /*GoodTestScript12() mimicks T12-good-approximate-change-with-credit test script */
    [TestMethod]
    public void GoodTestScript12()
    {
        //CREATE(5, 10, 25, 100; 1; 10; 10; 10)
        int[] coinKinds = { 5, 10, 25, 100 };
        int numPops = 1;
        int capacity = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //CONFIGURE([0] "stuff", 140)
        List<string> popNames = new List<string> { "stuff" };
        List<int> popCosts = new List<int> { 140 };
        vm.Configure(popNames, popCosts);

        //COIN_LOAD([0] 0; 5, 0)
        Coin c1 = new Coin(5);
        List<Coin> coinLoad1 = new List<Coin> { };
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 10, 5)
        Coin c2 = new Coin(10);
        List<Coin> coinLoad2 = new List<Coin> { c2, c2, c2, c2, c2 };
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 1)
        Coin c3 = new Coin(25);
        List<Coin> coinLoad3 = new List<Coin> { c3 };
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 100, 1)
        Coin c4 = new Coin(100);
        List<Coin> coinLoad4 = new List<Coin> { c4 };
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "stuff", 1)
        PopCan p1 = new PopCan("stuff");
        List<PopCan> popLoad1 = new List<PopCan> { p1 };
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        var items = vm.DeliveryChute.RemoveItems();
        var itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(155, "stuff")
        int coinTotalValue = 155;
        List<PopCan> popsDelivered = new List<PopCan> { p1 };
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else if (item is PopCan)
            {
                if (popsDelivered.Contains((PopCan)item))
                {
                    popsDelivered.Remove((PopCan)item);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(coinTotalValue, 0);
        Assert.AreEqual(popsDelivered.Count, 0);

        //UNLOAD([0])
        var vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(320; 0)
        var coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        var coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        var unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        var totalChangeRemaining = 320;
        var totalCoinsUsed = 0;
        var popsRemaining = new List<PopCan> { };

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        foreach (var popCanRack in unsoldPopCanRacks)
        {
            foreach (var popCan in popCanRack)
            {
                if (popsRemaining.Contains((PopCan)popCan))
                {
                    popsRemaining.Remove((PopCan)popCan);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
        Assert.AreEqual(popsRemaining.Count, 0);

        //COIN_LOAD([0] 0; 5, 10)
        coinLoad1 = new List<Coin> { c1, c1, c1, c1, c1, c1, c1, c1, c1, c1 };
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 10, 10)
        coinLoad2 = new List<Coin> { c2, c2, c2, c2, c2, c2, c2, c2, c2, c2 };
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 10)
        coinLoad3 = new List<Coin> { c3, c3, c3, c3, c3, c3, c3, c3, c3, c3 };
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 100, 10)
        coinLoad4 = new List<Coin> { c4, c4, c4, c4, c4, c4, c4, c4, c4, c4  };
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "stuff", 1)
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //INSERT([0] 25)
        vm.CoinSlot.AddCoin(c3);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);
        //INSERT([0] 10)
        vm.CoinSlot.AddCoin(c2);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        items = vm.DeliveryChute.RemoveItems();
        itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(0, "stuff")
        coinTotalValue = 0;
        popsDelivered = new List<PopCan> { p1 };
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else if (item is PopCan)
            {
                if (popsDelivered.Contains((PopCan)item))
                {
                    popsDelivered.Remove((PopCan)item);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(coinTotalValue, 0);
        Assert.AreEqual(popsDelivered.Count, 0);

        //UNLOAD([0])
        vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(1400; 135)
        coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        totalChangeRemaining = 1400;
        totalCoinsUsed = 135;

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
    }

    /*GoodTestScript13() mimicks T13-good-need-to-store-payment test script */
    [TestMethod]
    public void GoodTestScript13()
    {
        //CREATE(5, 10, 25, 100; 1; 10; 10; 10)
        int[] coinKinds = { 5, 10, 25, 100 };
        int numPops = 1;
        int capacity = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //CONFIGURE([0] "stuff", 135)
        List<string> popNames = new List<string> { "stuff" };
        List<int> popCosts = new List<int> { 135 };
        vm.Configure(popNames, popCosts);

        //COIN_LOAD([0] 0; 5, 10)
        Coin c1 = new Coin(5);
        List<Coin> coinLoad1 = new List<Coin> { c1, c1, c1, c1, c1, c1, c1, c1, c1, c1 };
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 10, 10)
        Coin c2 = new Coin(10);
        List<Coin> coinLoad2 = new List<Coin> { c2, c2, c2, c2, c2, c2, c2, c2, c2, c2 };
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 10)
        Coin c3 = new Coin(25);
        List<Coin> coinLoad3 = new List<Coin> { c3, c3, c3, c3, c3, c3, c3, c3, c3, c3 };
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 100, 10)
        Coin c4 = new Coin(100);
        List<Coin> coinLoad4 = new List<Coin> { c4, c4, c4, c4, c4, c4, c4, c4, c4, c4 };
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "stuff", 1)
        PopCan p1 = new PopCan("stuff");
        List<PopCan> popLoad1 = new List<PopCan> { p1 };
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //INSERT([0] 25)
        vm.CoinSlot.AddCoin(c3);
        //INSERT([0] 100)
        vm.CoinSlot.AddCoin(c4);
        //INSERT([0] 10)
        vm.CoinSlot.AddCoin(c2);

        //PRESS([0] 0)
        vm.SelectionButtons[0].Press();

        //EXTRACT([0])
        var items = vm.DeliveryChute.RemoveItems();
        var itemsAsList = new List<IDeliverable>(items);

        //CHECK_DELIVERY(0, "stuff")
        int coinTotalValue = 0;
        List<PopCan> popsDelivered = new List<PopCan> { p1 };
        foreach (var item in itemsAsList)
        {
            if (item is Coin)
            {
                coinTotalValue -= ((Coin)item).Value;
            }
            else if (item is PopCan)
            {
                if (popsDelivered.Contains((PopCan)item))
                {
                    popsDelivered.Remove((PopCan)item);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(coinTotalValue, 0);
        Assert.AreEqual(popsDelivered.Count, 0);

        //UNLOAD([0])
        var vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(1400; 135)
        var coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        var coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        var unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        var totalChangeRemaining = 1400;
        var totalCoinsUsed = 135;

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
    }
    
    /*BadTestScript1() mimicks U01-bad-configure-before-construct test script*/
    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public void BadTestScript1()
    {
        //CONFIGURE([0] "Coke", 250; "water", 250; "stuff", 205) // This SHOULD cause an error, but DOES NOT!
        VendingMachine vm = null; 
        VendingMachineLogic vml = null;
        List<string> popNames = new List<string> { "Coke", "water", "stuff" };
        List<int> popCosts = new List<int> { 250, 250, 205 };
        vm.Configure(popNames, popCosts);

        //CREATE(5, 10, 25, 100; 3; 10; 10; 10)
        int[] coinKinds = { 5, 10, 25, 100 };
        int numPops = 3;
        int capacity = 10;
        vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        vml = new VendingMachineLogic(vm);

        //COIN_LOAD([0] 0; 5, 1)
        Coin c1 = new Coin(5);
        List<Coin> coinLoad1 = new List<Coin> { c1 };
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 10, 1)
        Coin c2 = new Coin(10);
        List<Coin> coinLoad2 = new List<Coin> { c2 };
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 2)
        Coin c3 = new Coin(25);
        List<Coin> coinLoad3 = new List<Coin> { c3, c3 };
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 100, 0)
        Coin c4 = new Coin(100);
        List<Coin> coinLoad4 = new List<Coin> { };
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "Coke", 1)
        PopCan p1 = new PopCan("Coke");
        List<PopCan> popLoad1 = new List<PopCan> { p1 };
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //POP_LOAD([0] 1; "water", 1)
        PopCan p2 = new PopCan("water");
        List<PopCan> popLoad2 = new List<PopCan> { p2 };
        vm.PopCanRacks[1].LoadPops(popLoad2);

        //POP_LOAD([0] 2; "stuff", 1)
        PopCan p3 = new PopCan("stuff");
        List<PopCan> popLoad3 = new List<PopCan> { p3 };
        vm.PopCanRacks[2].LoadPops(popLoad3);

        //UNLOAD([0])
        var vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(65; 0; "Coke", "water", "stuff") // This causes an error for the dummy but should not
        var coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        var coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        var unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        var totalChangeRemaining = 65;
        var totalCoinsUsed = 0;
        var popsRemaining = new List<PopCan> { p1, p2, p3 };

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        foreach (var popCanRack in unsoldPopCanRacks)
        {
            foreach (var popCan in popCanRack)
            {
                if (popsRemaining.Contains((PopCan)popCan))
                {
                    popsRemaining.Remove((PopCan)popCan);
                }
                else
                {
                    break;
                }
            }
        }
        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
        Assert.AreEqual(popsRemaining.Count, 0);
    }

    /*BadTestScript2() mimicks U02-bad-cost-list test script*/
    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void BadTestScript2()
    {
        //CREATE(5, 10, 25, 100; 3; 10; 10; 10)
        int[] coinKinds = { 5, 10, 25, 100 };
        int numPops = 3;
        int capacity = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //CONFIGURE([0] "Coke", 250; "water", 250; "stuff", 0 /* the cost of "stuff" is zero */) // This SHOULD cause an error, but DOES NOT in the dummy!
        List<string> popNames = new List<string> { "Coke", "water", "stuff" };
        List<int> popCosts = new List<int> { 250, 250, 0 };
        vm.Configure(popNames, popCosts);

        //COIN_LOAD([0] 0; 5, 1)
        Coin c1 = new Coin(5);
        List<Coin> coinLoad1 = new List<Coin> { c1 };
        vm.CoinRacks[0].LoadCoins(coinLoad1);

        //COIN_LOAD([0] 1; 10, 1)
        Coin c2 = new Coin(10);
        List<Coin> coinLoad2 = new List<Coin> { c2 };
        vm.CoinRacks[1].LoadCoins(coinLoad2);

        //COIN_LOAD([0] 2; 25, 2)
        Coin c3 = new Coin(25);
        List<Coin> coinLoad3 = new List<Coin> { c3, c3 };
        vm.CoinRacks[2].LoadCoins(coinLoad3);

        //COIN_LOAD([0] 3; 100, 0)
        Coin c4 = new Coin(100);
        List<Coin> coinLoad4 = new List<Coin> { };
        vm.CoinRacks[3].LoadCoins(coinLoad4);

        //POP_LOAD([0] 0; "Coke", 1)
        PopCan p1 = new PopCan("Coke");
        List<PopCan> popLoad1 = new List<PopCan> { p1 };
        vm.PopCanRacks[0].LoadPops(popLoad1);

        //POP_LOAD([0] 1; "water", 1)
        PopCan p2 = new PopCan("water");
        List<PopCan> popLoad2 = new List<PopCan> { p2 };
        vm.PopCanRacks[1].LoadPops(popLoad2);

        //POP_LOAD([0] 2; "stuff", 1)
        PopCan p3 = new PopCan("stuff");
        List<PopCan> popLoad3 = new List<PopCan> { p3 };
        vm.PopCanRacks[2].LoadPops(popLoad3);

        //UNLOAD([0])
        var vmsc = new VendingMachineStoredContents();

        foreach (var coinRack in vm.CoinRacks)
        {
            vmsc.CoinsInCoinRacks.Add(coinRack.Unload());
        }
        vmsc.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
        foreach (var popCanRack in vm.PopCanRacks)
        {
            vmsc.PopCansInPopCanRacks.Add(popCanRack.Unload());
        }

        //CHECK_TEARDOWN(0; 0) // This passes, but we should not get this far
        var coinsInCoinRacks = vmsc.CoinsInCoinRacks;
        var coinsUsedForPayment = vmsc.PaymentCoinsInStorageBin;
        var unsoldPopCanRacks = vmsc.PopCansInPopCanRacks;
        var totalChangeRemaining = 0;
        var totalCoinsUsed = 0;

        foreach (var rack in coinsInCoinRacks)
        {
            foreach (var coin in rack)
            {
                totalChangeRemaining -= ((Coin)coin).Value;
            }
        }
        foreach (var coin in coinsUsedForPayment)
        {
            totalCoinsUsed -= ((Coin)coin).Value;
        }

        Assert.AreEqual(totalChangeRemaining, 0);
        Assert.AreEqual(totalCoinsUsed, 0);
    }

    /*BadTestScript3() mimicks U03-bad-names-list test script*/
    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void BadTestScript3()
    {
        //CREATE(5, 10, 25, 100; 3; 10; 10; 10)
        int[] coinKinds = { 5, 10, 25, 100 };
        int numPops = 3;
        int capacity = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //CONFIGURE([0] "Coke", 250; "water", 250)
        List<string> popNames = new List<string> { "Coke", "water"};
        List<int> popCosts = new List<int> { 250, 250 };
        vm.Configure(popNames, popCosts);
    }

    /*BadTestScript4() mimicks U04-bad-non-unique-denomination test script*/
    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void BadTestScript4()
    {
        //CREATE(1, 1; 1; 10; 10; 10)
        int[] coinKinds = { 1, 1 };
        int numPops = 1;
        int capacity = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml = new VendingMachineLogic(vm);
    }

    /*BadTestScript5() mimicks U05-bad-coin-kind test script*/
    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void BadTestScript5()
    {
        //CREATE(0; 1; 10; 10; 10)
        int[] coinKinds = { 0 };
        int numPops = 1;
        int capacity = 10;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, capacity, capacity, capacity);
        VendingMachineLogic vml = new VendingMachineLogic(vm);
    }

    /*BadTestScript6() mimicks U06-bad-button-number test script*/
    [TestMethod]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void BadTestScript6()
    {
        //CREATE(5, 10, 25, 100; 3)
        int[] coinKinds = { 5, 10, 25, 100 };
        int numPops = 3;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, 1, 1, 1);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //PRESS([0] 3)
        vm.SelectionButtons[3].Press();

    }

    /*BadTestScript7() mimicks U07-bad-button-number-2 test script*/
    [TestMethod]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void BadTestScript7()
    {
        //CREATE(5, 10, 25, 100; 3)
        int[] coinKinds = { 5, 10, 25, 100 };
        int numPops = 3;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, 1, 1, 1);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //PRESS([0] -1)
        vm.SelectionButtons[-1].Press();

    }

    /*BadTestScript8() mimicks U08-bad-button-number-3 test script*/
    [TestMethod]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void BadTestScript8()
    {
        //CREATE(5, 10, 25, 100; 3)
        int[] coinKinds = { 5, 10, 25, 100 };
        int numPops = 3;
        VendingMachine vm = new VendingMachine(coinKinds, numPops, 1, 1, 1);
        VendingMachineLogic vml = new VendingMachineLogic(vm);

        //PRESS([0] 4)
        vm.SelectionButtons[4].Press();
    }
}