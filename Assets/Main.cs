using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : WifiDirectBase {
    public GameObject canvas;
    public GameObject buttonList;
    public GameObject addrButton;
    public GameObject addrPanel;
    public GameObject cube;
    public GameObject colorPanel;
    public GameObject red;
    public GameObject green;
    public GameObject blue;
    public GameObject send;
	// Adds listeners to the color sliders and calls the initialize script on the library
	void Start () {
        colorPanel.SetActive(false);
        addrPanel.SetActive(false);
        cube.SetActive(false);
        canvas.SetActive(false);
        red.GetComponent<Slider>().onValueChanged.AddListener((blah) => {
            this.updateCube(this.getSliderColors());
        });
        green.GetComponent<Slider>().onValueChanged.AddListener((blah) => {
            this.updateCube(this.getSliderColors());
        });
        blue.GetComponent<Slider>().onValueChanged.AddListener((blah) => {
            this.updateCube(this.getSliderColors());
        });
        send.GetComponent<Button>().onClick.AddListener(() => {
            base.sendMessage("#" + ColorUtility.ToHtmlStringRGB(this.getSliderColors())); //when send button is clicked, send the new rgb color
        });
        base.initialize(this.gameObject.name);
    }
	//when the WifiDirect services is connected to the phone, begin broadcasting and discovering services
    public override void onServiceConnected() {
        canvas.SetActive(true);
        addrPanel.SetActive(true);
        Dictionary<string, string> record = new Dictionary<string, string>();
        record.Add("demo", "unity");
        base.broadcastService("hello", record);
        base.discoverServices();
    }
	//On finding a service, create a button with that service's address
    public override void onServiceFound(string addr) {
        GameObject newButton = Instantiate(addrButton);
        newButton.GetComponentInChildren<Text>().text = addr;
        newButton.transform.SetParent(buttonList.transform, false);
        newButton.GetComponent<Button>().onClick.AddListener(() => {
            this.makeConnection(addr);
        });
    }
	//When the button is clicked, connect to the service at its address
    private void makeConnection(string addr) {
        base.connectToService(addr);
    }
	//When connected, begin rendering the cube
    public override void onConnect() {
        addrPanel.SetActive(false);
        cube.SetActive(true);
        colorPanel.SetActive(true);
    }
	//Turns the slider values into a Color
    private Color getSliderColors () {
        return new Color(red.GetComponent<Slider>().value, green.GetComponent<Slider>().value, blue.GetComponent<Slider>().value);
    }
	//Updates the color of the cube
    private void updateCube(Color c) {
        cube.GetComponent<Renderer>().material.color = c;
    }
	//When recieving a new message, parse the color and set it to the cube
    public override void onMessage(string message) {
        Color c = new Color(0,0,0);
        ColorUtility.TryParseHtmlString(message, out c);
        this.updateCube(c);
    }
	//Kill Switch
    public override void onServiceDisconnected() {
        base.terminate();
        Application.Quit();
    }
	//Kill Switch
    public void OnApplicationPause(bool pause) {
        if(pause) {
            this.onServiceDisconnected();
        }
    }
}
