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
	// Use this for initialization
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
            base.sendMessage("#" + ColorUtility.ToHtmlStringRGB(this.getSliderColors()));
        });
        base.initialize(this.gameObject.name);
    }
    public override void onServiceConnected() {
        canvas.SetActive(true);
        addrPanel.SetActive(true);
        Dictionary<string, string> record = new Dictionary<string, string>();
        record.Add("demo", "unity");
        base.broadcastService("hello", record);
        base.discoverServices();
    }
    public override void onServiceFound(string addr) {
        GameObject newButton = Instantiate(addrButton);
        newButton.GetComponentInChildren<Text>().text = addr;
        newButton.transform.SetParent(buttonList.transform, false);
        newButton.GetComponent<Button>().onClick.AddListener(() => {
            this.makeConnection(addr);
        });
    }
    private void makeConnection(string addr) {
        base.connectToService(addr);
    }
    public override void onConnect() {
        addrPanel.SetActive(false);
        cube.SetActive(true);
        colorPanel.SetActive(true);
    }
    private Color getSliderColors () {
        return new Color(red.GetComponent<Slider>().value, green.GetComponent<Slider>().value, blue.GetComponent<Slider>().value);
    }
    private void updateCube(Color c) {
        cube.GetComponent<Renderer>().material.color = c;
    }
    public override void onMessage(string message) {
        Color c = new Color(0,0,0);
        ColorUtility.TryParseHtmlString(message, out c);
        this.updateCube(c);
    }
    public override void onServiceDisconnected() {
        base.terminate();
        Application.Quit();
    }
    public void OnApplicationPause(bool pause) {
        if(pause) {
            this.onServiceDisconnected();
        }
    }
}
