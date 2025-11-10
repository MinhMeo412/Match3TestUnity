## 📂 Scripts
    │
    ├── Board/
    │ ├── Board.cs  #Added variable `CellBottom`
    │ │             #Added function `CreateBottomCells`
    │ │             #Rewrote `Fill`
    │ │             #Added `MoveCellToBottomCell(Cell cell, Action callback)`
    │ │             #Added `MoveBottomCellToCell(CellBottom cellBottom, Action callback)`
    │ │             #Remove unused functions
    │ │ 
    │ ├── Cell.cs   #Added function `IsSameType(CellBottom other)`
    │ │ 
    │ ├── Item.cs   #Added variable `CellBottom`
    │ │             #Added function `SetCellBottom(CellBottom cellBottom)`
    │ │     
    │ └── Added CellBottom.cs
    │
    ├── Controllers/
    │ ├── GameManager.cs    #Added public property 'LevelMode'
    │ │                     #Adjusted function `LoadLevel(eLevelMode mode)`
    │ │                     #Added `GameWin`
    │ │                     #Adjusted function `WaitBoardController(bool isWin)
    │ │     
    │ ├── BoardController.cs    #Added function `SetIsBusy`
    │ │                         #Added function `GetBoard`
    │ │                         #Added function `GetBoardSize`
    │ │                         #Adjusted `Update`
    │ │                         #Added function `IsTimerMode`
    │ │                         #Added function `CheckBottomCellMatch(Board board)` 
    │ │                         #Added function `CheckLoseCondition(Board board)`
    │ │                         #Added function `ShiftBottomCells(Board board)`
    │ │                         #Remove unused functions
    │ │
    │ ├── LevelMoves.cs         #Added variable `m_score`
    │ │                         #Added variable `m_targetScore`
    │ │                         #Adjusted function `Setup`
    │ │                         #Added function `OnScore()`
    │ │
    │ ├── LevelTime.cs          #Added variable `m_score`
    │ │                         #Added variable `m_targetScore`
    │ │                         #Added variable `m_board`    
    │ │                         #Adjusted function `Setup`
    │ │                         #Added function `OnScore()`
    │ │      
    │ └── Added AutoPlay.cs   
    │             
    ├── UI/
    │ ├── UIMainManager.cs      #Adjusted function `OnGameStateChange`
    │ │                         #Added function `ShowGameMenuWithAuto()`
    │ │                         
    │ ├── UIPanelGame.cs        #Added variable `btnAutoWin`
    │ │                         #Added variable `btnAutoLose`
    │ │                         #Added function `SetAutoPlay(AutoPlay autoPlay)`
    │ │                         #Added function `SetAutoButtonsVisible(bool visible)`
    │ │                         #Added function `OnClickAutoWin()`
    │ │                         #Added function `OnClickAutoLose()`
    │ │
    │ └── UIPanelPause.cs       #Adjusted function `OnClickClose()`
    │ 
    ├── ...
    │


