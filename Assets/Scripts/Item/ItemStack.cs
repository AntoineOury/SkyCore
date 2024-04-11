using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a stack of items.
/// </summary>
public class ItemStack
{
    public ItemIdentity identity;
    public int amount;

    private static Stack<ItemStack> _pool = new();
    private static int _stacksCount; // The size of the pool plus the number of stacks which are in use.

    public bool IsStackFull => amount >= identity.MaxStack;


    static ItemStack()
    {
        AddNewObjectsToPool(100);
    }

    // Don't use the constructor. Use ProduceObject instead, for object pooling.
    private ItemStack()
    {
        amount = -1;
        identity = null;
    }

    /// <summary>
    /// Creates a new stack of items, potentially using a previously constructed object in a pool.
    /// The point of doing this rather than directly using the constructor is to avoid garbage allocation.
    /// </summary>
    public static ItemStack ProduceObject(ItemIdentity identity, int amount)
    {
        if (identity == null)
        {
            throw new System.ArgumentNullException("itemInfo");
        }
        if (amount > identity.MaxStack)
        {
            throw new System.ArgumentException("ItemStack constructor's" +
                $" amount ({amount}) > itemInfo.MaxStack ({identity.MaxStack}) (can't fit that many items in a single stack.)");
        }

        if (_pool.Count == 0)
        {
            AddNewObjectsToPool(_stacksCount / 2);
            Debug.LogWarning($"Expanded ItemStack's pool to {_stacksCount}. This might indicate some code isn't calling" +
                $" ItemStack.ReturnToPool() when it's done using an ItemStack.");
        }

        ItemStack result = _pool.Pop();
        result.identity = identity;
        result.amount = amount;
        return result;
    }

    private static void AddNewObjectsToPool(int countToAdd)
    {
        for (int i = 0; i < countToAdd; i++)
        {
            _pool.Push(new ItemStack());
        }
        _stacksCount += countToAdd;
    }

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CheckAllItemStacksWereReturned()
    {
        if (_pool.Count != _stacksCount)
        {
            throw new System.Exception($"Not all item stacks were returned to the pool before a scene loaded (and presumably" +
                $" the other scene unloaded). All code should call ItemStack.ReturnToPool when it stops using the item stack," +
                $" including when the scene unloads.");
        }
    }
#endif

    /// <summary>
    /// Returns this item stack to the pool. Call this method when done using this item stack.
    /// </summary>
    public void ReturnToPool()
    {
        if (identity == null)
        {
            throw new System.Exception("identity == null, indicating this item stack was already returned to the pool.");
        }
        amount = -1;
        identity = null;
        _pool.Push(this);
    }

    public ItemStack GiveAsManyAsCanAsNewStack()
    {
        // If this stack isn't overfull (so amount <= identity.MaxStack), this just makes an identical stack and sets its own amount to 0.
        int amountToTake = System.Math.Min(amount, identity.MaxStack);
        amount -= amountToTake;
        return ProduceObject(identity, amountToTake);
    }

    public void StealAsManyAsCan(ItemStack other)
    {
        int amountBeforeFull = identity.MaxStack - amount;
        int amountToSteal = System.Math.Min(amountBeforeFull, other.amount);
        amount += amountToSteal;
        other.amount -= amountToSteal;
    }

    public override string ToString()
    {
        return "Name: " + identity.name + " Amount: " + amount;
    }
}
