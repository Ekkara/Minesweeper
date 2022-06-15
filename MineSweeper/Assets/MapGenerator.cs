using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] [Range(9, 100)] int width = 18, height = 14;
    [SerializeField] int mines = 10;
    [SerializeField] GameObject tile;
    [SerializeField] Color color1, color2, BGColor1, BGColor2, flagColor1, flagColor2;
    [SerializeField] GameObject flag;
    [SerializeField] GameObject numberOfColidingMines;
    [SerializeField] Color mine1, mine2, mine3, mine4, mine5, mine6, mine7, mine8;
    [SerializeField] Tile[,] mineField;
    [HideInInspector] [SerializeField] bool mapMade = false;

    private void OnValidate() {
        if (mines >= ((width * height)-9)) {
            int newAmount = ((width * height)) - 9;
            Debug.LogWarning("mines can't cover the entire field, value changed to entire field minus nine, the new value is: " + newAmount);
            mines = newAmount;
        }
        if(numberOfColidingMines.GetComponent<TextMesh>() == null) {
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
        minSafeFound = (width * height) - mines;
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
    }
    void LeftClick(int wPos, int hPos) {
        if (mineField[wPos, hPos].state.Equals(0) && !mineField[wPos, hPos].isPressed) {           
            if (mineField[wPos, hPos].containMine) {
                mineField[wPos, hPos].tileGO.GetComponent<SpriteRenderer>().color = Color.red;
                GenerateTiles();
            }
            else {
                UpdateVision(wPos, hPos);
            }
        }
    }
    void UpdateVision(int wp, int hp) {
        List<Arr> remainingPos = new List<Arr>();
        remainingPos.Add(new Arr(wp, hp));
        while(remainingPos.Count > 0) {
            if (mineField[remainingPos[0].w, remainingPos[0].h].isPressed) {
                remainingPos.RemoveAt(0);
                continue;
            }


            mineField[remainingPos[0].w, remainingPos[0].h].tileGO.GetComponent<SpriteRenderer>().color = (((remainingPos[0].w + remainingPos[0].h) % 2) == 0) ? BGColor1 : BGColor2;
            mineField[remainingPos[0].w, remainingPos[0].h].isPressed = true;
            GenerateNumber(remainingPos[0].w, remainingPos[0].h);

            if (mineField[remainingPos[0].w, remainingPos[0].h].colidingMineAmount <= 0) {
                if (remainingPos[0].w >= 1) {
                    if (!mineField[remainingPos[0].w - 1, remainingPos[0].h].isPressed) {
                        remainingPos.Add(new Arr(remainingPos[0].w - 1, remainingPos[0].h));
                    }
                    if (remainingPos[0].h >= 1) {
                        if (!mineField[remainingPos[0].w - 1, remainingPos[0].h - 1].isPressed) {
                            remainingPos.Add(new Arr(remainingPos[0].w - 1, remainingPos[0].h - 1));
                        }
                    }
                    if (remainingPos[0].h < height - 1) {
                        if (!mineField[remainingPos[0].w - 1, remainingPos[0].h + 1].isPressed) {
                            remainingPos.Add(new Arr(remainingPos[0].w - 1, remainingPos[0].h + 1));
                        }
                    }
                }
                if (remainingPos[0].w < width - 1) {
                    if (!mineField[remainingPos[0].w + 1, remainingPos[0].h].isPressed) {
                        remainingPos.Add(new Arr(remainingPos[0].w + 1, remainingPos[0].h));
                    }
                    if (remainingPos[0].h >= 1) {
                        if (!mineField[remainingPos[0].w + 1, remainingPos[0].h - 1].isPressed) {
                            remainingPos.Add(new Arr(remainingPos[0].w + 1, remainingPos[0].h - 1));
                        }
                    }
                    if (remainingPos[0].h < height - 1) {
                        if (!mineField[remainingPos[0].w + 1, remainingPos[0].h + 1].isPressed) {
                            remainingPos.Add(new Arr(remainingPos[0].w + 1, remainingPos[0].h + 1));
                        }
                    }
                }
                if (remainingPos[0].h >= 1) {
                    if (!mineField[remainingPos[0].w, remainingPos[0].h - 1].isPressed) {
                        remainingPos.Add(new Arr(remainingPos[0].w, remainingPos[0].h - 1));
                    }
                }
                if (remainingPos[0].h < height - 1) {
                    if (!mineField[remainingPos[0].w, remainingPos[0].h + 1].isPressed) {
                        remainingPos.Add(new Arr(remainingPos[0].w, remainingPos[0].h + 1));
                    }
                }
            }
            remainingPos.RemoveAt(0);
            currentFound++;
        }
        if(currentFound >= minSafeFound) {
            Debug.Log("Win");
        }
    }
    void GenerateNumber(int w, int h) {
        if (mineField[w, h].colidingMineAmount <= 0)
            return;

        GameObject newNumber = Instantiate(numberOfColidingMines);
        newNumber.transform.position = new Vector3(w,h,numberOfColidingMines.transform.position.z);
        newNumber.transform.SetParent(mineField[w, h].tileGO.transform);
        TextMesh text = newNumber.GetComponent<TextMesh>();
        text.text = mineField[w, h].colidingMineAmount.ToString();
        switch(mineField[w, h].colidingMineAmount){
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
        if (mineField[wPos, hPos].state >= 3) {
            mineField[wPos, hPos].state = 0;
        }
        if (mineField[wPos, hPos].state.Equals(0)) {
            Destroy(mineField[wPos, hPos].flagGO);
        }
        else if (mineField[wPos, hPos].state.Equals(1)) {
            GameObject newFlag = Instantiate(flag);
            newFlag.transform.localPosition = new Vector3(wPos, hPos);
            newFlag.transform.SetParent(mineField[wPos, hPos].tileGO.transform);

            newFlag.gameObject.GetComponent<SpriteRenderer>().color = flagColor1;
            mineField[wPos, hPos].flagGO = newFlag;
        }
        else if (mineField[wPos, hPos].state.Equals(2)) {
            Destroy(mineField[wPos, hPos].flagGO);
            GameObject newFlag = Instantiate(flag);
            newFlag.transform.position = new Vector3(wPos, hPos);
            newFlag.transform.SetParent(mineField[wPos, hPos].tileGO.transform);
            newFlag.gameObject.GetComponent<SpriteRenderer>().color = flagColor2;
            mineField[wPos, hPos].flagGO = newFlag;
        }
    }


    void PlaceMines(int wPos, int hPos) {
        List<Arr> listOfArr = new List<Arr>();
        //generete all possible spawning positions for the mines
        //remove first mine to avoid sudden death at start as well as all adjecent tiles, to avoid starting on a number
        for (int w = 0; w < width; w++) {
            for (int h = 0; h < height; h++) {
                if (w.Equals(wPos) && h.Equals(hPos+1)) {
                    continue;
                }
                if (w.Equals(wPos) && h.Equals(hPos-1)) {
                    continue;
                }
                if (w.Equals(wPos) && h.Equals(hPos)) {
                    continue;
                }
                if (w.Equals(wPos+1) && h.Equals(hPos+1)) {
                    continue;
                }
                if (w.Equals(wPos+1) && h.Equals(hPos-1)) {
                    continue;
                }
                if (w.Equals(wPos+1) && h.Equals(hPos)) {
                    continue;
                }
                if (w.Equals(wPos-1) && h.Equals(hPos+1)) {
                    continue;
                }
                if (w.Equals(wPos-1) && h.Equals(hPos-1)) {
                    continue;
                }
                if (w.Equals(wPos-1) && h.Equals(hPos)) {
                    continue;
                }

                listOfArr.Add(new Arr(w, h));
            }
        }
        //shuffle tiles to place mines at random
        for (int i = 0; i < listOfArr.Count; i++) {
            int randomIndex = Random.Range(i, listOfArr.Count);

            Arr holderOfArr = listOfArr[i];
            listOfArr[i] = listOfArr[randomIndex];
            listOfArr[randomIndex] = holderOfArr;
        }
        //place mines
        for (int i = 0; i < mines; i++) {
            mineField[listOfArr[i].w, listOfArr[i].h].containMine = true;
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
                        if (mineField[w + 1, h + 1].containMine) 
                        {
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
                    if (mineField[w, h + 1].containMine) 
                        {
                        countedMines++;
                    }
                }
                mineField[w, h].colidingMineAmount = countedMines;
            }
        }
    }
    bool firstPress = true;
    int minSafeFound, currentFound;

    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            LayerMask mask = LayerMask.GetMask("Tile");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, mask);
            if (hit.collider != null) {
                Tile tile = mineField[(int)hit.transform.position.x, (int)hit.transform.position.y];
                Debug.Log(((int)tile.tileGO.transform.position.x).ToString() + ":" + ((int)tile.tileGO.transform.position.y).ToString());
            }
        }
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

    public struct Tile
    {
        public bool containMine;
        public int colidingMineAmount;
        public GameObject tileGO;
        public int state;
        public GameObject flagGO;
        public bool isPressed;
    }

    public class Arr
    {
       public Arr(int w, int h) {
            this.w = w;
            this.h = h;
        }

        public int w;
        public int h;
    }
}
