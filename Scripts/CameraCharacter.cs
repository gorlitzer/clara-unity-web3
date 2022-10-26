using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// use web3.jslib
using System.Runtime.InteropServices;

public class CameraCharacter : MonoBehaviour
{

    string chain = "ethereum";
    public string network = "goerli";
    string contract = "0x5cBC2B8b4f1c871ef0e3d3DD9D7a0FFf652FE9C9"; // leaderboard SC goerli address
    string privateKeyStr = "0xdc4aa42e60f00000b000000042cf412a00000000000080467b43"; //0x46Cb2E9d3cdb92ca0ff8021303F879Cd1e3d7a46

    public float speed = 1;
    public float incrementFactor = 0.02f;
    public float spawnHelper = 4.5f;
    public float ballForce = 700;
    
    public GameObject ball;
    public GameObject gameOverFull;
    public GameObject gameOverRestart;
    private CharacterController cameraChar;
    private Camera _cam;
    public GameObject loading;
    public GameObject toaster;

    private Vector3 spawnCameraPos;

    public static bool camMoving = false;    
    private bool collision = false; //A boolean whose value will be determined by OnTriggerEnter
    public static int ballCount = 7; // count balls for points
    public static int score = 0;
    string isTx1Confirmed = "pending";
    string isTx2Confirmed = "pending";

    public Text ballText; // UI to show ball count
    public Text currScore; // UI to show current score
    public Text TextAddress; // reference to text address

    // use functions from web3.jslib
    [DllImport("__Internal")]
    private static extern string WalletAddress();
    [DllImport ("__Internal")]
    private static extern string SendTransaction(string to, string data);
    [DllImport ("__Internal")]
    private static extern string SignGameRecipe(string privKey, string player, int score, string nonce);
    
    // Use this for initialization
    void Start()
    {
        ballCount = 7;
        score = 0;
        cameraChar = gameObject.GetComponent<CharacterController>();
        _cam = GetComponent<Camera>();
        spawnCameraPos = _cam.transform.position; // get initial camera position
    }

    private void Awake() {
         // UI: toaster and loader management initialization
        loading = GameObject.Find("LoadingPanel");
        toaster = GameObject.Find("ToasterPanel");
        
        loading.SetActive(false); 
        toaster.SetActive(false); // control set on start
    }

