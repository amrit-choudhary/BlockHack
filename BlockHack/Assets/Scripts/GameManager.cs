using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private const string url = "http://127.0.0.1:3000/";

    public GameObject loginScreen;
    public GameObject mainMenuScreen;
    public GameObject quizScreen;

    public InputField username_field;
    public InputField password_field;
    public InputField matchID_field;

    public Text option1_text;
    public Text option2_text;
    public Text option3_text;
    public Text option4_text;
    public Text result_text;
    private Text option_Text;

    public Image flag_image;

    public Text matchID_text;

    private string saved_username;
    private bool created_game = false;
    private MatchData saved_matchData;
    private int save_correct_answer_index = 0;

    private string username;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Login() {
        username = username_field.text;
        string password = password_field.text;

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        StartCoroutine("TryLogin", form);
    }

    IEnumerator TryLogin(WWWForm form_) {
        WWW download = new WWW(url + "user/login", form_);
        yield return download;

        if (!string.IsNullOrEmpty(download.error)) {
            print("Error downloading: " + download.error);
        } else {
            if (!(download.text == "Wrong pass" || download.text == "Cant find user")) {
                saved_username = username;
                loginScreen.SetActive(false);
                mainMenuScreen.SetActive(true);
            }
        }
    }

    public void Register() {
        username = username_field.text;
        string password = password_field.text;

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        StartCoroutine("TryRegister", form);
    }

    IEnumerator TryRegister(WWWForm form_) {
        WWW download = new WWW(url + "user/register", form_);
        yield return download;

        if (!string.IsNullOrEmpty(download.error)) {
            print("Error downloading: " + download.error);
        } else {
            print(download.text);
        }
    }

    public void BackToLoginScreen() {
        mainMenuScreen.SetActive(false);
        loginScreen.SetActive(true);
    }

    public void CreateGame() {
        StartCoroutine("TryCreateGame");
    }

    IEnumerator TryCreateGame() {
        WWWForm form = new WWWForm();
        form.AddField("username", saved_username);

        WWW download = new WWW(url + "match/new", form);
        yield return download;

        if (!string.IsNullOrEmpty(download.error)) {
            print("Error downloading: " + download.error);
        } else {
            if (!(download.text == "Match already exits" || download.text == "Cant create new match")) {
                saved_matchData = JsonUtility.FromJson<MatchData>(download.text);
                matchID_text.text = "Match ID: " + saved_matchData.match_id;
                created_game = true;
            }
        }
    }

    public void JoinGame() {
        StartCoroutine("TryJoinGame");
    }

    IEnumerator TryJoinGame() {
        WWWForm form = new WWWForm();
        form.AddField("username", saved_username);
        form.AddField("match_id", matchID_field.text);

        WWW download = new WWW(url + "match/join", form);
        yield return download;

        if (!string.IsNullOrEmpty(download.error)) {
            print("Error downloading: " + download.error);
        } else {
            if (!(download.text == "Cant join" || download.text == "Cant find match")) {
                saved_matchData = JsonUtility.FromJson<MatchData>(download.text);
                matchID_text.text = "Joined ID: " + saved_matchData.match_id;
            }
        }
    }

    public void StartGame() {
        StartCoroutine("TryStartGame");
    }

    IEnumerator TryStartGame() {
        WWWForm form = new WWWForm();
        form.AddField("match_id", saved_matchData.match_id);

        WWW download;

        if (created_game) {
            download = new WWW(url + "match/new_question", form);
        } else {
            download = new WWW(url + "match/get_question", form);
        }

        yield return download;

        if (!string.IsNullOrEmpty(download.error)) {
            print("Error downloading: " + download.error);
        } else {
            if (!(download.text == "Cant join" || download.text == "Cant find match")) {
                saved_matchData = JsonUtility.FromJson<MatchData>(download.text);
                PopulateQuiz();
                mainMenuScreen.SetActive(false);
                quizScreen.SetActive(true);
            }
        }
    }

    private void PopulateQuiz() {
        option1_text.text = saved_matchData.name1;
        option2_text.text = saved_matchData.name2;
        option3_text.text = saved_matchData.name3;
        option4_text.text = saved_matchData.name4;

        if (saved_matchData.code == saved_matchData.code1)
            save_correct_answer_index = 1;

        if (saved_matchData.code == saved_matchData.code2)
            save_correct_answer_index = 2;

        if (saved_matchData.code == saved_matchData.code3)
            save_correct_answer_index = 3;

        if (saved_matchData.code == saved_matchData.code4)
            save_correct_answer_index = 4;

        string path = "FlagImages/" + saved_matchData.code.ToLower();
        var flag_sprite = Resources.Load<Sprite>(path);
        flag_image.sprite = flag_sprite;
    }

    public void Next() {
        StartGame();
        result_text.text = "";
    }

    public void GiveAnswer(int i) {
        if (i == save_correct_answer_index) {
            result_text.text = "Correct!!";
        } else {
            result_text.text = "Incorrect!!";
        }
    }
}
