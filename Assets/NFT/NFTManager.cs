using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
//using Siccity.GLTFUtility;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public delegate void OnNFTMethodCallback(string message   );

public class NFTManager : MonoBehaviour
{
    public string chain = "alfajores-forno";
    public string network = "testnet";
    public string rpc = "https://alfajores-forno.celo-testnet.org";

    public string contractMint;
    public string valueMint = "100000000000000";
    public string contractIdMint = "1";

    public string contractOpen;
    public string valueOpen = "0";
    public string contractIdOpen = "0";

    public string chainId = "44787";
    public string abi = "";

    public bool overrideEVM = false;
    public string gasLimit = "1000000";
    public string gasPrice = "100000";

    private string account;
    public static NFTManager instance;
    public  GameObject unitPack;
    public bool isPackOpened = false;
    public void Awake()
    {
        account = PlayerPrefs.GetString("Account");
        SceneManager.sceneLoaded += OnSceneLoaded;
        if(instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = gameObject.GetComponent<NFTManager>();
        }
        else
        {
            
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        unitPack = GameObject.Find("UnitPack");
        UpdateUnitPack();
    }

    public void UpdateUnitPack()
    {
        if(unitPack)
        {
            for (int i = 0; i < unitPack.transform.childCount; i++)
            {
                unitPack.transform.GetChild(i).gameObject.SetActive(isPackOpened);
            }
        }
    }

    public string GetABI()
    {
        return @"
                [
                    {
                ""inputs"": [
                    {
                    ""internalType"": ""string"",
                    ""name"": ""baseTokenURIC"",
                    ""type"": ""string""
                    },
                    {
                    ""internalType"": ""address"",
                    ""name"": ""_fungiContrAddress"",
                    ""type"": ""address""
                    },
                    {
                    ""internalType"": ""address"",
                    ""name"": ""_insectContrAddress"",
                    ""type"": ""address""
                    },
                    {
                    ""internalType"": ""address"",
                    ""name"": ""_plantsContrAddress"",
                    ""type"": ""address""
                    }
                ],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""constructor""
                },
                {
                ""anonymous"": false,
                ""inputs"": [
                    {
                    ""indexed"": true,
                    ""internalType"": ""address"",
                    ""name"": ""account"",
                    ""type"": ""address""
                    },
                    {
                    ""indexed"": true,
                    ""internalType"": ""address"",
                    ""name"": ""operator"",
                    ""type"": ""address""
                    },
                    {
                    ""indexed"": false,
                    ""internalType"": ""bool"",
                    ""name"": ""approved"",
                    ""type"": ""bool""
                    }
                ],
                ""name"": ""ApprovalForAll"",
                ""type"": ""event""
                },
                {
                ""anonymous"": false,
                ""inputs"": [
                    {
                    ""indexed"": true,
                    ""internalType"": ""address"",
                    ""name"": ""previousOwner"",
                    ""type"": ""address""
                    },
                    {
                    ""indexed"": true,
                    ""internalType"": ""address"",
                    ""name"": ""newOwner"",
                    ""type"": ""address""
                    }
                ],
                ""name"": ""OwnershipTransferred"",
                ""type"": ""event""
                },
                {
                ""anonymous"": false,
                ""inputs"": [
                    {
                    ""indexed"": true,
                    ""internalType"": ""address"",
                    ""name"": ""operator"",
                    ""type"": ""address""
                    },
                    {
                    ""indexed"": true,
                    ""internalType"": ""address"",
                    ""name"": ""from"",
                    ""type"": ""address""
                    },
                    {
                    ""indexed"": true,
                    ""internalType"": ""address"",
                    ""name"": ""to"",
                    ""type"": ""address""
                    },
                    {
                    ""indexed"": false,
                    ""internalType"": ""uint256[]"",
                    ""name"": ""ids"",
                    ""type"": ""uint256[]""
                    },
                    {
                    ""indexed"": false,
                    ""internalType"": ""uint256[]"",
                    ""name"": ""values"",
                    ""type"": ""uint256[]""
                    }
                ],
                ""name"": ""TransferBatch"",
                ""type"": ""event""
                },
                {
                ""anonymous"": false,
                ""inputs"": [
                    {
                    ""indexed"": true,
                    ""internalType"": ""address"",
                    ""name"": ""operator"",
                    ""type"": ""address""
                    },
                    {
                    ""indexed"": true,
                    ""internalType"": ""address"",
                    ""name"": ""from"",
                    ""type"": ""address""
                    },
                    {
                    ""indexed"": true,
                    ""internalType"": ""address"",
                    ""name"": ""to"",
                    ""type"": ""address""
                    },
                    {
                    ""indexed"": false,
                    ""internalType"": ""uint256"",
                    ""name"": ""id"",
                    ""type"": ""uint256""
                    },
                    {
                    ""indexed"": false,
                    ""internalType"": ""uint256"",
                    ""name"": ""value"",
                    ""type"": ""uint256""
                    }
                ],
                ""name"": ""TransferSingle"",
                ""type"": ""event""
                },
                {
                ""anonymous"": false,
                ""inputs"": [
                    {
                    ""indexed"": false,
                    ""internalType"": ""string"",
                    ""name"": ""value"",
                    ""type"": ""string""
                    },
                    {
                    ""indexed"": true,
                    ""internalType"": ""uint256"",
                    ""name"": ""id"",
                    ""type"": ""uint256""
                    }
                ],
                ""name"": ""URI"",
                ""type"": ""event""
                },
                {
                ""inputs"": [
                    {
                    ""internalType"": ""address"",
                    ""name"": ""account"",
                    ""type"": ""address""
                    },
                    {
                    ""internalType"": ""uint256"",
                    ""name"": ""id"",
                    ""type"": ""uint256""
                    }
                ],
                ""name"": ""balanceOf"",
                ""outputs"": [
                    {
                    ""internalType"": ""uint256"",
                    ""name"": """",
                    ""type"": ""uint256""
                    }
                ],
                ""stateMutability"": ""view"",
                ""type"": ""function""
                },
                {
                ""inputs"": [
                    {
                    ""internalType"": ""address[]"",
                    ""name"": ""accounts"",
                    ""type"": ""address[]""
                    },
                    {
                    ""internalType"": ""uint256[]"",
                    ""name"": ""ids"",
                    ""type"": ""uint256[]""
                    }
                ],
                ""name"": ""balanceOfBatch"",
                ""outputs"": [
                    {
                    ""internalType"": ""uint256[]"",
                    ""name"": """",
                    ""type"": ""uint256[]""
                    }
                ],
                ""stateMutability"": ""view"",
                ""type"": ""function""
                },
                {
                ""inputs"": [],
                ""name"": ""baseTokenURI"",
                ""outputs"": [
                    {
                    ""internalType"": ""string"",
                    ""name"": """",
                    ""type"": ""string""
                    }
                ],
                ""stateMutability"": ""view"",
                ""type"": ""function""
                },
                {
                ""inputs"": [
                    {
                    ""internalType"": ""address"",
                    ""name"": ""account"",
                    ""type"": ""address""
                    },
                    {
                    ""internalType"": ""uint256"",
                    ""name"": ""id"",
                    ""type"": ""uint256""
                    },
                    {
                    ""internalType"": ""uint256"",
                    ""name"": ""value"",
                    ""type"": ""uint256""
                    }
                ],
                ""name"": ""burn"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
                },
                {
                ""inputs"": [
                    {
                    ""internalType"": ""address"",
                    ""name"": ""account"",
                    ""type"": ""address""
                    },
                    {
                    ""internalType"": ""uint256[]"",
                    ""name"": ""ids"",
                    ""type"": ""uint256[]""
                    },
                    {
                    ""internalType"": ""uint256[]"",
                    ""name"": ""values"",
                    ""type"": ""uint256[]""
                    }
                ],
                ""name"": ""burnBatch"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
                },
                {
                ""inputs"": [
                    {
                    ""internalType"": ""uint256"",
                    ""name"": ""id"",
                    ""type"": ""uint256""
                    }
                ],
                ""name"": ""exists"",
                ""outputs"": [
                    {
                    ""internalType"": ""bool"",
                    ""name"": """",
                    ""type"": ""bool""
                    }
                ],
                ""stateMutability"": ""view"",
                ""type"": ""function""
                },
                {
                ""inputs"": [],
                ""name"": ""fungiContrAddress"",
                ""outputs"": [
                    {
                    ""internalType"": ""address"",
                    ""name"": """",
                    ""type"": ""address""
                    }
                ],
                ""stateMutability"": ""view"",
                ""type"": ""function""
                },
                {
                ""inputs"": [],
                ""name"": ""insectContrAddress"",
                ""outputs"": [
                    {
                    ""internalType"": ""address"",
                    ""name"": """",
                    ""type"": ""address""
                    }
                ],
                ""stateMutability"": ""view"",
                ""type"": ""function""
                },
                {
                ""inputs"": [
                    {
                    ""internalType"": ""address"",
                    ""name"": ""account"",
                    ""type"": ""address""
                    },
                    {
                    ""internalType"": ""address"",
                    ""name"": ""operator"",
                    ""type"": ""address""
                    }
                ],
                ""name"": ""isApprovedForAll"",
                ""outputs"": [
                    {
                    ""internalType"": ""bool"",
                    ""name"": """",
                    ""type"": ""bool""
                    }
                ],
                ""stateMutability"": ""view"",
                ""type"": ""function""
                },
                {
                ""inputs"": [
                    {
                    ""internalType"": ""uint256"",
                    ""name"": ""amount"",
                    ""type"": ""uint256""
                    },
                    {
                    ""internalType"": ""bytes"",
                    ""name"": ""data"",
                    ""type"": ""bytes""
                    }
                ],
                ""name"": ""mint"",
                ""outputs"": [],
                ""stateMutability"": ""payable"",
                ""type"": ""function""
                },
                {
                ""inputs"": [
                    {
                    ""internalType"": ""uint256"",
                    ""name"": ""id"",
                    ""type"": ""uint256""
                    },
                    {
                    ""internalType"": ""uint256"",
                    ""name"": ""amount"",
                    ""type"": ""uint256""
                    }
                ],
                ""name"": ""openPack"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
                },
                {
                ""inputs"": [],
                ""name"": ""owner"",
                ""outputs"": [
                    {
                    ""internalType"": ""address"",
                    ""name"": """",
                    ""type"": ""address""
                    }
                ],
                ""stateMutability"": ""view"",
                ""type"": ""function""
                },
                {
                ""inputs"": [],
                ""name"": ""plantsContrAddress"",
                ""outputs"": [
                    {
                    ""internalType"": ""address"",
                    ""name"": """",
                    ""type"": ""address""
                    }
                ],
                ""stateMutability"": ""view"",
                ""type"": ""function""
                },
                {
                ""inputs"": [],
                ""name"": ""price"",
                ""outputs"": [
                    {
                    ""internalType"": ""uint256"",
                    ""name"": """",
                    ""type"": ""uint256""
                    }
                ],
                ""stateMutability"": ""view"",
                ""type"": ""function""
                },
                {
                ""inputs"": [],
                ""name"": ""renounceOwnership"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
                },
                {
                ""inputs"": [
                    {
                    ""internalType"": ""address"",
                    ""name"": ""from"",
                    ""type"": ""address""
                    },
                    {
                    ""internalType"": ""address"",
                    ""name"": ""to"",
                    ""type"": ""address""
                    },
                    {
                    ""internalType"": ""uint256[]"",
                    ""name"": ""ids"",
                    ""type"": ""uint256[]""
                    },
                    {
                    ""internalType"": ""uint256[]"",
                    ""name"": ""amounts"",
                    ""type"": ""uint256[]""
                    },
                    {
                    ""internalType"": ""bytes"",
                    ""name"": ""data"",
                    ""type"": ""bytes""
                    }
                ],
                ""name"": ""safeBatchTransferFrom"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
                },
                {
                ""inputs"": [
                    {
                    ""internalType"": ""address"",
                    ""name"": ""from"",
                    ""type"": ""address""
                    },
                    {
                    ""internalType"": ""address"",
                    ""name"": ""to"",
                    ""type"": ""address""
                    },
                    {
                    ""internalType"": ""uint256"",
                    ""name"": ""id"",
                    ""type"": ""uint256""
                    },
                    {
                    ""internalType"": ""uint256"",
                    ""name"": ""amount"",
                    ""type"": ""uint256""
                    },
                    {
                    ""internalType"": ""bytes"",
                    ""name"": ""data"",
                    ""type"": ""bytes""
                    }
                ],
                ""name"": ""safeTransferFrom"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
                },
                {
                ""inputs"": [
                    {
                    ""internalType"": ""address"",
                    ""name"": ""operator"",
                    ""type"": ""address""
                    },
                    {
                    ""internalType"": ""bool"",
                    ""name"": ""approved"",
                    ""type"": ""bool""
                    }
                ],
                ""name"": ""setApprovalForAll"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
                },
                {
                ""inputs"": [
                    {
                    ""internalType"": ""bytes4"",
                    ""name"": ""interfaceId"",
                    ""type"": ""bytes4""
                    }
                ],
                ""name"": ""supportsInterface"",
                ""outputs"": [
                    {
                    ""internalType"": ""bool"",
                    ""name"": """",
                    ""type"": ""bool""
                    }
                ],
                ""stateMutability"": ""view"",
                ""type"": ""function""
                },
                {
                ""inputs"": [
                    {
                    ""internalType"": ""uint256"",
                    ""name"": ""id"",
                    ""type"": ""uint256""
                    }
                ],
                ""name"": ""totalSupply"",
                ""outputs"": [
                    {
                    ""internalType"": ""uint256"",
                    ""name"": """",
                    ""type"": ""uint256""
                    }
                ],
                ""stateMutability"": ""view"",
                ""type"": ""function""
                },
                {
                ""inputs"": [
                    {
                    ""internalType"": ""address"",
                    ""name"": ""newOwner"",
                    ""type"": ""address""
                    }
                ],
                ""name"": ""transferOwnership"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
                },
                {
                ""inputs"": [
                    {
                    ""internalType"": ""uint256"",
                    ""name"": ""_tokenId"",
                    ""type"": ""uint256""
                    }
                ],
                ""name"": ""uri"",
                ""outputs"": [
                    {
                    ""internalType"": ""string"",
                    ""name"": """",
                    ""type"": ""string""
                    }
                ],
                ""stateMutability"": ""view"",
                ""type"": ""function""
                }
            ]";
    }    
    public async void MintNFT(OnNFTMethodCallback callback)
    {
        string method = "mint";
        string[] obj = {contractIdMint, "0x0000"};
        string args = JsonConvert.SerializeObject(obj);
        string data = await EVM.CreateContractData(GetABI(), method, args);

        if(!overrideEVM)
        {
            gasLimit = await EVM.GasLimit(chain, network, "", valueMint, data, rpc);
        }

        gasPrice = await EVM.GasPrice(chain, network, rpc);
        callback("");
        string response = await Web3Wallet.SendTransaction(chainId, contractMint, valueMint, data, gasLimit, gasPrice);
        
    }

    public async void OpenNFTPack(OnNFTMethodCallback callback)
    {
        string method = "openPack";
        string[] obj = {contractIdOpen, "1" };
        string args = JsonConvert.SerializeObject(obj);
        string data = await EVM.CreateContractData(GetABI(), method, args);
        Debug.Log(data);
        if(!overrideEVM)
        {
            gasLimit = await EVM.GasLimit(chain, network, "", valueOpen, data, rpc);
        }

        gasPrice = await EVM.GasPrice(chain, network, rpc);
        isPackOpened = true;
        UpdateUnitPack();
        string response = await Web3Wallet.SendTransaction(chainId, contractOpen, valueOpen, data, gasLimit, gasPrice);
        
        callback(response);
    }
}