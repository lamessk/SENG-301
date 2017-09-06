using System.Collections;
using System.Collections.Generic;

using Frontend1;

namespace seng301_asgn1
{
    /// <summary>
    /// Represents the concrete virtual vending machine factory that you will implement.
    /// This implements the IVendingMachineFactory interface, and so all the functions
    /// are already stubbed out for you.
    /// 
    /// Your task will be to replace the TODO statements with actual code.
    /// 
    /// Pay particular attention to extractFromDeliveryChute and unloadVendingMachine:
    /// 
    /// 1. These are different: extractFromDeliveryChute means that you take out the stuff
    /// that has already been dispensed by the machine (e.g. pops, money) -- sometimes
    /// nothing will be dispensed yet; unloadVendingMachine is when you (virtually) open
    /// the thing up, and extract all of the stuff -- the money we've made, the money that's
    /// left over, and the unsold pops.
    /// 
    /// 2. Their return signatures are very particular. You need to adhere to this return
    /// signature to enable good integration with the other piece of code (remember:
    /// this was written by your boss). Right now, they return "empty" things, which is
    /// something you will ultimately need to modify.
    /// 
    /// 3. Each of these return signatures returns typed collections. For a quick primer
    /// on typed collections: https://www.youtube.com/watch?v=WtpoaacjLtI -- if it does not
    /// make sense, you can look up "Generic Collection" tutorials for C#.
    /// </summary>
    public class VendingMachineFactory : IVendingMachineFactory
    {
        public int vmindex = -1;
        List<VendingMachine> vmFactory;

        public VendingMachineFactory()
        {
            vmFactory = new List<VendingMachine>();
        }

        public int createVendingMachine(List<int> coinKinds, int selectionButtonCount)
        {
            //Increase vending machine index for newest vending machine
            vmindex++;

            //Create a new vending machine
            VendingMachine vm = new VendingMachine(coinKinds, selectionButtonCount);

            //Add the new vending machine to the vending machine factory list
            vmFactory.Add(vm);

            //Return its index
            return vmindex;
        }

        public void configureVendingMachine(int vmIndex, List<string> popNames, List<int> popCosts)
        {
            //Access vending machine at the specified index
            VendingMachine vm = vmFactory[vmIndex];
            
            //Check if any of the pop costs are zero or negiative, if so, throw an exception
            foreach (int pc in popCosts)
            {
                if (pc <= 0)
                    throw new System.ArgumentException("Pop costs cannot be negative or zero");
            }

            //Check if  number of popNames and popCosts are different, if so, throw an exception
            int numPopNames = popNames.Count;
            int numPopCosts = popCosts.Count;
            if(numPopNames != numPopCosts)
                throw new System.ArgumentException("Amount of pops given does not equal amount of pop costs given");
 
            //Create a dictionary that will associate a pop with a cost
            //Key is Pop , Value is cost 
            vm.configure = new Dictionary<string, int>();
            int indexCount = 0;
            while(indexCount < numPopNames)
            {
                vm.configure.Add(popNames[indexCount], popCosts[indexCount]);
                indexCount++;
            }
            vm.costindexmap = new List<int>();
            foreach(KeyValuePair<string, int> kvp in vm.configure)
            {
                int popCost = kvp.Value;
                vm.costindexmap.Add(popCost);
            }
        }

        public void loadCoins(int vmIndex, int coinKindIndex, List<Coin> coins)
        {
            VendingMachine vm = vmFactory[vmindex];

            Queue<Coin> cki = vm.listCChutes[coinKindIndex];

            foreach (Coin c in coins)
            {
                cki.Enqueue(c);
            }
        }

        public void loadPops(int vmIndex, int popKindIndex, List<Pop> pops)
        {
            VendingMachine vm = vmFactory[vmIndex];
            Queue<Pop> pki = vm.pChutes[popKindIndex];
            foreach(Pop p in pops)
            {
                pki.Enqueue(p);
            }
        }

        public void insertCoin(int vmIndex, Coin coin)
        {
            VendingMachine vm = vmFactory[vmIndex];
            int coinValue = coin.Value;

            if(vm.dictCChutes.ContainsKey(coinValue) == false)
            {
                System.Console.WriteLine("Invalid coin. Dispensed to delivery chute");
                vm.deliveryChute.Add(coin);
            }
            else
            {
                vm.total = vm.total + coinValue;
                vm.coinsInserted.Add(coin);
            }
            
        }

        public void pressButton(int vmIndex, int value)
        {
            VendingMachine vm = vmFactory[vmIndex];

            int numPops = vm.configure.Count - 1;
            if (value > numPops || value < 0)
            {
                throw new System.ArgumentException("Button requested out of range");
            }
            else
            {  
                if((vm.total >= vm.costindexmap[value]) && (vm.pChutes[value].Count > 0))
                {
                    Queue<Pop> popChuteVal = vm.pChutes[value];
                    Pop dispense = popChuteVal.Dequeue();
                    vm.deliveryChute.Add(dispense);
                }
                if(vm.total >= vm.costindexmap[value])
                {
                    List<Coin> changeCreated = createChange(vmIndex, vm.total, vm.costindexmap[value]);
                    foreach(Coin a in changeCreated)
                    {
                        vm.deliveryChute.Add(a);
                    }
                }
            }
        }

