using Jellies;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class JelliesParametersTests
{
    private void CreateTestJelly(out GameObject testJelly, out Parameters parameters)
    {
        testJelly = new GameObject("TestJelly");
        parameters = testJelly.AddComponent<Parameters>();
    }
    
    [UnityTest]
    public IEnumerator TestIncreaseFoodSaturation()
    {
        CreateTestJelly(out GameObject testJelly, out Parameters parameters);
        float initValue = 50;
        float increaseValue = 10;
        parameters.SetSatiation(initValue);
        
        parameters.IncreaseSatiation(increaseValue);
        
        Assert.AreEqual(initValue + increaseValue, parameters.Satiation);
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator TestDecreaseFoodSaturation()
    { 
        CreateTestJelly(out GameObject testJelly, out Parameters parameters);
        float initValue = 50;
        float increaseValue = 10;
        parameters.SetSatiation(initValue);
        
        parameters.DecreaseSatiation(increaseValue);
        
        Assert.AreEqual(initValue - increaseValue, parameters.Satiation);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestIfFoodDoesNotExceedMaxSaturation()
    {
        CreateTestJelly(out GameObject testJelly, out Parameters parameters);
        float maxValue = parameters.MaxSatiation;
        parameters.SetSatiation(maxValue);
        parameters.IncreaseSatiation(1f);

        Assert.LessOrEqual(parameters.Satiation, maxValue);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestIfJellyGetsHungrierAfter10Seconds()
    {
        CreateTestJelly(out GameObject testJelly, out Parameters parameters);
        float initValue = parameters.MaxSatiation;
        float timeToWait = 10f;
        parameters.SetSatiation(initValue);
        
        yield return new WaitForSeconds(timeToWait);
        Assert.LessOrEqual(parameters.Satiation, parameters.MaxSatiation);
    }
}