    // Update is called once per frame
    void Update()
    {
        ballText.text = CameraCharacter.ballCount.ToString(); // make this text obj always display “ballCount”
        currScore.text = CameraCharacter.score.ToString(); // make this text obj always display “ballCount”

        float mousePosx = Input.mousePosition.x;
        float mousePosy = Input.mousePosition.y;

        Vector3 BallInstantiatePoint =
            _cam
                .ScreenToWorldPoint(new Vector3(mousePosx,
                    mousePosy,
                    _cam.nearClipPlane + spawnHelper));

        //This checks if we have collided
        if (!collision && camMoving)
        {
            cameraChar.Move(Vector3.forward * Time.deltaTime * speed);

            //This is so that the camera's movement will speed up
            speed += incrementFactor;
            ballForce += incrementFactor * 2;
        }
        else if (collision || !camMoving)
        {
            cameraChar.Move(Vector3.zero);
        } 
        
        if (transform.position.y != spawnCameraPos.y) {
            Vector3 NewPos = new Vector3(spawnCameraPos.x, spawnCameraPos.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, NewPos, 5f);
        }

        // THIS WAY WORKS ON WEBGL WITH ONE SPAWNING BALL AT MOUSE CLICK
        /**
        FOR MOBILE CHECKS FOR TOUCH COUNT INPUT (MULTIPLE SPAWNING)
        if (Input.touchCount > 0 && camMoving)
                {
                    if (ballCount != 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            float mousePosx = Input.GetTouch(i).position.x;
                            float mousePosy = Input.GetTouch(i).position.y;
                            if (Input.GetTouch(i).phase == TouchPhase.Began)
                            {
                                GameObject ballRigid;
                                Vector3 BallInstantiatePoint = _cam.ScreenToWorldPoint(new Vector3(mousePosx, mousePosy, _cam.nearClipPlane + spawnHelper));
                                ballRigid = Instantiate(ball, BallInstantiatePoint, transform.rotation) as GameObject;
                                ballRigid.GetComponent<Rigidbody>().AddForce(Vector3.forward * ballForce);
                                ballCount--;
                            }
                        }
                    }
        */
        if (Input.GetMouseButtonDown(0) && camMoving)
        {
            if (ballCount > 0)
            {
                GameObject ballRigid;
                ballRigid = Instantiate(ball, BallInstantiatePoint, transform.rotation) as GameObject;
                ballRigid.GetComponent<Rigidbody>().AddForce(Vector3.forward * ballForce);
                ballCount--;
            }
            else
            {
                ballText.text = "We're out of energy!";
                camMoving = false;
                if( score == 0) {
                    gameOverRestart.SetActive(true);
                } else  {
                    gameOverFull.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collision detected");
        if (other.CompareTag("good") || other.CompareTag("bad"))
        {
            collision = true;
            //Debug.Log("Collided!! Man down!!");
            if( score == 0) {
                    gameOverRestart.SetActive(true);
            } else  {
                    gameOverFull.SetActive(true);
            }
            camMoving = false;
        }
    }

    public void StartCam()
    {
        string value = "0.01"; // 0,01 ethereum - value converted in wei inside .jslib
        loading.SetActive(true);  
        SendTransaction(contract, value); // 2. send tx and wait for transaction hash
    }
 
    void TransactionCallback(string txHash) { 
        StartCoroutine(DoCheckReceiptFirst(txHash)); // Start coroutine to call function every tot seconds
    }

    void TransactionCallbackError() { 
        loading.SetActive(false); // stop loader if error or rejected transaction
        Reset();
    }

    public async void AddScoreLeaderboard() { 
        string walletText = WalletAddress(); 
        string nonce = await EVM.Nonce(chain, network, walletText);
        gameOverFull.SetActive(false);
        loading.SetActive(true);  
        SignGameRecipe(privateKeyStr, walletText, int.Parse(currScore.text), nonce);
    }

    // SERVIREBBE FARE CHECK DELLA RICEVUTA E CONTROLLARE EVENTO SU BLOCKCHAIN 
    void AddScoreLeaderboardCallback(string txHash) { 
        StartCoroutine(DoCheckReceiptSecond(txHash));
    }

    void AddScoreLeaderboardCallbackError(int code) { 
        Debug.Log(code); // code 4001: MetaMask Tx Signature: User denied transaction signature.
        loading.SetActive(false);
        Reset();
    }

        // Coroutine function
    IEnumerator DoCheckReceiptFirst(string hash)
    {   
        for (; ; )
        {
            CheckReceiptFirst(hash);
            if(isTx1Confirmed == "success") {
                loading.SetActive(false); // 4. stop loader
                camMoving = !camMoving;  // 5. start movement and game - called from .jslib [TWO WAY COMMUNICATION]
                yield break; // 6. stop coroutine

            } else if (isTx2Confirmed == "fail") {
                Reset(); // -> SHOULD MANAGE FAIL TOASTER
            }
            // https://docs.unity3d.com/ScriptReference/WaitForSeconds.html
            yield return new WaitForSeconds(3); // wait for 3 secs
        }
    }

    // Coroutine function
    IEnumerator DoCheckReceiptSecond(string hash)
    {   
        for (; ; )
        {
            CheckReceiptSecond(hash);
            if (isTx2Confirmed == "success") {
                loading.SetActive(false); // 4. stop loader
                toaster.SetActive(true);
                yield break; // 6. stop coroutine
            } else if (isTx2Confirmed == "fail") {
                loading.SetActive(false); // 4. stop loader
                toaster.SetActive(true); // -> SHOULD MANAGE FAIL TOASTER
                yield break; // 6. stop coroutine
            }

            // https://docs.unity3d.com/ScriptReference/WaitForSeconds.html
            yield return new WaitForSeconds(3); // wait for 3 secs
        }
    }
        
    async void CheckReceiptFirst(string hash)
    {   
        isTx1Confirmed = await EVM.TxStatus(chain, network, hash);
        isTx2Confirmed = "pending";
        print(System.DateTime.Now.ToString("HH:mm:ss - dd MMMM, yyyy - ") + "Transaction confirmed: " + isTx1Confirmed);
    }

    async void CheckReceiptSecond(string hash)
    {   
        isTx2Confirmed = await EVM.TxStatus(chain, network, hash);
        isTx1Confirmed = "pending";
        print(System.DateTime.Now.ToString("HH:mm:ss - dd MMMM, yyyy - ") + "Transaction confirmed: " + isTx2Confirmed);    
    }

    public void Reset()
    {
        SceneManager.LoadScene("Scene1");
    }
}