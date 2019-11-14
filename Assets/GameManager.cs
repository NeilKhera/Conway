using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public Sprite sprite;
    public float[,] array;
    int height, width;
    int rows, columns;
    // Start is called before the first frame update
    void Start() {
        height = (int)Camera.main.orthographicSize;
        width = height * (Screen.width / Screen.height);
        rows = height * 2;
        columns = width * 2;
        array = new float[columns, rows];
        for (int i = 0; i < columns; i++) {
            for (int j = 0; j < rows; j++) {
                array[i, j] = Random.Range(0.0f, 1.0f);
                SpawnTile(i, j, array[i, j]);
            }  
        }
    }

    private void SpawnTile(int x, int y, float value) {
        GameObject g = new GameObject("x: " + x + " y: " + y);
        g.transform.position = new Vector3(x - (width - 0.5f), y - (height - 0.5f));
        var s = g.AddComponent<SpriteRenderer>();
        s.sprite = sprite;
        s.color = new Color(value, value, value);
    }
}
