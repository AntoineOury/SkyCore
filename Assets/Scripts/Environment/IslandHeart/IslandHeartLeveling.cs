using Codice.Client.Common.GameUI;
using Codice.CM.Common.Tree.Partial;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Android;

public class IslandHeartLeveling : MonoBehaviour
{
    /// <summary>
    /// This event must be subscribed to by all scripts that need to trigger when the
    /// Island Heart this script is attached to levels up
    /// </summary>
    public event EventHandler islandHeartLevelUp;

	[Tooltip("The world space where the generator is placed when attached to Island Heart")]
	[SerializeField]
	private GameObject _generatorLocation;
	private GameObject _generator;
	public GameObject Generator => _generator;

	[Header("Island Heart Level Manager")]
	[Tooltip("Current Island Heart Level")]
	[Min(1f)]
	[SerializeField]
	private float _heartLevel = 1;

    [Tooltip("Maximum Island Heart Level")]
    [Min(2f)]
    [SerializeField]
    private float _maxHeartLevel = 2;

	[Tooltip("Multiplier to increase the experience threshold to level up the Island Heart")]
	[Min(1)]
	[SerializeField]
	private float _xpMultiplier = 2;

	[Tooltip("The experience threshold to level up the Island Heart")]
	[field: SerializeField]
	public float _xpThreshold
	{
		get;
		private set;
	} = 5;

	[Tooltip("The current amount of experience the Island Heart has")]
    [field: SerializeField]
    public float _currentXP
    {
        get;
        private set;
    } = 0;


    // Start is called before the first frame update
    void Start()
	{
		_generator = null;
	}

	/// <summary>
	/// Keeping this to pick up tossed items for now, eventually want to replace most likely
	/// </summary>
	/// <param name="other"></param>
	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<GeneratorHandler>())
		{
			if(_generator == null)
			{
				PlaceGenerator(other.gameObject, false);
			}
		}
		else if(other.gameObject.GetComponent<PickupItem>())
		{
			if(other.gameObject.GetComponent<PickupItem>().ItemInfo is JellyDewItemIdentity && !IsMaxLevel()) {
                //TODO: If dropping a large stack of Jelly Dew, we should make a way for that stack to be registered
                //      as a stack of items with a count that can be put in place of the 1 currently used here. This
                //		would also help prevent future performance problems due to too many items spawned for the
                //		game to keep track of.
                FeedIslandHeart(1);
				Destroy(other.gameObject);
            }
		}
	}
	

	/// <summary>
	/// This method is called by the entity providing the experience. 
	/// It also carries over surplus experience to the next level. 
	/// </summary>
	/// <param name="xpValue"></param>
	public void FeedIslandHeart(float xpValue)
	{
        if (_currentXP + xpValue >= _xpThreshold)
		{
			float difference = _currentXP - _xpThreshold;
			difference += xpValue;
			_currentXP = difference;
            /// This should only be a temporary method of setting the charge of the generator. We will need
            /// to know what the generator does specifically on an Island Heart level up.
            LevelUp();
			if (IsMaxLevel()) {
				_currentXP = _xpThreshold;
            }
            if (_generator != null)
            {
                _generator.GetComponent<GeneratorHandler>().SetCharge(_currentXP * 100);
            }
        } else
		{
			_currentXP += xpValue;
            if (_generator != null)
            {
                _generator.GetComponent<GeneratorHandler>().AddCharge((xpValue / _xpThreshold) * 100);
            }
        }
    }

	public bool PlaceGenerator(GameObject generator, bool instantiate)
	{
		if (_generator == null)
		{
		    generator.transform.position = _generatorLocation.transform.position;
			generator.transform.rotation = gameObject.transform.rotation;
			_generator = generator;
			_generator.GetComponent<GeneratorHandler>().SetCharge((_currentXP / _xpThreshold) * 100);
			_generator.GetComponent<Rigidbody>().isKinematic = true;
			return true;
		}
		return false;
	}

	public bool IsMaxLevel()
	{
		return _heartLevel == _maxHeartLevel;
	}

	/// <summary>
	/// This event increments the heart's level and invokes an event. This event can be subscribed to trigger
	/// subsequent level up event sequences such as island expansion and increased production.
	/// </summary>
	private void LevelUp()
	{
		_heartLevel++;
		_xpThreshold *= _xpMultiplier;
		islandHeartLevelUp?.Invoke(this, null);
		//TODO: Add any other sequences or animations/model changes an island heart goes through upon level up
	}
}
