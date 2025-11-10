using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTime : LevelCondition
{
    private float m_time;

    private GameManager m_mngr;

    private BoardController m_board;
    private int m_score;
    private int m_targetScore;

    public override void Setup(float value, Text txt, BoardController board, GameManager mngr)
    {
        base.Setup(value, txt, board, mngr);

        m_mngr = mngr;

        m_time = value;

        m_score = 0;

        m_board = board;

        m_targetScore = m_board.GetBoardSize() / 3;

        m_board.OnScoreEvent += OnScore;

        UpdateText();
    }

    private void Update()
    {
        if (m_conditionCompleted) return;

        if (m_mngr.State != GameManager.eStateGame.GAME_STARTED) return;

        m_time -= Time.deltaTime;

        UpdateText();

        if (m_time <= -0.5f)
        {
            m_mngr.GameOver();
            m_conditionCompleted = true;
        }
    }

    protected override void UpdateText()
    {
        if (m_time < 0f) return;

        m_txt.text = string.Format("TIME:\n{0:00}", m_time);
    }

    private void OnScore()
    {
        if (m_conditionCompleted) return;

        m_score++;

        if (m_score >= m_targetScore)
        {
            OnConditionComplete();
        }
    }
}
