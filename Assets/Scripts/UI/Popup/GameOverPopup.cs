using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
public class GameOverPopup : MonoBehaviour,IPopup
{
    [SerializeField]
    private TextMeshProUGUI _hightScore;
    [SerializeField]
    private TextMeshProUGUI _score;
    private Tweener windowTweener;
    public void Open(UnityAction i_onComplete)
    {
        transform.localScale = Vector3.zero;
        windowTweener?.Kill();
        windowTweener = this.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).OnComplete(()=>
        {
            _hightScore.text = "HIGHT SCORE : " + GameObject.FindObjectOfType<GameData>()?.GetHightScore().ToString("00000");
            _score.text = "YOUR SCORE : " + GameManager.Instance.GetScore().ToString("00000");
            i_onComplete?.Invoke();
        }).Play();
    }
    public void Close(UnityAction i_onComplete)
    {
        windowTweener?.Kill();
        windowTweener = this.transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                i_onComplete?.Invoke();
                
            })
            .Play();
    }

    public void OnPlayAgainClick()
    {
        Close(()=>
        {
            this.gameObject.SetActive(false);
            GameManager.Instance.ChangeGameState(GAMESTATE.SETUP);
        });
    }
    public void OnQuitClick()
    {
        Close(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }
}
