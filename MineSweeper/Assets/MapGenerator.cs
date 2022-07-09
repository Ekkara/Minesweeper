using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] [Range(9, 100)] int width = 18, height = 14;
    int flagesPlaced = 0;
    [SerializeField] int amountOfMines = 10;
    [SerializeField] GameObject tile;
    [SerializeField] Color color1, color2, BGColor1, BGColor2, hintColor1, hintColor2, flagColor1, flagColor2;
    [SerializeField] GameObject flag;
    [SerializeField] GameObject numberOfColidingMines;
    [SerializeField] GameObject mineGO;
    [SerializeField] Color mine1, mine2, mine3, mine4, mine5, mine6, mine7, mine8;
    [SerializeField] Tile[,] mineField;
    [SerializeField] bool mapMade = false;
    bool gameOver;

    [SerializeField] Text gameText;
    string baseGameText;
    private void Start() {
        baseGameText = gameText.text;
        GenerateTiles();
    }

    private void OnValidate() {
        if (amountOfMines >= ((width * height) - 9)) {
            int newAmount = ((width * height)) - 9;
            Debug.LogWarning("mines can't cover the entire field, value changed to entire field minus nine, the new value is: " + newAmount);
            amountOfMines = newAmount;
        }
        if (numberOfColidingMines.GetComponent<TextMesh>() == null) {
            Debug.LogWarning("objectDidn't contain text");
            numberOfColidingMines.AddComponent<TextMesh>();
        }
    }
    public void RemoveTiles() {
        if (mapMade) {
            mapMade = false;
            //destroy tiles
            for (int w = 0; w < mineField.GetLength(0); w++) {
                for (int h = 0; h < mineField.GetLength(1); h++) {
                    DestroyImmediate(mineField[w, h].tileGO);
                }
            }
        }
    }
    public void GenerateTiles() {
        firstPress = true;
        minSafeFound = (width * height) - amountOfMines;
        currentFound = 0;
        if (mapMade) {
            //destroy tiles
            RemoveTiles();
        }

        mineField = new Tile[width, height];
        for (int w = 0; w < width; w++) {
            for (int h = 0; h < height; h++) {
                GameObject newTile = Instantiate(tile);
                newTile.transform.position = transform.position + new Vector3(w, h);
                newTile.transform.SetParent(transform);

                newTile.GetComponent<SpriteRenderer>().color = (((w + h) % 2) == 0) ? color1 : color2;

                mineField[w, h].tileGO = newTile;
            }
        }
        mapMade = true;
        haveHint = false;
        flagesPlaced = 0;
        gameOver = false;
        currentTime = 0;
    }
    void LeftClick(int wPos, int hPos) {
        if (mineField[wPos, hPos].state.Equals(0) && !mineField[wPos, hPos].isPressed) {
            if (mineField[wPos, hPos].containMine) {
                mineField[wPos, hPos].tileGO.GetComponent<SpriteRenderer>().color = Color.red;
                DisplayAllMines();
                gameOver = true;
            }
            else {
                UpdateVision(wPos, hPos);
            }
        }
    }
    void UpdateVision(int wp, int hp) {
        List<Vector2Int> remainingPos = new List<Vector2Int>();
        remainingPos.Add(new Vector2Int(wp, hp));
        while (remainingPos.Count > 0) {
            if (mineField[remainingPos[0].x, remainingPos[0].y].isPressed) {// as dublet of the same position can be stored this skips them to make it load faster
                remainingPos.RemoveAt(0);
                continue;
            }
            // for every squere change color and marje them as pressed
            mineField[remainingPos[0].x, remainingPos[0].y].tileGO.GetComponent<SpriteRenderer>().color = (((remainingPos[0].x + remainingPos[0].y) % 2) == 0) ? BGColor1 : BGColor2;
            mineField[remainingPos[0].x, remainingPos[0].y].isPressed = true;
            GenerateNumber(remainingPos[0].x, remainingPos[0].y);

            //add all adjacent tiles to the queue to be progressed 
            if (mineField[remainingPos[0].x, remainingPos[0].y].colidingMineAmount <= 0) {
                if (remainingPos[0].x >= 1) {
                    if (!mineField[remainingPos[0].x - 1, remainingPos[0].y].isPressed) {
                        remainingPos.Add(new Vector2Int(remainingPos[0].x - 1, remainingPos[0].y));
                    }
                    if (remainingPos[0].y >= 1) {
                        if (!mineField[remainingPos[0].x - 1, remainingPos[0].y - 1].isPressed) {
                            remainingPos.Add(new Vector2Int(remainingPos[0].x - 1, remainingPos[0].y - 1));
                        }
                    }
                    if (remainingPos[0].y < height - 1) {
                        if (!mineField[remainingPos[0].x - 1, remainingPos[0].y + 1].isPressed) {
                            remainingPos.Add(new Vector2Int(remainingPos[0].x - 1, remainingPos[0].y + 1));
                        }
                    }
                }
                if (remainingPos[0].x < width - 1) {
                    if (!mineField[remainingPos[0].x + 1, remainingPos[0].y].isPressed) {
                        remainingPos.Add(new Vector2Int(remainingPos[0].x + 1, remainingPos[0].y));
                    }
                    if (remainingPos[0].y >= 1) {
                        if (!mineField[remainingPos[0].x + 1, remainingPos[0].y - 1].isPressed) {
                            remainingPos.Add(new Vector2Int(remainingPos[0].x + 1, remainingPos[0].y - 1));
                        }
                    }
                    if (remainingPos[0].y < height - 1) {
                        if (!mineField[remainingPos[0].x + 1, remainingPos[0].y + 1].isPressed) {
                            remainingPos.Add(new Vector2Int(remainingPos[0].x + 1, remainingPos[0].y + 1));
                        }
                    }
                }
                if (remainingPos[0].y >= 1) {
                    if (!mineField[remainingPos[0].x, remainingPos[0].y - 1].isPressed) {
                        remainingPos.Add(new Vector2Int(remainingPos[0].x, remainingPos[0].y - 1));
                    }
                }
                if (remainingPos[0].y < height - 1) {
                    if (!mineField[remainingPos[0].x, remainingPos[0].y + 1].isPressed) {
                        remainingPos.Add(new Vector2Int(remainingPos[0].x, remainingPos[0].y + 1));
                    }
                }
            }
            remainingPos.RemoveAt(0);
            currentFound++;
        }
        if (currentFound >= minSafeFound) {
            LookForWinState();
        }
    }
    void LookForWinState() {
        if (currentFound >= minSafeFound &&
            flagesPlaced >= amountOfMines) {
            DisplayAllMines();
            gameOver = true;
        }
    }
    void GenerateNumber(int w, int h) {
        if (mineField[w, h].colidingMineAmount <= 0)
            return;

        GameObject newNumber = Instantiate(numberOfColidingMines);
        newNumber.transform.position = new Vector3(w, h, numberOfColidingMines.transform.position.z);
        newNumber.transform.SetParent(mineField[w, h].tileGO.transform);
        TextMesh text = newNumber.GetComponent<TextMesh>();
        text.text = mineField[w, h].colidingMineAmount.ToString();
        switch (mineField[w, h].colidingMineAmount) {
            case (1):
                text.color = mine1;
                break;
            case (2):
                text.color = mine2;
                break;
            case (3):
                text.color = mine3;
                break;
            case (4):
                text.color = mine4;
                break;
            case (5):
                text.color = mine5;
                break;
            case (6):
                text.color = mine6;
                break;
            case (7):
                text.color = mine7;
                break;
            case (8):
                text.color = mine8;
                break;
        }
    }

    void RightClick(int wPos, int hPos) {
        if (mineField[wPos, hPos].isPressed) {
            return;
        }
        mineField[wPos, hPos].state++;
        if (mineField[wPos, hPos].state.Equals(1)) {//add one blue flag
            GameObject newFlag = Instantiate(flag);
            newFlag.transform.localPosition = new Vector3(wPos, hPos);
            newFlag.transform.SetParent(mineField[wPos, hPos].tileGO.transform);

            newFlag.gameObject.GetComponent<SpriteRenderer>().color = flagColor1;
            mineField[wPos, hPos].flagGO = newFlag;

            flagesPlaced++;
            LookForWinState();// look if all mines are marked and no otther tiles exsist, if so the game is won
        }
        else if (mineField[wPos, hPos].state.Equals(2)) {//change flags color to orange
            mineField[wPos, hPos].flagGO.GetComponent<SpriteRenderer>().color = flagColor2;
        }
        else if (mineField[wPos, hPos].state >= 3) {//value to large, change back to zero and remove any flag
            mineField[wPos, hPos].state = 0;
            flagesPlaced--;
            Destroy(mineField[wPos, hPos].flagGO);
        }
    }


    void PlaceMines(int wPos, int hPos) {
        List<Vector2Int> listOfArr = new List<Vector2Int>();
        //generete all possible spawning positions for the mines
        //remove first mine to avoid sudden death at start as well as all adjecent tiles, to avoid starting on a number
        for (int w = 0; w < width; w++) {
            for (int h = 0; h < height; h++) {
                if (w.Equals(wPos) && h.Equals(hPos + 1)) {
                    continue;
                }
                if (w.Equals(wPos) && h.Equals(hPos - 1)) {
                    continue;
                }
                if (w.Equals(wPos) && h.Equals(hPos)) {
                    continue;
                }
                if (w.Equals(wPos + 1) && h.Equals(hPos + 1)) {
                    continue;
                }
                if (w.Equals(wPos + 1) && h.Equals(hPos - 1)) {
                    continue;
                }
                if (w.Equals(wPos + 1) && h.Equals(hPos)) {
                    continue;
                }
                if (w.Equals(wPos - 1) && h.Equals(hPos + 1)) {
                    continue;
                }
                if (w.Equals(wPos - 1) && h.Equals(hPos - 1)) {
                    continue;
                }
                if (w.Equals(wPos - 1) && h.Equals(hPos)) {
                    continue;
                }

                listOfArr.Add(new Vector2Int(w, h));
            }
        }
        //shuffle tiles to place mines at random
        for (int i = 0; i < listOfArr.Count; i++) {
            int randomIndex = Random.Range(i, listOfArr.Count);

            Vector2Int holderOfArr = listOfArr[i];
            listOfArr[i] = listOfArr[randomIndex];
            listOfArr[randomIndex] = holderOfArr;
        }
        //place mines
        for (int i = 0; i < amountOfMines; i++) {
            mineField[listOfArr[i].x, listOfArr[i].y].containMine = true;
        }
        //count each tile's mine colliding amount
        for (int w = 0; w < width; w++) {
            for (int h = 0; h < height; h++) {
                int countedMines = 0;
                if (w >= 1) {
                    if (mineField[w - 1, h].containMine) {
                        countedMines++;
                    }
                    if (h >= 1) {
                        if (mineField[w - 1, h - 1].containMine) {
                            countedMines++;
                        }
                    }
                    if (h < height - 1) {
                        if (mineField[w - 1, h + 1].containMine) {
                            countedMines++;
                        }
                    }
                }
                if (w < width - 1) {
                    if (mineField[w + 1, h].containMine) {
                        countedMines++;
                    }
                    if (h >= 1) {
                        if (mineField[w + 1, h - 1].containMine) {
                            countedMines++;
                        }
                    }
                    if (h < height - 1) {
                        if (mineField[w + 1, h + 1].containMine) {
                            countedMines++;
                        }
                    }
                }
                if (h >= 1) {
                    if (mineField[w, h - 1].containMine) {
                        countedMines++;
                    }
                }
                if (h < height - 1) {
                    if (mineField[w, h + 1].containMine) {
                        countedMines++;
                    }
                }
                mineField[w, h].colidingMineAmount = countedMines;
            }
        }
    }

    //called on gameover, display where all mines is
    void DisplayAllMines() {
        for (int w = 0; w < width; w++) {
            for (int h = 0; h < height; h++) {
                if (mineField[w, h].containMine) {
                    GameObject newMine = Instantiate(mineGO,
                        mineField[w, h].tileGO.transform.position,
                        mineField[w, h].tileGO.transform.rotation,
                        mineField[w, h].tileGO.transform);
                }
            }
        }
    }

    bool firstPress = true;
    int minSafeFound, currentFound;

    float currentTime;
    private void Update() {
        if (isPaused || gameOver)
            return;
        //update game text to match latest varible's values
        string tempString = baseGameText;
        currentTime += Time.deltaTime;
        tempString = tempString.Replace("[$t]", (currentTime / 60).ToString("00") + ":" + (currentTime % 60).ToString("00"));

        tempString = tempString.Replace("[$fP]", flagesPlaced.ToString());
        tempString = tempString.Replace("[$tM]", amountOfMines.ToString());
        gameText.text = tempString;

        if (Input.GetMouseButtonDown(0)) {
            LayerMask mask = LayerMask.GetMask("Tile");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, mask);
            if (hit.collider != null) {
                Tile tile = mineField[(int)hit.transform.position.x, (int)hit.transform.position.y];

                if (firstPress) {
                    PlaceMines((int)tile.tileGO.transform.position.x, (int)tile.tileGO.transform.position.y);
                    firstPress = false;
                }
                LeftClick((int)tile.tileGO.transform.position.x, (int)tile.tileGO.transform.position.y);
            }
        }
        if (Input.GetMouseButtonDown(1)) {
            LayerMask mask = LayerMask.GetMask("Tile");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, mask);
            if (hit.collider != null) {
                Tile tile = mineField[(int)hit.transform.position.x, (int)hit.transform.position.y];
                RightClick((int)tile.tileGO.transform.position.x, (int)tile.tileGO.transform.position.y);
            }
        }
    }


    Vector2Int currentHint;
    bool haveHint;
    public void GenerateClue() {
        List<Vector2Int> possibleHints = new List<Vector2Int>();//list of possible squares
        List<Vector2Int> hidenFlags = new List<Vector2Int>();//list of safe tiles that the player might not know of
        //if it is not empty calculate all possible tiles
        #region naive check
        /*
         * this check will only track the obviously missed as to find all guaranteed bombs and safe tiles
         * doing this will result in some "misses" where no result is given, there are more advanced rules to be implemented
         * to find these but nothing that is implemented here. 
         * Also: this func can generate wrong answers if and only if the player places a flag on a safe tile, as the computer 
         * uses these tiles in it's calculation. The algorithm could be changed to find safe tiles and ignore the playuers input but
         * where is the sport in that?
         * 
         * also yes, for a real game i could simply implement that the ai gives a random right answer since the computer already 
         * store the right answers, but to help the player find relevant information it was implemented like a algorithm to play
         * the game.
         * 
         */
        for (int w = 0; w < width; w++) {
            for (int h = 0; h < height; h++) {
                if (!mineField[w, h].isPressed)
                    continue;

                int flagedTiles = 0;
                int unkownTiles = 0;
                int amountOBombs = mineField[w, h].colidingMineAmount;
                List<Vector2Int> potentialBombs = new List<Vector2Int>();
                if (w >= 1) {
                    if (!mineField[w - 1, h].isPressed && mineField[w - 1, h].state == 0) {
                        unkownTiles++;
                        potentialBombs.Add(new Vector2Int(w - 1, h));
                    }
                    if (mineField[w - 1, h].state > 0) {
                        flagedTiles++;
                    }
                    if (h >= 1) {
                        if (!mineField[w - 1, h - 1].isPressed && mineField[w - 1, h - 1].state == 0) {
                            unkownTiles++;
                            potentialBombs.Add(new Vector2Int(w - 1, h - 1));
                        }
                        if (mineField[w - 1, h - 1].state > 0) {
                            flagedTiles++;
                        }
                    }
                    if (h < height - 1) {
                        if (!mineField[w - 1, h + 1].isPressed && mineField[w - 1, h + 1].state == 0) {
                            potentialBombs.Add(new Vector2Int(w - 1, h + 1));
                            unkownTiles++;
                        }
                        if (mineField[w - 1, h + 1].state > 0) {
                            flagedTiles++;
                        }
                    }
                }
                if (w < width - 1) {
                    if (!mineField[w + 1, h].isPressed && mineField[w + 1, h].state == 0) {
                        potentialBombs.Add(new Vector2Int(w + 1, h));
                        unkownTiles++;
                    }
                    if (mineField[w + 1, h].state > 0) {
                        flagedTiles++;
                    }
                    if (h >= 1) {
                        if (!mineField[w + 1, h - 1].isPressed && mineField[w + 1, h - 1].state == 0) {
                            potentialBombs.Add(new Vector2Int(w + 1, h - 1));
                            unkownTiles++;
                        }
                        if (mineField[w + 1, h - 1].state > 0) {
                            flagedTiles++;
                        }
                    }
                    if (h < height - 1) {
                        if (!mineField[w + 1, h + 1].isPressed && mineField[w + 1, h + 1].state == 0) {
                            potentialBombs.Add(new Vector2Int(w + 1, h + 1));
                            unkownTiles++;
                        }
                        if (mineField[w + 1, h + 1].state > 0) {
                            flagedTiles++;
                        }
                    }
                }
                if (h >= 1) {
                    if (!mineField[w, h - 1].isPressed && mineField[w, h - 1].state == 0) {
                        potentialBombs.Add(new Vector2Int(w, h - 1));
                        unkownTiles++;
                    }
                    if (mineField[w, h - 1].state > 0) {
                        flagedTiles++;
                    }
                }
                if (h < height - 1) {
                    if (!mineField[w, h + 1].isPressed && mineField[w, h + 1].state == 0) {
                        potentialBombs.Add(new Vector2Int(w, h + 1));
                        unkownTiles++;
                    }
                    if (mineField[w, h + 1].state > 0) {
                        flagedTiles++;
                    }
                }
                if ((amountOBombs - flagedTiles).Equals(unkownTiles)) {
                    foreach (Vector2Int ar in potentialBombs) {
                        hidenFlags.Add(ar);
                        mineField[ar.x, ar.y].state = 3;
                    }
                }
            }
        }

        //look for bombs that player have missed
        for (int w = 0; w < width; w++) {
            for (int h = 0; h < height; h++) {
                if (!mineField[w, h].isPressed)
                    continue;

                int flagedTiles = 0;
                int amountOBombs = mineField[w, h].colidingMineAmount;
                List<Vector2Int> potentialBombs = new List<Vector2Int>();
                if (w >= 1) {
                    if (!mineField[w - 1, h].isPressed && mineField[w - 1, h].state == 0) {
                        potentialBombs.Add(new Vector2Int(w - 1, h));
                    }
                    if (mineField[w - 1, h].state > 0) {
                        flagedTiles++;
                    }
                    if (h >= 1) {
                        if (!mineField[w - 1, h - 1].isPressed && mineField[w - 1, h - 1].state == 0) {
                            potentialBombs.Add(new Vector2Int(w - 1, h - 1));
                        }
                        if (mineField[w - 1, h - 1].state > 0) {
                            flagedTiles++;
                        }
                    }
                    if (h < height - 1) {
                        if (!mineField[w - 1, h + 1].isPressed && mineField[w - 1, h + 1].state == 0) {
                            potentialBombs.Add(new Vector2Int(w - 1, h + 1));
                        }
                        if (mineField[w - 1, h + 1].state > 0) {
                            flagedTiles++;
                        }
                    }
                }
                if (w < width - 1) {
                    if (!mineField[w + 1, h].isPressed && mineField[w + 1, h].state == 0) {
                        potentialBombs.Add(new Vector2Int(w + 1, h));
                    }
                    if (mineField[w + 1, h].state > 0) {
                        flagedTiles++;
                    }
                    if (h >= 1) {
                        if (!mineField[w + 1, h - 1].isPressed && mineField[w + 1, h - 1].state == 0) {
                            potentialBombs.Add(new Vector2Int(w + 1, h - 1));
                        }
                        if (mineField[w + 1, h - 1].state > 0) {
                            flagedTiles++;
                        }
                    }
                    if (h < height - 1) {
                        if (!mineField[w + 1, h + 1].isPressed && mineField[w + 1, h + 1].state == 0) {
                            potentialBombs.Add(new Vector2Int(w + 1, h + 1));
                        }
                        if (mineField[w + 1, h + 1].state > 0) {
                            flagedTiles++;
                        }
                    }
                }
                if (h >= 1) {
                    if (!mineField[w, h - 1].isPressed && mineField[w, h - 1].state == 0) {
                        potentialBombs.Add(new Vector2Int(w, h - 1));
                    }
                    if (mineField[w, h - 1].state > 0) {
                        flagedTiles++;
                    }
                }
                if (h < height - 1) {
                    if (!mineField[w, h + 1].isPressed && mineField[w, h + 1].state == 0) {
                        potentialBombs.Add(new Vector2Int(w, h + 1));
                    }
                    if (mineField[w, h + 1].state > 0) {
                        flagedTiles++;
                    }
                }
                if (flagedTiles.Equals(amountOBombs)) {
                    foreach (Vector2Int ar in potentialBombs) {
                        if (!possibleHints.Contains(ar) && !mineField[ar.x, ar.y].isPressed) {
                            possibleHints.Add(ar);
                        }
                    }
                }
            }
        }
        foreach (Vector2Int ar in hidenFlags) {
            mineField[ar.x, ar.y].state = 0;
        }
        #endregion

        //if list is not empty pick a random number and return
        if (possibleHints.Count > 0) {
            PickRandomClue(possibleHints);
        }
        else {//if not display that no clear answers can be given
            if (isAnimating) {
                StopAllCoroutines();
                StartCoroutine(InteruptedFadeInNoClueText());
            }
            else {
                StartCoroutine(FadeInNoClueText());
            }
        }
    }
    void PickRandomClue(List<Vector2Int> possibleHints) {
        int randomIndex = Random.RandomRange(0, possibleHints.Count - 1);
        if (!haveHint) {
            if (!mineField[currentHint.x, currentHint.y].isPressed) {
                mineField[currentHint.x, currentHint.y].tileGO.GetComponent<SpriteRenderer>().color =
                    (((currentHint.x + currentHint.y) % 2) == 0) ? color1 : color2;
            }
        }
        else {
            haveHint = true;
        }
        currentHint = new Vector2Int(possibleHints[randomIndex].x, possibleHints[randomIndex].y);
        possibleHints.RemoveAt(randomIndex);
        mineField[currentHint.x, currentHint.y].tileGO.GetComponent<SpriteRenderer>().color = (((currentHint.x + currentHint.y) % 2) == 0) ? hintColor1 : hintColor2;
    }

    bool isPaused = false;
    [SerializeField] Text pausedText;
    public void PauseTheGame() {
        isPaused = !isPaused;
        pausedText.text = isPaused ? "Unpause" : "Pause";
    }
    public void Restart() {
        GenerateTiles();
        //restore values to default values so when a new minefield is generated.
        //this will be done the next frame too in the update. But it looks nicer like
        //this if the game is paused. 

        string tempString = baseGameText;
        currentTime += Time.deltaTime;
        tempString = tempString.Replace("[$t]", (currentTime / 60).ToString("00") + ":" + (currentTime % 60).ToString("00"));

        tempString = tempString.Replace("[$fP]", flagesPlaced.ToString());
        tempString = tempString.Replace("[$tM]", amountOfMines.ToString());
        gameText.text = tempString;
    }
    public void ExitGame() {
        Application.Quit(); //qui the game
    }

    [SerializeField] Text hintText;
    [SerializeField] float fadeInDuration, displayDuration, fadeAwayDuration;
    bool isAnimating = false;

    IEnumerator FadeInNoClueText() {
        isAnimating = true;
        Color color = hintText.color;
        Color hidenColor = new Color(color.r, color.g, color.b, 0);
        Color displayColor = new Color(color.r, color.g, color.b, 1);

        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration) {//slowly fade in the text
            elapsedTime += Time.deltaTime;
            hintText.color = Color.Lerp(hidenColor, displayColor, elapsedTime / fadeInDuration);
            yield return null;
        }
        //keep it as it is for x minutes
        yield return new WaitForSeconds(displayDuration + (fadeInDuration - elapsedTime));

        //slowly fade out again
        elapsedTime = 0f;
        while (elapsedTime < fadeAwayDuration) {
            elapsedTime += Time.deltaTime;
            hintText.color = Color.Lerp(displayColor, hidenColor, elapsedTime / fadeAwayDuration);
            yield return null;
        }
        isAnimating = false;
    }
    //if last (or this) is interrupted, another animation plays instead this
    //animation skip immidatly to a alpha 1 and after a while  fades away
    IEnumerator InteruptedFadeInNoClueText() {
        Color color = hintText.color;
        hintText.color = new Color(color.r, color.g, color.b, 1);
        Color hidenColor = new Color(color.r, color.g, color.b, 0);
        Color displayColor = new Color(color.r, color.g, color.b, 1);


        yield return new WaitForSeconds(displayDuration);

        float elapsedTime = 0f;
        while (elapsedTime < fadeAwayDuration) {
            elapsedTime += Time.deltaTime;
            hintText.color = Color.Lerp(displayColor, hidenColor, elapsedTime / fadeAwayDuration);
            yield return null;
        }
        isAnimating = false;
    }

    public struct Tile
    {
        public bool containMine;
        public int colidingMineAmount;
        public GameObject tileGO;
        public int state; //ranging from 0-3 
        /*
         * 0: not marked with anything
         * 1: blue flag
         * 2: orange flag
         * 3: hidden flag, a emporary flag the computer uses for hints inicating it found out it is safe but it is not marked by the player
         */
        public GameObject flagGO;
        public bool isPressed;
    }
}
