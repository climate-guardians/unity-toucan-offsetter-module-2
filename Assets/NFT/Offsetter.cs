using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class Offsetter : Variables
{
    // set chain: ethereum, moonbeam, polygon etc
    public string chain = "polygon";
    // set network mainnet, testnet
    public string network = "mumbai";
    // ChainId
    // String chainId = 80001;
    // smart contract method to call
    public string rpc = "https://rpc-mumbai.maticvigil.com";

    public string[] methods = {"approve", "autoOffsetUsingToken", "calculateNeededTokenAmount"};

    string[] addresses = {"0x30dC279166DCFB69F52C91d6A3380dCa75D0fCa7", "0x7beCBA11618Ca63Ead5605DE235f6dD3b25c530E", "0xDd052AcA9AC1492a8b4F1432B68f11989903dE4d", "0xe11A86849d99F524cAC3E7A0Ec1241828e332C62", "0xA6FA4fB5f76172d178d61B04b0ecd319C5d1C0aa', '0x9c3C9283D3e44854697Cd22D3Faa240Cfb032889"};
    
    // String offsetHelper = addresses[0];
    // String nct = addresses[1];
    // String swapToken = addresses[2];

    // String usdc = addresses[3];
    // String weth = addresses[4];
    // String wmatic = addresses[5];
    long amount = 1000000000000000000;

    // array of arguments for contract

    public void dummyFunc(string d)
    {
        autoOffsetUsingToken(chain, network, methods, addresses, amount);
    }

    public void DummFunc2()
    {
        Test();
    }

    public async void Test()
    {
        string chain = "polygon";
        string network = "mumbai";
        string transaction = "0x2c74a12426b98a70c4473a3a95304423785cab7c2aa2cfdc7c2fa1c68492e15f";
        string rpc = "https://rpc-mumbai.maticvigil.com/";

        string txStatus = await EVM.TxStatus(chain, network, transaction, rpc);
        print(txStatus); // success, fail, pending
    }

    public async void SendTKey()
    {
        // private key of account
        //string privateKey = "0x78dae1a22c7507a4ed30c06172e7614eb168d3546c13856340771e63ad3c0081";
        string privateKey = "0x13d8df86635ae868e980cd8565b4b325353c168b27159d75e522d5a974338635";
        // set chain: ethereum, moonbeam, polygon etc
        string chain = "polygon";// "ethereum";
        // set network mainnet, testnet
        string network = "mumbai";// "rinkeby";
        // account of player        
        string account = Web3PrivateKey.Address(privateKey);
        Debug.Log(account);
        // account to send to
        string to = "0x428066dd8A212104Bc9240dCe3cdeA3D3A0f7979";
        // value in wei
        string value = "0";// "123";
        // optional rpc url
        string rpc = this.rpc;

        string chainId = await EVM.ChainId(chain, network, rpc);
        Debug.Log(chainId);
        string gasPrice = await EVM.GasPrice(chain, network, rpc);
        Debug.Log(gasPrice);
        string data = "";
        string gasLimit = "";// "21000";

        string[] argsCalculateNeededTokenAmount = { $"{this.addresses[4]}", $"{this.addresses[1]}", $"{this.amount}" }; // WETH to NCT amount

        string argsCalculateNeededTokenAmountSerialized = JsonConvert.SerializeObject(argsCalculateNeededTokenAmount);

        string dataCalculateNeededTokenAmount = await EVM.CreateContractData(getAbiOffset(), this.methods[2], argsCalculateNeededTokenAmountSerialized);
        Debug.Log("dataCalculateNeededTokenAmount:\n" + dataCalculateNeededTokenAmount);


        string transaction = await EVM.CreateTransaction(chain, network, account, to, value, dataCalculateNeededTokenAmount /*data*/, gasPrice, gasLimit, rpc);
        print(transaction);
        string signature = Web3PrivateKey.SignTransaction(privateKey, transaction, chainId);
        print(signature);
        string response = await EVM.BroadcastTransaction(chain, network, account, to, value, dataCalculateNeededTokenAmount /*data*/, signature, gasPrice, gasLimit, rpc);

        print(response);
    }

    public async void autoOffsetUsingToken(string chain, string network, string[] methods, string[] addresses, long amount ) {


        // Replace call with private key
        // string response = await Web3Wallet.SendTransaction(chainId, to, value, data, gasLimit, gasPrice);

        // Replace call with web3wallet
        string chainId = "80001";// await EVM.ChainId(chain, network, rpc);
        // string gasPrice = await EVM.GasPrice(chain, network, rpc);
        string gasPrice = "";
        string data = "";
        string value = "0";
        string gasLimit = "";
        
        string[] argsCalculateNeededTokenAmount = { $"{addresses[4]}", $"{addresses[1]}", $"{amount}" }; // WETH to NCT amount

        string argsCalculateNeededTokenAmountSerialized = JsonConvert.SerializeObject(argsCalculateNeededTokenAmount);

        string dataCalculateNeededTokenAmount = await EVM.CreateContractData( getAbiOffset(), methods[2], argsCalculateNeededTokenAmountSerialized);
        Debug.Log("dataCalculateNeededTokenAmount:\n" + dataCalculateNeededTokenAmount);
        
        // calculateNeededTokenAmount
        string responseCalculateNeededTokenAmount = await Web3Wallet.SendTransaction(chainId, addresses[0], value, dataCalculateNeededTokenAmount, gasLimit, gasPrice);
        
        
        // TODO: Check what the response is and prepare to fit in the args
        Debug.Log(responseCalculateNeededTokenAmount);
        return;

        string[] argsApprovePoolContract = { $"{addresses[0]}", $"{responseCalculateNeededTokenAmount}" }; // allow offsetHelper to spend responseCalculateNeededTokenAmount
        string argsApprovePoolContractSerialized = JsonConvert.SerializeObject(argsApprovePoolContract);
        
        
        string dataApprovePoolContract = await EVM.CreateContractData(getAbiNCT(), methods[0], argsApprovePoolContractSerialized);
        Debug.Log("dataApprovePoolContract:\n" + dataApprovePoolContract);
        
        // approve Pool Contract
        string responseApprove = await Web3Wallet.SendTransaction(chainId, addresses[1], value, dataApprovePoolContract, gasLimit, gasPrice);
        Debug.Log(responseApprove);
        
        string[] argAutoOffsetUsingToken = { $"{addresses[4]}", $"{addresses[1]}", $"{amount}" }; 
        string argsAutoOffsetUsingTokenSerialized = JsonConvert.SerializeObject(argAutoOffsetUsingToken);

        string dataAutoOffsetUsingToken = await EVM.CreateContractData(getAbiOffset(), methods[1], argsAutoOffsetUsingTokenSerialized);
        Debug.Log("dataAutoOffsetUsingToken:\n" + dataAutoOffsetUsingToken);

        // autoOffsetUsingToken
        string responseAutoOffsetUsingToken = await Web3Wallet.SendTransaction(chainId, addresses[1], value, dataAutoOffsetUsingToken, gasLimit, gasPrice);
        Debug.Log(responseAutoOffsetUsingToken);
    }
}
