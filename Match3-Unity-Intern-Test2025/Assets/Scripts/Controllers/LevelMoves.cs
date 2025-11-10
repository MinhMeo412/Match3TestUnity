using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMoves : LevelCondition
{
    private int m_score;

    private int m_targetScore;

    private BoardController m_board;

    public override void Setup(float value, Text txt, BoardController board)
    {
        base.Setup(value, txt);

        m_targetScore = board.GetBoardSize() / 3;
        m_score = 0;

        m_board = board;

        m_board.OnScoreEvent += OnScore;

        UpdateText();
    }

    private void OnScore()
    {
        if (m_conditionCompleted) return;

        m_score++;

        UpdateText();

        if (m_score >= m_targetScore)
        {
            OnConditionComplete();
        }
    }

    protected override void UpdateText()
    {
        m_txt.text = string.Format("SCORE:\n{0}", m_score);
    }

    protected override void OnDestroy()
    {
        if (m_board != null) m_board.OnScoreEvent -= OnScore;

        base.OnDestroy();
    }
}
