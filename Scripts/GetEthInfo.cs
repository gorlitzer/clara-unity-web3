// GetWalletAddress.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// use web3.jslib
using System.Runtime.InteropServices;

public class GetEthInfo : MonoBehaviour
{
    public Text TextAddress;
    public Text TextBlock;

    string walletText = "0x09400000000000000000000aaaaaaaaaa";
    string chain = "ethereum";
    public string network = "goerli";

    // use WalletAddress function from web3.jslib
    [DllImport("__Internal")] private static extern string WalletAddress();

    void Start()
    {
        // 1. get wallet address and display it on the input - WITH WEB3.LIBJS
        GetCurrentAccount();

        // 2. get current block and display on the input - WITH CHAINSAFE SDK
        StartCoroutine("DoCheckBlockNAccount"); // Start coroutine to call function every tot seconds
    }
    void Update()
    {
    }

    // Coroutine function
    IEnumerator DoCheckBlockNAccount()
    {
        for (; ; )
        {
            GetCurrentBlock();
            //GetCurrentAccount();
            // https://docs.unity3d.com/ScriptReference/WaitForSeconds.html
            yield return new WaitForSeconds(10); // wait for 1 min
        }
    }

    void GetCurrentAccount() {
        walletText = WalletAddress(); 
        walletText = walletText.Substring(0, 25);
        walletText = walletText += "...";
        if (TextAddress.text != walletText) {
            TextAddress.text = walletText; // eth address counts 40 characters (20 bytes)
        }
    }

    async void GetCurrentBlock()
    {
        // check for current block and update if different
        int blockNumber = await EVM.BlockNumber(chain, network);
        string blockText = "Block: " + blockNumber;
        //Debug.Log("Changing block number, latest block stored is: " + blockNumber);
        if (TextBlock.text != blockText) {
            TextBlock.text = "Block: " + blockNumber; // eth address counts 40 characters (20 bytes)
        }
       
    }
}