        public List<Coin> createChange(int vmIndex, int total, int cost)
        {
            VendingMachine vm = vmFactory[vmIndex];
            int change = total - cost;
            List<int> coinList = new List<int>();
            foreach(KeyValuePair<int, Queue<Coin>> kv in vm.dictCChutes)
            {
                coinList.Add(kv.Key);
            }
            coinList.Sort();
            coinList.Reverse();
            List<Coin> changeList = new List<Coin>();
            foreach(int i in coinList)
            {
                Queue<Coin> currentChute = vm.dictCChutes[i]; 
                while((change >= i) && (currentChute.Count > 0))
                {
                    Coin c = currentChute.Dequeue();
                    changeList.Add(c);
                    change = change - i;
                }
            }
            return changeList;
        }

        public List<Deliverable> extractFromDeliveryChute(int vmIndex)
        {
            VendingMachine vm = vmFactory[vmIndex];
            List<Deliverable> extracted = new List<Deliverable>();

            foreach (Deliverable i in vm.deliveryChute)
            {
                extracted.Add(i);
            }
            vm.deliveryChute.Clear();
            return extracted;
        }

        public List<IList> unloadVendingMachine(int vmIndex)
        {
            VendingMachine vm = vmFactory[vmIndex];
            List<Coin> remainingChange = new List<Coin>();
            foreach (Queue<Coin> q in vm.listCChutes)
            {
                foreach (Coin c in q)
                {
                    remainingChange.Add(c);
                }
                q.Clear();
            }

            List<Coin> moneyMade = new List<Coin>();
            foreach(Coin i in vm.coinsInserted)
            {
                moneyMade.Add(i);
            }
            vm.coinsInserted.Clear();

            List<Pop> remainingPop = new List<Pop>();
            foreach (Queue<Pop> qp in vm.pChutes)
            {
                foreach (Pop p in qp)
                {
                    remainingPop.Add(p);
                }
                qp.Clear();
            }
            vm.total = 0;

            List<IList> unloadedVM = new List<IList>();
            unloadedVM.Add(remainingChange);
            unloadedVM.Add(moneyMade);
            unloadedVM.Add(remainingPop);

            return unloadedVM;
        }
    }

    internal class VendingMachine
    {
        public List<Queue<Pop>> pChutes;
        public Dictionary<int, Queue<Coin>> dictCChutes;
        public List<Queue<Coin>> listCChutes;
        public Dictionary<string, int> configure;
        public int total = 0;
        public List<Coin> coinsInserted = new List<Coin>();
        public ArrayList deliveryChute = new ArrayList();
        public List<int> costindexmap;

        public VendingMachine(List<int> coinKinds, int pops)
        {
            //Number of pops to be accepted by vending machine
            int numPops = pops;

            //Use a list to keep track of all of the pop chutes
            pChutes = new List<Queue<Pop>>();

            //While we still have less chutes than pops
            //Create a new pop chute
            int counter = 0;
            while(counter < numPops)
            {
                Queue<Pop> newPChute = createPopChute();
                pChutes.Add(newPChute);
                counter++;
            }

            //Create a list for all the coins we will be accepting
            List<int> coins = new List<int>();

            //For every coin in the given list
            foreach (int coin in coinKinds)
            {
                if (coins.Contains(coin) || coin == 0 || coin < 0)
                    throw new System.ArgumentException("Chute for" + coin + "already exists");
                else
                    coins.Add(coin);
            }

            dictCChutes = createCoinChuteDict(coins);
            listCChutes = coinChuteList(dictCChutes);
        }

        public Dictionary<int, Queue<Coin>> createCoinChuteDict(List<int> coins)
        {
            Dictionary<int, Queue<Coin>> coinChuteDict = new Dictionary<int, Queue<Coin>>();
            foreach(int i in coins)
            {
                Queue<Coin> newCoinQ = new Queue<Coin>();
                coinChuteDict.Add(i, newCoinQ);
            }

            return coinChuteDict;
        }

        public List<Queue<Coin>> coinChuteList(Dictionary<int, Queue<Coin>> ccDict)
        {
            List<Queue<Coin>> ccList = new List<Queue<Coin>>();
            foreach(KeyValuePair<int, Queue<Coin>> k in ccDict)
            {
                Queue<Coin> chute = k.Value;
                ccList.Add(chute);
            }
            return ccList;
        }

        public Queue<Pop> createPopChute()
        {
            Queue<Pop> popChute = new Queue<Pop>();

            return popChute;
        }

    }
}