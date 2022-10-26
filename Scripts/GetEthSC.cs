using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Newtonsoft.Json;
using SimpleJSON;


[System.Serializable]
public class PlayerInfo
{
    public string address = "0x00000000000000000000000...";
    public string score ="000";
}
    
public class GetEthSC : MonoBehaviour
{
    public Text FirstClassified;
    public Text SecondClassified;
    public Text ThirdClassified;

    PlayerInfo firstPlayerObj = new PlayerInfo();
    PlayerInfo secondPlayerObj = new PlayerInfo();
    PlayerInfo thirdPlayerObj = new PlayerInfo();


    string chain = "ethereum";
    public string network = "goerli";
    string contract = "0x5cBC2B8b4f1c871ef0e3d3DD9D7a0FFf652FE9C9"; // leaderboard SC goerli address
   // SC abi in JSON format (String)
    string abi = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_game_address\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_game_wallet\",\"type\":\"address\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"address\",\"name\":\"signer\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"receiver\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"time\",\"type\":\"uint256\"}],\"name\":\"StartLog\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"components\":[{\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"}],\"indexed\":false,\"internalType\":\"structLeaderboard.User\",\"name\":\"firstWinner\",\"type\":\"tuple\"},{\"components\":[{\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"}],\"indexed\":false,\"internalType\":\"structLeaderboard.User\",\"name\":\"secondWinner\",\"type\":\"tuple\"},{\"components\":[{\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"}],\"indexed\":false,\"internalType\":\"structLeaderboard.User\",\"name\":\"thirdWinner\",\"type\":\"tuple\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"time\",\"type\":\"uint256\"}],\"name\":\"Withdrawal\",\"type\":\"event\"},{\"stateMutability\":\"payable\",\"type\":\"fallback\"},{\"inputs\":[],\"name\":\"_calculatePercentage\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"thirty\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"twenty\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"tenth\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"nonce\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"signature\",\"type\":\"bytes\"}],\"name\":\"addScore\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"boolean\",\"type\":\"bool\"}],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"game_wallet\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getBalance\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getSigner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"leaderboard\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"withdraw\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"success\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"stateMutability\":\"payable\",\"type\":\"receive\"}]";


    void Start()
    {
        // Start coroutine to call function every tot seconds
        StartCoroutine("DoCheckLeaderboard");
    }
    void Update()
    {
    }

    // Coroutine function - checks for leaderboard mapping updates
    IEnumerator DoCheckLeaderboard()
    {
        for (; ; )
        {
            CheckLeaderboard1();
            CheckLeaderboard2();
            CheckLeaderboard3();
            // https://docs.unity3d.com/ScriptReference/WaitForSeconds.html
            yield return new WaitForSeconds(10); // wait for 3 min
        }
    }

    // function helper: foreach cycle
    public void DoForeach(string currentPlayer, PlayerInfo objName)
    {
        var obj = JSON.Parse(currentPlayer);
        foreach(var kvp in obj)
        {   
            //Debug.Log("Dict = " + kvp.Key + " : " + kvp.Value.Value);
            if(kvp.Key == "user") {
                string walletText = "0x00000000000000000000000..."; // initial value
                walletText = kvp.Value.Value.Substring(0, 25);
                walletText = walletText += "...";
                objName.address = walletText;      
            } 
            if(kvp.Key == "score") {
                objName.score = kvp.Value.Value;      
            }
        }
    }

    async void CheckLeaderboard1()
    {

    // smart contract method to call
    string method = "leaderboard";
    string first = await EVM.Call(chain, network, contract, abi, method, "[0]");

    DoForeach(first, firstPlayerObj); // cycle iteration to create game objects
    FirstClassified.text = firstPlayerObj.score + " - " + firstPlayerObj.address;
    }
    
    async void CheckLeaderboard2()
    {

    // smart contract method to call
    string method = "leaderboard";
    string second = await EVM.Call(chain, network, contract, abi, method, "[1]");

    DoForeach(second, secondPlayerObj); // cycle iteration to create game objects
    SecondClassified.text = secondPlayerObj.score + " - " + secondPlayerObj.address;
    }

    async void CheckLeaderboard3()
    {

    // smart contract method to call
    string method = "leaderboard";
    string third = await EVM.Call(chain, network, contract, abi, method, "[2]");

    DoForeach(third, thirdPlayerObj); // cycle iteration to create game objects
    ThirdClassified.text = thirdPlayerObj.score + " - " + thirdPlayerObj.address;
    }
}
