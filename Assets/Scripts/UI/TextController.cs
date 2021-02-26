using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TextController : MonoBehaviour 
{
	public string[] scenarios;
	[SerializeField] TextMeshProUGUI uiText;

    [SerializeField] string nextSceneName;

	[SerializeField][Range(0.001f, 0.3f)]
	float intervalForCharacterDisplay = 0.05f;

	private string currentText = string.Empty;
	private float timeUntilDisplay = 0;
	private float timeElapsed = 1;
	private int currentLine = 0;
	private int lastUpdateCharacter = -1;

	// 文字の表示が完了しているかどうか
	public bool IsCompleteDisplayText 
	{
		get { return  Time.time > timeElapsed + timeUntilDisplay; }
	}

	void Start()
	{
        SoundManager.Instance.PlayBgm("opening");
		SetNextLine();
	}

	void Update () 
	{
		// 文字の表示が完了してるならクリック時に次の行を表示する
		if( IsCompleteDisplayText ){
			if(Input.GetMouseButtonDown(0)){
				SetNextLine();
                SoundManager.Instance.PlaySe("select");
			}
		}else{
		// 完了してないなら文字をすべて表示する
			if(Input.GetMouseButtonDown(0)){
				timeUntilDisplay = 0;
                SoundManager.Instance.PlaySe("select");
			}
		}

		int displayCharacterCount = (int)(Mathf.Clamp01((Time.time - timeElapsed) / timeUntilDisplay) * currentText.Length);
		if( displayCharacterCount != lastUpdateCharacter ){
			uiText.text = currentText.Substring(0, displayCharacterCount);
            SoundManager.Instance.PlaySe("move");
			lastUpdateCharacter = displayCharacterCount;
		}
	}


	void SetNextLine()
	{
        if (currentLine < scenarios.Length) {
            currentText = scenarios[currentLine];
            timeUntilDisplay = currentText.Length * intervalForCharacterDisplay;
            timeElapsed = Time.time;
            currentLine ++;
            lastUpdateCharacter = -1;
        }
        else {
            SceneManager.LoadScene(nextSceneName);
        }
	}
}