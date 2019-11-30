using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VoxelGrid : MonoBehaviour {
  private const int SIZE = 12;
  private bool[,,] bool_array = new bool[SIZE, SIZE, SIZE];
  private bool[,,] temp_bool_array = new bool[SIZE, SIZE, SIZE];
  private GameObject[,,] cube_array = new GameObject[SIZE, SIZE, SIZE];
  private float spawnTime;
  private float alpha;

  public Text title;
  //public GameObject refresh;
  public Button refresh;
  public Material cube_material;
  public Material sphere_material;
  public float color_speed = 0.003f;
  public int[] param = new int[4]{4, 6, 6, 6};

  IEnumerator Start() {
    refresh.enabled = false;
    StartCoroutine(fadeButton(refresh, false, 0));
    Graphic title_g = title.GetComponent<Graphic>();
    title_g.GetComponent<CanvasRenderer>().SetAlpha(0f);
    yield return new WaitForSeconds(2f);
    FadeIn(title_g);
    yield return new WaitForSeconds(4f);
    FadeOut(title_g);
    yield return new WaitForSeconds(2f);

    sphere_material.SetColor("_Color", new Color(1, 1, 1, 1));
    for (int i = 0; i < SIZE; i++) {
      for (int j = 0; j < SIZE; j++) {
        for (int k = 0; k < SIZE; k++) {
          if (Random.value >= 0.75) {
            bool_array[i, j, k] = true;
          } else {
            bool_array[i, j, k] = false;
          }
          cube_array[i, j, k] = GameObject.CreatePrimitive(PrimitiveType.Cube);
          cube_array[i, j, k].transform.localScale = new Vector3(1f, 1f, 1f);
          cube_array[i, j, k].transform.position = new Vector3(i - (SIZE / 2), j - (SIZE / 2), k - (SIZE / 2));

          cube_array[i, j, k].GetComponent<Renderer>().material = cube_material;
          if (!bool_array[i, j, k]) {
            cube_array[i, j, k].GetComponent<MeshRenderer>().enabled = false;
          }
        }
      }
    }
    spawnTime = Time.time;
    InvokeRepeating("RunGame", 0.1f, 0.1f);

    yield return new WaitForSeconds(4.0f);
    StartCoroutine(fadeButton(refresh, true, 2.0f));
    refresh.enabled = true;
    //refresh.SetActive(true);
  }

  void Update() {
    alpha = 1.0f - ((Time.time - spawnTime) * 0.3f);
    alpha = Mathf.Clamp(alpha, 0, 1.0f);
    sphere_material.SetColor("_Color", new Color(1, 1, 1, alpha));
    cube_material.SetColor("_Color", HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * color_speed, 1), 1, 1, 0.9f)));
  }

  void FadeIn(Graphic g) {
    g.GetComponent<CanvasRenderer>().SetAlpha(0f);
    g.CrossFadeAlpha(1f, 1f, false);//second param is the time
  }

  void FadeOut(Graphic g) {
    g.GetComponent<CanvasRenderer>().SetAlpha(1f);
    g.CrossFadeAlpha(0f, 1f, false);
  }

  int LiveNeighbours(int i, int j, int k) {
    int count = 0;
    for (int x = -1; x <= 1; x++) {
      for (int y = -1; y <= 1; y++) {
        for (int z = -1; z <= 1; z++) {
          int c_x = i + x;
          int c_y = j + y;
          int c_z = k + z;

          if (c_x >= 0 && c_x < SIZE && c_y >= 0 && c_y < SIZE && c_z >= 0 && c_z < SIZE && (x != 0 || y != 0 || z != 0)) {
            if (bool_array[c_x, c_y, c_z]) {
              count++;
            }
          }
        }
      }
    }
    return count;
  }

  void RunGame() {
    for (int i = 0; i < SIZE; i++) {
      for (int j = 0; j < SIZE; j++) {
        for (int k = 0; k < SIZE; k++) {
          int count = LiveNeighbours(i, j, k);
          if (bool_array[i, j, k]) {
            if (param[0] <= count && count <= param[1]) {
              temp_bool_array[i, j, k] = true;
            } else {
              temp_bool_array[i, j, k] = false;
              cube_array[i, j, k].GetComponent<MeshRenderer>().enabled = false;
            }
          } else {
            if (param[2] <= count && count <= param[3]) {
              temp_bool_array[i, j, k] = true;
              cube_array[i, j, k].GetComponent<MeshRenderer>().enabled = true;
            } else {
              temp_bool_array[i, j, k] = false;
            }
          }
        }
      }
    }

    bool_array = temp_bool_array;
  }

  IEnumerator fadeButton(Button button, bool fadeIn, float duration) {

    float counter = 0f;

    //Set Values depending on if fadeIn or fadeOut
    float a, b;
    if (fadeIn) {
      a = 0;
      b = 1;
    } else {
      a = 1;
      b = 0;
    }

    Image buttonImage = button.GetComponent<Image>();
    Text buttonText = button.GetComponentInChildren<Text>();

    //Enable both Button, Image and Text components
    if (!button.enabled)
      button.enabled = true;

    if (!buttonImage.enabled)
      buttonImage.enabled = true;

    if (!buttonText.enabled)
      buttonText.enabled = true;

    //For Button None or ColorTint mode
    Color buttonColor = buttonImage.color;
    Color textColor = buttonText.color;

    //For Button SpriteSwap mode
    ColorBlock colorBlock = button.colors;


    //Do the actual fading
    while (counter < duration) {
      counter += Time.deltaTime;
      float alpha = Mathf.Lerp(a, b, counter / duration);
      //Debug.Log(alpha);

      if (button.transition == Selectable.Transition.None || button.transition == Selectable.Transition.ColorTint) {
        buttonImage.color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, alpha);//Fade Traget Image
        buttonText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);//Fade Text
      } else if (button.transition == Selectable.Transition.SpriteSwap) {
        ////Fade All Transition Images
        colorBlock.normalColor = new Color(colorBlock.normalColor.r, colorBlock.normalColor.g, colorBlock.normalColor.b, alpha);
        colorBlock.pressedColor = new Color(colorBlock.pressedColor.r, colorBlock.pressedColor.g, colorBlock.pressedColor.b, alpha);
        colorBlock.highlightedColor = new Color(colorBlock.highlightedColor.r, colorBlock.highlightedColor.g, colorBlock.highlightedColor.b, alpha);
        colorBlock.disabledColor = new Color(colorBlock.disabledColor.r, colorBlock.disabledColor.g, colorBlock.disabledColor.b, alpha);

        button.colors = colorBlock; //Assign the colors back to the Button
        buttonImage.color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, alpha);//Fade Traget Image
        buttonText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);//Fade Text
      } else {
        Debug.LogError("Button Transition Type not Supported");
      }

      yield return null;
    }

    if (!fadeIn) {
      //Disable both Button, Image and Text components
      buttonImage.enabled = false;
      buttonText.enabled = false;
      button.enabled = false;
    }
  }

  public void ButtonPressed() {
    for (int i = 0; i < SIZE; i++) {
      for (int j = 0; j < SIZE; j++) {
        for (int k = 0; k < SIZE; k++) {
          if (Random.value >= 0.75) {
            bool_array[i, j, k] = true;
          } else {
            bool_array[i, j, k] = false;
          }

          if (!bool_array[i, j, k]) {
            cube_array[i, j, k].GetComponent<MeshRenderer>().enabled = false;
          } else {
            cube_array[i, j, k].GetComponent<MeshRenderer>().enabled = true;
          }
        }
      }
    }
  }
}