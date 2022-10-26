// web3.jslib
// This is the bridge between JS and C#

mergeInto(LibraryManager.library, {
    WalletAddress: function () {
        var returnStr
        try {
            // get address from metamask
            returnStr = web3.currentProvider.selectedAddress
        } catch (e) {
            returnStr = "0x09400000000000000000000aaaaaaaaaa"
        }
        if (returnStr !== null) {
            var bufferSize = lengthBytesUTF8(returnStr) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(returnStr, buffer, bufferSize);
            return buffer;
        }
    },
    SendTransaction: function (to, data) {
        var tostr = Pointer_stringify(to);
        var wei = parseFloat(Pointer_stringify(data)) * Math.pow(10, 18);
        var from = "";
        ethereum.request({ method: 'eth_accounts' })
            .then(function (response) {
                from = response[0];
                var args = {
                    "from": from,
                    "to": tostr,
                    "value": wei.toString(16), //convert to hex
                    //"data": datastr
                };

                ethereum.request({ method: 'eth_sendTransaction', params: [args], })
                    .then(function (txHash) {
                        unityInstance.SendMessage("Camera", "TransactionCallback", txHash);
                    })
                    .catch(function (error) {
                        unityInstance.SendMessage("Camera", "TransactionCallbackError");
                    });
            })
    },
    SignGameRecipe: function (privKey, player, score, nonce) {

        var LeaderboardABI = [{ "inputs": [{ "internalType": "address", "name": "_game_address", "type": "address" }, { "internalType": "address", "name": "_game_wallet", "type": "address" }], "stateMutability": "nonpayable", "type": "constructor" }, { "anonymous": !1, "inputs": [{ "indexed": !1, "internalType": "address", "name": "signer", "type": "address" }, { "indexed": !1, "internalType": "address", "name": "receiver", "type": "address" }, { "indexed": !1, "internalType": "uint256", "name": "score", "type": "uint256" }], "name": "Approval", "type": "event" }, { "anonymous": !1, "inputs": [{ "indexed": !1, "internalType": "address", "name": "sender", "type": "address" }, { "indexed": !1, "internalType": "uint256", "name": "value", "type": "uint256" }, { "indexed": !1, "internalType": "uint256", "name": "time", "type": "uint256" }], "name": "StartLog", "type": "event" }, { "anonymous": !1, "inputs": [{ "components": [{ "internalType": "address", "name": "user", "type": "address" }, { "internalType": "uint256", "name": "score", "type": "uint256" }], "indexed": !1, "internalType": "struct Leaderboard.User", "name": "firstWinner", "type": "tuple" }, { "components": [{ "internalType": "address", "name": "user", "type": "address" }, { "internalType": "uint256", "name": "score", "type": "uint256" }], "indexed": !1, "internalType": "struct Leaderboard.User", "name": "secondWinner", "type": "tuple" }, { "components": [{ "internalType": "address", "name": "user", "type": "address" }, { "internalType": "uint256", "name": "score", "type": "uint256" }], "indexed": !1, "internalType": "struct Leaderboard.User", "name": "thirdWinner", "type": "tuple" }, { "indexed": !1, "internalType": "uint256", "name": "time", "type": "uint256" }], "name": "Withdrawal", "type": "event" }, { "stateMutability": "payable", "type": "fallback", "payable": !0 }, { "inputs": [], "name": "game_wallet", "outputs": [{ "internalType": "address", "name": "", "type": "address" }], "stateMutability": "view", "type": "function", "constant": !0 }, { "inputs": [], "name": "isOwner", "outputs": [{ "internalType": "bool", "name": "", "type": "bool" }], "stateMutability": "view", "type": "function", "constant": !0 }, { "inputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "name": "leaderboard", "outputs": [{ "internalType": "address", "name": "user", "type": "address" }, { "internalType": "uint256", "name": "score", "type": "uint256" }], "stateMutability": "view", "type": "function", "constant": !0 }, { "inputs": [], "name": "owner", "outputs": [{ "internalType": "address", "name": "", "type": "address" }], "stateMutability": "view", "type": "function", "constant": !0 }, { "stateMutability": "payable", "type": "receive", "payable": !0 }, { "inputs": [{ "internalType": "address", "name": "user", "type": "address" }, { "internalType": "uint256", "name": "score", "type": "uint256" }, { "internalType": "uint256", "name": "nonce", "type": "uint256" }, { "internalType": "bytes", "name": "signature", "type": "bytes" }], "name": "addScore", "outputs": [{ "internalType": "bool", "name": "boolean", "type": "bool" }], "stateMutability": "payable", "type": "function", "payable": !0 }, { "inputs": [], "name": "withdraw", "outputs": [{ "internalType": "bool", "name": "success", "type": "bool" }], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [], "name": "getBalance", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "stateMutability": "view", "type": "function", "constant": !0 }, { "inputs": [], "name": "getSigner", "outputs": [{ "internalType": "address", "name": "", "type": "address" }], "stateMutability": "view", "type": "function", "constant": !0 }, { "inputs": [], "name": "_calculatePercentage", "outputs": [{ "internalType": "uint256", "name": "thirty", "type": "uint256" }, { "internalType": "uint256", "name": "twenty", "type": "uint256" }, { "internalType": "uint256", "name": "tenth", "type": "uint256" }], "stateMutability": "view", "type": "function", "constant": !0 }]
        var contractAddress = '0x5cBC2B8b4f1c871ef0e3d3DD9D7a0FFf652FE9C9'

        var web3 = new Web3(Web3.givenProvider)
        var leaderboardContract = new web3.eth.Contract(LeaderboardABI, contractAddress)

        var playerStr = Pointer_stringify(player)
        var nonceStr = Pointer_stringify(nonce)
        var privateKeyStr = Pointer_stringify(privKey)

        var signedMsg = "";

        try {
            var message = Web3.utils.soliditySha3(playerStr, score, nonceStr);
            var fullMessage = web3.eth.accounts.sign(message, privateKeyStr); // sign message with game pk
            signedMsg = fullMessage.signature;
        } catch (error) {
            console.log("Error : ", error)
        }
        if (signedMsg !== "") {
            var from = "";
            ethereum.request({ method: 'eth_accounts' })
                .then(function (response) {
                    from = response[0];
                    leaderboardContract.methods
                        .addScore(from, score, nonceStr, signedMsg)
                        .send({ from: from, value: Web3.utils.toWei("0.02", "ether") }, function (err, res) {
                            if (err) {
                                console.log("An error occured sending to AddScore! ", err)
                                unityInstance.SendMessage("Camera", "AddScoreLeaderboardCallbackError", err.code);
                                return;
                            }
                            unityInstance.SendMessage("Camera", "AddScoreLeaderboardCallback", res);
                        })
                })
        } else {
            unityInstance.SendMessage("Camera", "AddScoreLeaderboardCallbackError", "Signature failed!");
        }
    }
});